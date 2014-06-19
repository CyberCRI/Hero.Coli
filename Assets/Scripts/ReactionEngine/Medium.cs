using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class represents a Medium
  \details
  A Medium is an area closed by a something that is permeable or not.
  Each Medium contains a list of molecules wich contains the concentration of
  each kind of molecules.
  Each Medium also has a list of reactions.
  You can define molecule diffusion between mediums with Fick or ActiveTransport.
  \sa Fick
  \sa ActiveTransport
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
*/
using System.Xml;


public class Medium : XmlLoadableImpl
{
  private LinkedList<IReaction> _reactions;             //!< The list of reactions
  private ArrayList             _molecules;             //!< The list of molecules (Molecule)

  private int           _id;                            //!< The id of the Medium
  private string        _name;                          //!< The name of the Medium
  private string        _reactionsSet;                  //!< The ReactionSet id assigned to this Medium
  private string        _moleculesSet;                  //!< The MoleculeSet id assigned to this Medium
  private bool          _enableSequential;
  private bool          _enableNoise;
  private NumberGenerator _numberGenerator;             //!< Random number generator
  private bool          _enableEnergy;
  private float         _energy;                        //!< Represents the quantity of ATP
  private float 		_energyVariation;				//!< The variation of energy during one frame
  private float         _maxEnergy;                     //!< The maximum quantity of ATP
  private float         _energyProductionRate;          //!< The energy production speed
  public bool           enableShufflingReactionOrder;   //!< Enables shuffling of reactions

  public void setId(int id) { _id = id;}
  public int getId() { return _id;}
  public void setName(string name) { _name = name;}
  public string getName() { return _name;}
  public void setReactions(LinkedList<IReaction> RL) { _reactions = RL;}
  public LinkedList<IReaction> getReactions() { return _reactions;}
  public void setReactionSet(string reactionsSet) { _reactionsSet = reactionsSet;}
  public string getReactionSet() { return _reactionsSet;}
  public void setMoleculeSet(string moleculesSet) { _moleculesSet = moleculesSet;}
  public string getMoleculeSet() { return _moleculesSet;}
  public ArrayList getMolecules() { return _molecules; }

  //TODO extract energy methods and fields and make class out of it
  public void setEnergy(float v) { _energy = Mathf.Min(v, _maxEnergy); if (_energy < 0f) _energy = 0f;}
  public float getEnergy() { return _energy; }
  public void addEnergy(float v) {
		addVariation(v);
		//_energy += v; if (_energy < 0) _energy = 0f; else if (_energy > _maxEnergy) _energy = _maxEnergy;
	}
  public void subEnergy(float v) {
		addVariation(-v);
		//_energy -= v; if (_energy < 0) _energy = 0f; else if (_energy > _maxEnergy) _energy = _maxEnergy;
	}
  public void setMaxEnergy(float v) { _maxEnergy = v; if (_maxEnergy < 0f) _maxEnergy = 0f; }
  public float getMaxEnergy() { return _maxEnergy; }
  public void setEnergyProductionRate(float v) { _energyProductionRate = v;}
  public float getEnergyProductionRate() { return _energyProductionRate;}

	public float getEnergyVariation() { return _energyVariation;}

	public void addVariation(float variation)
	{
		_energyVariation += variation;
	}

	public void applyVariation()
	{
		_energy += _energyVariation;
		if(_energy <= 0f) _energy = 0f;
		else if(_energy >= _maxEnergy) _energy = _maxEnergy;

		ResetVariation();
				
	}
	
	// Reset the variation value : called at the end of the update
	public void ResetVariation()
	{
		_energyVariation = 0f;
	}

  public void enableEnergy(bool b)
  {
    _enableEnergy = b;
    foreach (IReaction r in _reactions)
      r.enableEnergy = b;
  }

  public void enableSequential(bool b)
  {
    _enableSequential = b;
    foreach (IReaction r in _reactions)
      r.enableSequential = b;
  }

  public void enableNoise(bool b)
  {
    _enableNoise = b;
  }

  /*!
    \brief Add a new reaction to the medium
    \param reaction The reaction to add.
   */
  public void addReaction(IReaction reaction)
  {
    Logger.Log("Medium::addReaction to medium#"+_id+" with "+reaction, Logger.Level.DEBUG);
    if (reaction != null)
    {
      reaction.setMedium(this);
      reaction.enableEnergy = _enableEnergy;
      _reactions.AddLast(reaction);
      //Logger.Log("Medium::addReaction _reactions="+Logger.ToString<IReaction>(_reactions), Logger.Level.DEBUG);
    }
    else
      Logger.Log("Medium::addReaction Cannot add this reaction because null was given", Logger.Level.WARN);
  }

  /* !
    \brief Remove the reaction that has the given name
    \param name The name of the reaction to remove.
   */
  public void removeReactionByName(string name)
  {
    LinkedListNode<IReaction> node = _reactions.First;
    bool b = true;

    while (node != null && b)
      {
        if (node.Value.getName() == name)
          {
            _reactions.Remove(node);
            b = false;
          }
        node = node.Next;
      }
  }

  /* !
    \brief Remove the reaction that has the same characteristics as the one given as parameter
    \param reaction The model of reaction to remove.
    \param checkNameAndMedium Whether the name  and medium must be taken into account or not.
   */
  public void removeReaction(IReaction reaction, bool checkNameAndMedium)
  {
    LinkedListNode<IReaction> node = _reactions.First;
    bool b = true;

    while (node != null && b)
    {
      if (node.Value.Equals(reaction, checkNameAndMedium))
      {
        _reactions.Remove(node);
        b = false;
      }
      node = node.Next;
    }
    if(b)
      Logger.Log("ReactionEngine::removeReaction failed to find matching reaction", Logger.Level.WARN);
    else
      Logger.Log("ReactionEngine::removeReaction successfully removed matching reaction", Logger.Level.DEBUG);
  }

  /*!
    \brief Substract a concentration to molecule corresponding to the name.
    \param name The name of the Molecules.
    \param value The value to substract.
   */
  public void subMolConcentration(string name, float value)
  {
    Molecule mol = ReactionEngine.getMoleculeFromName(name, _molecules);

    if (mol != null)
      mol.setConcentration(mol.getConcentration() - value);
  }

  /*!
    \brief Add a concentration to molecule corresponding to the name.
    \param name The name of the Molecules.
    \param value The value to Add.    
   */
  public void addMolConcentration(string name, float value)
  {
    Molecule mol = ReactionEngine.getMoleculeFromName(name, _molecules);

    if (mol != null)
      mol.setConcentration(mol.getConcentration() + value);
  }

  /*!
    \brief Load reactions from a ReactionSet
    \param reactionsSet The set to load
   */
  public void initReactionsFromReactionSet(ReactionSet reactionsSet)
  {
    if (reactionsSet == null)
      return;
    foreach (IReaction reaction in reactionsSet.reactions)
      addReaction(IReaction.copyReaction(reaction));
//       _reactions.AddLast(reaction);
  }

  /*!
    \brief Load degradation from each molecules
    \param allMolecules The list of all the molecules
   */
  public void initDegradationReactions(ArrayList allMolecules)
  {
    foreach (Molecule mol in allMolecules)
      addReaction(new Degradation(mol.getDegradationRate(), mol.getName()));
  }

  /*!
    \brief Load Molecules from a MoleculeSet
    \param molSet The set to Load
    \param allMolecules The list of all the molecules
   */
  public void initMoleculesFromMoleculeSets(MoleculeSet molSet, ArrayList allMolecules)
  {
	Logger.Log("Medium::initMoleculesFromMoleculeSets medium#"+_id,Logger.Level.TRACE);
    Molecule newMol;
    Molecule startingMolStatus;

    _molecules = new ArrayList();
    foreach (Molecule mol in allMolecules)
      {
        newMol = new Molecule(mol);
        startingMolStatus = ReactionEngine.getMoleculeFromName(mol.getName(), molSet.molecules);
        if (startingMolStatus == null) {
          newMol.setConcentration(0);
		} else {
          newMol.setConcentration(startingMolStatus.getConcentration());
		}
		Logger.Log("Medium::initMoleculesFromMoleculeSets medium#"+_id
				+" add mol "+newMol.getName()
				+" with cc="+newMol.getConcentration()
				,Logger.Level.TRACE
				);
        _molecules.Add(newMol);
      }   
  }

  /*!
    \brief This function initialize the production of ATP.
    \details It create a reaction of type ATPProducer
   */
  private void initATPProduction()
  {
    ATPProducer reaction = new ATPProducer();
    reaction.setProduction(_energyProductionRate);
    reaction.setName("ATP Production");
    if (_reactions == null)
      setReactions(new LinkedList<IReaction>());
    addReaction(reaction);
  }

  /*!
    \brief Initialize the Medium
    \param reactionsSets The list of all the reactions sets
    \param moleculesSets The list of all the molecules sets
   */
  public void Init(LinkedList<ReactionSet> reactionsSets, LinkedList<MoleculeSet> moleculesSets)
  {

		//Receive a linkedlist of Sets
    _reactions = new LinkedList<IReaction>();
    _numberGenerator = new NumberGenerator(NumberGenerator.normale, -10f, 10f, 0.01f);

		//Try to find the good set in the LinkedList
    ReactionSet reactSet = ReactionEngine.getReactionSetFromId(_reactionsSet, reactionsSets);
    MoleculeSet molSet = ReactionEngine.getMoleculeSetFromId(_moleculesSet, moleculesSets);

		//Put all the different molecules from the linkedList in an arrayList
    ArrayList allMolecules = ReactionEngine.getAllMoleculesFromMoleculeSets(moleculesSets);

    if (reactSet == null)
      Logger.Log("Medium::Init Cannot find group of reactions named " + _reactionsSet, Logger.Level.WARN);
    if (molSet == null)
      Logger.Log("Medium::Init Cannot find group of molecules named" + _moleculesSet, Logger.Level.WARN);

    initATPProduction();
    initReactionsFromReactionSet(reactSet);
    initMoleculesFromMoleculeSets(molSet, allMolecules);
    initDegradationReactions(allMolecules);
    foreach (IReaction r in _reactions)
      {
        r.enableSequential = _enableSequential;
      }
  }

  /*!
    \brief Set the concentration of each molecules of the medium to their new values
    \details Called only if sequential is disabled
   */
  public void updateMoleculesConcentrations()
  {
    foreach (Molecule m in _molecules)
      m.updateConcentration();
  }

  /*!
    \brief Debug the concentration of each molecules of the medium
   */
  public void Log(Logger.Level level = Logger.Level.TRACE)
  {
	string content = "";
    foreach (Molecule m in _molecules) {
	  if(!string.IsNullOrEmpty(content)) {
		content += ", ";
	  }
	  content += m.ToString();
	}
					
	Logger.Log("Medium::debug() #"+_id+"["+content+"]", level);
  }

  /*!
    \brief Execute everything about simulation into the Medium
   */
  public void Update()
  {
		
    if (enableShufflingReactionOrder)
      LinkedListExtensions.Shuffle<IReaction>(_reactions);

    foreach (IReaction reaction in _reactions) {
		
  	  if(Logger.isLevel(Logger.Level.TRACE)) {
  	    PromoterReaction promoter = reaction as PromoterReaction;
  	    if (promoter != null) {
	      Logger.Log("Medium::Update reaction.react("+_molecules+") with reaction="+reaction, Logger.Level.TRACE);
	    }
	  }
	  reaction.react(_molecules);
	}	
		
	applyVariation();
	
    if (_enableNoise)
    {
      float noise;
 
      foreach (Molecule m in _molecules)
      {
        noise = _numberGenerator.getNumber();
        if (_enableSequential)
          m.addConcentration(noise);
        else
          m.addNewConcentration(noise);
      }
    }

    //TODO improve check that it's the medium of the hero bacterium Cellia
    if (_name == "Cellia")
    {
      manageMoleculeConcentrationWithKey(KeyCode.P, KeyCode.M, "MOV");
      manageMoleculeConcentrationWithKey(KeyCode.O, KeyCode.L, "AMPI");
      manageMoleculeConcentrationWithKey(KeyCode.I, KeyCode.K, "FLUO1");
      manageMoleculeConcentrationWithKey(KeyCode.U, KeyCode.J, "AMPR");
      manageMoleculeConcentrationWithKey(KeyCode.Y, KeyCode.H, "FLUO2");
    }
  }

  private void manageMoleculeConcentrationWithKey(KeyCode plusCode, KeyCode minusCode, String molecule)
  {
    if (Input.GetKey(plusCode))
    {
      if (_enableSequential)
        ReactionEngine.getMoleculeFromName(molecule, _molecules).addConcentration(10f);
      else
        ReactionEngine.getMoleculeFromName(molecule, _molecules).addNewConcentration(100f);
    }
    if (Input.GetKey(minusCode))
    {
      if (_enableSequential)
        ReactionEngine.getMoleculeFromName(molecule, _molecules).addConcentration(- 10f);
      else
        ReactionEngine.getMoleculeFromName(molecule, _molecules).addNewConcentration(- 100f);
    }
  }

	public Medium initFromLoad(XmlNode node, MediumLoader loader)
	{
		return loader.loadMedium(node);
	}

}
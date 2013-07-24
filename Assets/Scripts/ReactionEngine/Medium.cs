using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class represent a Medium
  \details
  A Medium is an area closed by a something that is permeable or not.
  Each Medium contain a list of molecules wich contains the concentration of
  each kind of molecules.
  Each Medium also have a list of reactions.
  You can define molecules diffusion of molecules between mediums with Fick of ActiveTransport.
  \sa Fick
  \sa ActiveTransport
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
*/
public class Medium
{
  private LinkedList<IReaction> _reactions;             //!< The list of reaction
  private ArrayList             _molecules;             //!< The list of molecules (Molecule)

  private int           _id;                            //!< The id of the Medium
  private string        _name;                          //!< The name of the Medium
  private string        _reactionsSet;                  //!< The ReactionSet id affected to this Medium
  private string        _moleculesSet;                  //!< The MoleculeSet id affected to this Medium
  private bool          _enableSequential;
  private bool          _enableNoise;
  private bool          _enableEnergy;
  private float         _energy;                        //!< Represent the quantity of ATP
  private float         _maxEnergy;                     //!< The maximum quantity of ATP
  private float         _energyProductionRate;          //!< The energy production speed
  public bool           enableShufflingReactionOrder;   //!< Enable shuffling of reactions

  public void setId(int id) { _id = id;}
  public int getId() { return _id;}
  public void setName(string name) { _name = name;}
  public string getName() { return _name;}
  public void setReactions(LinkedList<IReaction> RL) { _reactions = RL;}
  public LinkedList<IReaction> getReactions() { return _reactions;}
  public void setReactionsSet(string reactionsSet) { _reactionsSet = reactionsSet;}
  public string getReactionsSet() { return _reactionsSet;}
  public void setMoleculesSet(string moleculesSet) { _moleculesSet = moleculesSet;}
  public string getMoleculesSet() { return _moleculesSet;}
  public ArrayList getMolecules() { return _molecules; }
  public void setEnergy(float v) { _energy = v; if (_energy < 0f) _energy = 0f; }
  public float getEnergy() { return _energy; }
  public void addEnergy(float v) { _energy += v; if (_energy < 0) _energy = 0f; else if (_energy > _maxEnergy) _energy = _maxEnergy;}
  public void subEnergy(float v) { _energy -= v; if (_energy < 0) _energy = 0f; else if (_energy > _maxEnergy) _energy = _maxEnergy;}
  public void setMaxEnergy(float v) { _maxEnergy = v; if (_maxEnergy < 0f) _maxEnergy = 0f; }
  public float getMaxEnergy() { return _maxEnergy; }
  public void setEnergyProductionRate(float v) { _energyProductionRate = v;}
  public float getEnergyProductionRate() { return _energyProductionRate;}

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
    foreach (IReaction r in _reactions)
      r.enableNoise = b;
  }

  /*!
    \brief Add a new reaction to the medium
    \param reaction The reaction to add.
   */
  public void addReaction(IReaction reaction)
  {
    if (reaction != null)
      {
        reaction.setMedium(this);
        reaction.enableEnergy = _enableEnergy;
        _reactions.AddLast(reaction);
      }
    else
      Debug.Log("Cannot add this reaction because null was given");
  }

  /*!
    \brief Remove the reaction that correspond to the given name
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
    \brief Load reactions from a ReactionsSet
    \param reactionsSet The set to load
   */
  public void initReactionsFromReactionsSet(ReactionsSet reactionsSet)
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
    \brief Load Molecules from a MoleculesSet
    \param molSet The set to Load
    \param allMolecules The list of all the molecules
   */
  public void initMoleculesFromMoleculesSets(MoleculesSet molSet, ArrayList allMolecules)
  {
    Molecule newMol;
    Molecule startingMolStatus;

    _molecules = new ArrayList();
    foreach (Molecule mol in allMolecules)
      {
        newMol = new Molecule(mol);
        startingMolStatus = ReactionEngine.getMoleculeFromName(mol.getName(), molSet.molecules);
        if (startingMolStatus == null)
          newMol.setConcentration(0);
        else
          newMol.setConcentration(startingMolStatus.getConcentration());
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
  public void Init(LinkedList<ReactionsSet> reactionsSets, LinkedList<MoleculesSet> moleculesSets)
  {
    _reactions = new LinkedList<IReaction>();
    ReactionsSet reactSet = ReactionEngine.getReactionsSetFromId(_reactionsSet, reactionsSets);
    MoleculesSet molSet = ReactionEngine.getMoleculesSetFromId(_moleculesSet, moleculesSets);
    ArrayList allMolecules = ReactionEngine.getAllMoleculesFromMoleculeSets(moleculesSets);

    if (reactSet == null)
      Debug.Log("Cannot find group of reactions named " + _reactionsSet);
    if (molSet == null)
      Debug.Log("Cannot find group of molecules named" + _moleculesSet);

    initATPProduction();
    initReactionsFromReactionsSet(reactSet);
    initMoleculesFromMoleculesSets(molSet, allMolecules);
    initDegradationReactions(allMolecules);
    foreach (IReaction r in _reactions)
      {
        r.enableSequential = _enableSequential;
        r.enableNoise = _enableNoise;
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
    \brief Execute everything about simulation into the Medium
   */
  public void Update()
  {
    if (enableShufflingReactionOrder)
      LinkedListExtensions.Shuffle<IReaction>(_reactions);

    foreach (IReaction reaction in _reactions)
        reaction.react(_molecules);

    //#FIXME : Delete theses this a the end
    if (_name == "Cellia")
      {
        if (Input.GetKey(KeyCode.T))
          {
            if (_enableSequential)
              ReactionEngine.getMoleculeFromName("H", _molecules).addConcentration(10f);
            else
              ReactionEngine.getMoleculeFromName("H", _molecules).addNewConcentration(10f);
          }
        if (Input.GetKey(KeyCode.R))
          {
            if (_enableSequential)
              ReactionEngine.getMoleculeFromName("H", _molecules).addConcentration(- 10f);
            else
              ReactionEngine.getMoleculeFromName("H", _molecules).addNewConcentration(- 10f);
          }
        if (Input.GetKey(KeyCode.G))
          {
            if (_enableSequential)
              ReactionEngine.getMoleculeFromName("O", _molecules).addConcentration(10f);
            else
              ReactionEngine.getMoleculeFromName("O", _molecules).addNewConcentration(100f);
//             Debug.Log("O: " + ReactionEngine.getMoleculeFromName("O", _molecules).getConcentration());
          }
        if (Input.GetKey(KeyCode.F))
          {
            if (_enableSequential)
              ReactionEngine.getMoleculeFromName("O", _molecules).addConcentration(- 10f);
            else
              ReactionEngine.getMoleculeFromName("O", _molecules).addNewConcentration(- 100f);
          }
      }
  }
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class represents a Medium
  \details
  A Medium is an area closed by a something that is permeable or not.
  Each Medium contains a list of molecules which contains the concentration of
  each kind of molecules.
  Each Medium also has a list of reactions.
  You can define molecule diffusion between mediums with Fick or ActiveTransport.
  \sa Fick
  \sa ActiveTransport
  
  
*/
using System.Xml;


public class Medium : LoadableFromXmlImpl
{
    private LinkedList<Reaction> _reactions;               //!< The list of reactions
	private Dictionary<string, Molecule> _molecules;               //!< The list of molecules (Molecule)

    private int _numberId;                            //!< The id of the Medium
    private string _name;                          //!< The name of the Medium
    private string _reactionsSet;                  //!< The ReactionSet id assigned to this Medium
    private string _moleculesSet;                  //!< The MoleculeSet id assigned to this Medium
    private bool _enableSequential;
    private bool _enableNoise;
    private NumberGenerator _numberGenerator;               //!< Random number generator
    private bool _enableEnergy;
    private float _energy;                        //!< Represents the quantity of ATP
    private float _energyVariation;                     //!< The variation of energy during one frame
    private const float _energyVariationFactor = 0.3f;
    private float _maxEnergy;                     //!< The maximum quantity of ATP
    private float _energyProductionRate;          //!< The energy production speed
    public bool enableShufflingReactionOrder;   //!< Enables shuffling of reactions

    //TODO refactor interactions out of medium
    private const string _shortkeyPlusSuffix = ".PLUS";
    private const string _shortkeyMinusSuffix = ".MINUS";
    private const string _characterMediumName = "Cellia";
    private const string _mediumTag = "Medium";

    public void setId(int id) { _numberId = id; }
    public int getId() { return _numberId; }
    public void setName(string name) { _name = name; }
    public string getName() { return _name; }
    public void setReactions(LinkedList<Reaction> RL) { _reactions = RL; }
    public LinkedList<Reaction> getReactions() { return _reactions; }
    public void setReactionSet(string reactionsSet) { _reactionsSet = reactionsSet; }
    public string getReactionSet() { return _reactionsSet; }
    public void setMoleculeSet(string moleculesSet) { _moleculesSet = moleculesSet; }
    public string getMoleculeSet() { return _moleculesSet; }
	public Dictionary<string, Molecule> getMolecules() { return _molecules; }

    //TODO extract energy methods and fields and make class out of it
    public void setEnergy(float v) { _energy = Mathf.Min(v, _maxEnergy); if (_energy < 0f) _energy = 0f; }
    public float getEnergy() { return _energy; }
    public void addEnergy(float v)
    {
        addVariation(v);
        //_energy += v; if (_energy < 0) _energy = 0f; else if (_energy > _maxEnergy) _energy = _maxEnergy;
    }
    public void subEnergy(float v)
    {
        addVariation(-v);
        //_energy -= v; if (_energy < 0) _energy = 0f; else if (_energy > _maxEnergy) _energy = _maxEnergy;
    }
    public void setMaxEnergy(float v) { _maxEnergy = v; if (_maxEnergy < 0f) _maxEnergy = 0f; }
    public float getMaxEnergy() { return _maxEnergy; }
    public void setEnergyProductionRate(float v) { _energyProductionRate = v; }
    public float getEnergyProductionRate() { return _energyProductionRate; }

    public float getEnergyVariation() { return _energyVariation; }

    public void addVariation(float variation)
    {
        _energyVariation += variation;
    }

    public void applyVariation()
    {
        _energy += _energyVariation * _energyVariationFactor;
        if (_energy <= 0f) _energy = 0f;
        else if (_energy >= _maxEnergy) _energy = _maxEnergy;

        ResetVariation();

    }

    public void resetMolecules()
    {
		foreach (Molecule m in _molecules.Values)
        {
            m.setConcentration(0);
        }
    }

    // Reset the variation value : called at the end of the update
    public void ResetVariation()
    {
        _energyVariation = 0f;
    }

    public void enableEnergy(bool b)
    {
        _enableEnergy = b;
        foreach (Reaction r in _reactions)
            r.enableEnergy = b;
    }

    public void enableSequential(bool b)
    {
        _enableSequential = b;
        foreach (Reaction r in _reactions)
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
    public void addReaction(Reaction reaction)
    {
        // Debug.Log(this.GetType() + " addReaction to medium#"+_numberId+" with "+reaction);
        if (reaction != null)
        {
            reaction.setMedium(this);
            reaction.enableEnergy = _enableEnergy;
            _reactions.AddLast(reaction);
            // Debug.Log(this.GetType() + " addReaction _reactions="+Logger.ToString<Reaction>(_reactions));
        }
        else
        {
            Debug.LogWarning(this.GetType() + " addReaction Cannot add this reaction because null was given");
        }

    }

    /* !
      \brief Remove the reaction that has the given name
      \param name The name of the reaction to remove.
     */
    public void removeReactionByName(string name)
    {
        LinkedListNode<Reaction> node = _reactions.First;
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
    public void removeReaction(Reaction reaction, bool checkNameAndMedium)
    {
        LinkedListNode<Reaction> node = _reactions.First;
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
        if (b)
        {
            Debug.LogWarning(this.GetType() + " removeReaction failed to find matching reaction");
        }
        else
        {
            // Debug.Log(this.GetType() + " removeReaction successfully removed matching reaction");
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
      \brief Load reactions from a ReactionSet
      \param reactionsSet The set to load
     */
    public void initReactionsFromReactionSet(ReactionSet reactionsSet)
    {
        if (reactionsSet == null)
            return;
        foreach (Reaction reaction in reactionsSet.reactions)
            addReaction(Reaction.copyReaction(reaction));
        //       _reactions.AddLast(reaction);
    }

    /*!
      \brief Load degradation from each molecules
      \param allMolecules The list of all the molecules
     */
    public void initDegradationReactions(Dictionary<string, Molecule> allMolecules)
    {
        foreach (Molecule mol in allMolecules.Values)
            addReaction(new Degradation(mol.getDegradationRate(), mol.getName()));
    }

    /*!
      \brief Load Molecules from a MoleculeSet
      \param molSet The set to Load
      \param allMolecules The list of all the molecules
     */
	public void initMoleculesFromMoleculeSets(MoleculeSet molSet, Dictionary<string, Molecule> allMolecules)
    {
        // Debug.Log(this.GetType() + " initMoleculesFromMoleculeSets medium#" + _numberId);
        Molecule newMol;
        Molecule startingMolStatus;

		_molecules = new Dictionary<string, Molecule>();
        foreach (Molecule mol in allMolecules.Values)
        {
            newMol = new Molecule(mol);
            startingMolStatus = ReactionEngine.getMoleculeFromName(mol.getName(), molSet.molecules);
            if (startingMolStatus == null)
            {
                newMol.setConcentration(0);
            }
            else
            {
                newMol.setConcentration(startingMolStatus.getConcentration());
            }
            // Debug.Log(this.GetType() + " initMoleculesFromMoleculeSets medium#" + _numberId
            // + " add mol " + newMol.getName()
            // + " with cc=" + newMol.getConcentration()
            // 
            // );
			_molecules.Add(newMol.getName(), newMol);
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
            setReactions(new LinkedList<Reaction>());
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
        _reactions = new LinkedList<Reaction>();
        _numberGenerator = new NumberGenerator(NumberGenerator.normale, -10f, 10f, 0.01f);

        //Try to find the good set in the LinkedList
        ReactionSet reactSet = ReactionEngine.getReactionSetFromId(_reactionsSet, reactionsSets);
        MoleculeSet molSet = ReactionEngine.getMoleculeSetFromId(_moleculesSet, moleculesSets);

        //Put all the different molecules from the linkedList in an arrayList
        var allMolecules = ReactionEngine.getAllMoleculesFromMoleculeSets(moleculesSets);

        if (reactSet == null)
            Debug.LogWarning(this.GetType() + " Init Cannot find group of reactions named " + _reactionsSet);
        if (molSet == null)
            Debug.LogWarning(this.GetType() + " Init Cannot find group of molecules named" + _moleculesSet);

        initATPProduction();
        initReactionsFromReactionSet(reactSet);
        initMoleculesFromMoleculeSets(molSet, allMolecules);
        initDegradationReactions(allMolecules);
        foreach (Reaction r in _reactions)
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
        foreach (Molecule m in _molecules.Values)
            m.updateConcentration();
    }

    /*!
      \brief Debug the concentration of each molecules of the medium
     */
    public void Log()
    {
        string content = "";
        foreach (Molecule m in _molecules.Values)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content += ", ";
            }
            content += m.ToString();
        }

        // Debug.Log(this.GetType() + " debug() #" + _numberId + "[" + content + "]");
    }

    /*!
      \brief Execute everything about simulation into the Medium
     */
    public void Update()
    {

        if (enableShufflingReactionOrder)
            LinkedListExtensions.Shuffle<Reaction>(_reactions);

        foreach (Reaction reaction in _reactions)
        {
            // PromoterReaction promoter = reaction as PromoterReaction;
            // if (promoter != null) {
            //   Debug.Log(this.GetType() + " Update reaction.react("+_molecules+") with reaction="+reaction);
            // }
            reaction.react(_molecules);
        }

        applyVariation();

        if (_enableNoise)
        {
            float noise;

            foreach (Molecule m in _molecules.Values)
            {
                noise = _numberGenerator.getNumber();
                if (_enableSequential)
                    m.addConcentration(noise);
                else
                    m.addNewConcentration(noise);
            }
        }

        //TODO improve check that it's the medium of the character
        //TODO refactor interactions out of medium
        if (_name == _characterMediumName)
        {
            if (GameConfiguration.isAdmin)
            {
                //TODO optimize
                manageMoleculeConcentrationWithKey("AMPI");
                manageMoleculeConcentrationWithKey("AMPR");
                manageMoleculeConcentrationWithKey("ATC");
                manageMoleculeConcentrationWithKey("FLUO1");
                manageMoleculeConcentrationWithKey("FLUO2");
                manageMoleculeConcentrationWithKey("IPTG");
                manageMoleculeConcentrationWithKey("MOV");
                manageMoleculeConcentrationWithKey("REPR1");
                manageMoleculeConcentrationWithKey("REPR2");
                manageMoleculeConcentrationWithKey("REPR3");
                // manageMoleculeConcentrationWithKey("REPR4");
            }
        }
    }

    private class MoleculeShortcut
    {
        public string shortCutPlus;
        public string shortCutMinus;
        public Molecule molecule;

        public MoleculeShortcut(string sCPlus, string sCMinus, Molecule mol)
        {
            this.shortCutPlus = sCPlus;
            this.shortCutMinus = sCMinus;
            this.molecule = mol;
        }
    }
    private Dictionary<string, MoleculeShortcut> _shortcuts = new Dictionary<string, MoleculeShortcut>();
    private MoleculeShortcut _shortCut;
    private Molecule _molecule;

    //TODO refactor interactions out of medium
    private void manageMoleculeConcentrationWithKey(String molecule)
    {
        _shortCut = null;
        _shortcuts.TryGetValue(molecule, out _shortCut);
        if (null == _shortCut)
        {
            _molecule = ReactionEngine.getMoleculeFromName(molecule, _molecules);
            if (null != _molecule)
            {
                _shortCut = new MoleculeShortcut(
                    GameStateController.keyPrefix + molecule + _shortkeyPlusSuffix,
                    GameStateController.keyPrefix + molecule + _shortkeyMinusSuffix,
                    _molecule
                );
                _shortcuts.Add(molecule, _shortCut);
            }
            else
            {
                Debug.LogWarning(this.GetType() + " molecule " + molecule + " not found");
                return;
            }
        }

        if (GameStateController.isShortcutKey(_shortCut.shortCutPlus))
        {
            if (_enableSequential)
                _shortCut.molecule.addConcentration(10f);
            else
                _shortCut.molecule.addNewConcentration(100f);
        }
        if (GameStateController.isShortcutKey(_shortCut.shortCutMinus))
        {
            if (_enableSequential)
                _shortCut.molecule.addConcentration(-10f);
            else
                _shortCut.molecule.addNewConcentration(-100f);
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// loading methods /////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /*! 
     *  \brief     Load medium files
     *  \details   This class loads everything about mediums from medium files
     A medium file should respect this syntax:

            <Mediums>
              <Medium type="Cellia">
                <Id>01</Id>                                         -> Unique ID of the medium
                <Name>Cellia</Name>                                 -> Name of the medium
                <ReactionSet>CelliaReactions</ReactionSet>        -> ReactionSet to load in the medium
                <MoleculeSet>CelliaMolecules</MoleculeSet>        -> MoleculeSet to load in the medium
                <Energy>1000</Energy>                               -> Initial Energy
                <MaxEnergy>2000</MaxEnergy>                         -> Maximal energy
                <EnergyProductionRate>10</EnergyProductionRate>     -> The energy production speed
              </Medium>
            </Mediums>

     *  
     *  \sa ReactionSet
     *  \sa MoleculeSet
     *  \sa Medium
     */

    public override string getTag() { return _mediumTag; }

    /*!
    \brief This function load the initial energy of the medium and parse the validity of the given string
    \param value The value to parse and load
    \param med The medium to initialize
    \return Return true if the function succeeded to parse the string or false else
   */
    private bool loadEnergy(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            // Debug.Log(this.GetType() + " Error: Empty Energy field. default value = 0");
            setEnergy(0f);
        }
        else
            setEnergy(parseFloat(value));

        return true;
    }

    /*!
    \brief This function load the energy production rate of the medium and parse the validity of the given string
    \param value The value to parse and load
    \param med The medium to initialize
    \return Return true if the function succeeded to parse the string or false else
   */
    private bool loadEnergyProductionRate(string value)
    {
        float productionRate;

        if (String.IsNullOrEmpty(value))
        {
            // Debug.Log(this.GetType() + " Error: Empty EnergyProductionRate field. default value = 0");
            productionRate = 0f;
        }
        else
            productionRate = parseFloat(value);
        setEnergyProductionRate(productionRate);

        return true;
    }

    /*!
    \brief This function load the maximum energy in the medium and parse the validity of the given string
    \param value The value to parse and load
    \param med The medium to initialize
    \return Return true if the function succeeded to parse the string or false else
   */
    private bool loadMaxEnergy(string value)
    {
        float prodMax;

        if (String.IsNullOrEmpty(value))
        {
            // Debug.Log(this.GetType() + " Error: Empty EnergyProductionRate field. default value = 0");
            prodMax = 0f;
        }
        else
            prodMax = parseFloat(value);
        setMaxEnergy(prodMax);

        return true;
    }

    /*!
    \brief This function create a new Medium based on the information in the given XML Node
    \param node The XmlNode to load.
  */
    public override bool tryInstantiateFromXml(XmlNode node)
    {
        // Debug.Log(this.GetType() + " tryInstantiateFromXml(" + Logger.ToString(node) + ")");

        foreach (XmlNode attr in node)
        {
            if (null == attr)
            {
                continue;
            }

            switch (attr.Name)
            {
                case "Id":
                    setId(Convert.ToInt32(attr.InnerText));
                    break;
                case "Name":
                    setName(attr.InnerText);
                    break;
                case "Energy":
                    loadEnergy(attr.InnerText);
                    break;
                case "EnergyProductionRate":
                    loadEnergyProductionRate(attr.InnerText);
                    break;
                case "MaxEnergy":
                    loadMaxEnergy(attr.InnerText);
                    break;
                case "ReactionSet":
                    setReactionSet(attr.InnerText);
                    break;
                case "MoleculeSet":
                    setMoleculeSet(attr.InnerText);
                    break;
            }
        }

        if (
               //_reactions;               //!< The list of reactions
               //_molecules;               //!< The list of molecules (Molecule)

               (0 == _numberId)
            || (string.IsNullOrEmpty(_name))            //!< The name of the Medium
            || string.IsNullOrEmpty(_reactionsSet)      //!< The ReactionSet id assigned to this Medium
            || string.IsNullOrEmpty(_moleculesSet)      //!< The MoleculeSet id assigned to this Medium
                                                        //_enableSequential;
                                                        //_enableNoise;
                                                        //_numberGenerator                          //!< Random number generator (initialized in Init)
                                                        //_enableEnergy;
                                                        //_energy;                                  //!< Represents the quantity of ATP
                                                        //_energyVariation;                         //!< The variation of energy during one frame
                                                        //_maxEnergy;                               //!< The maximum quantity of ATP
                                                        //_energyProductionRate;                    //!< The energy production speed

            )
        {
            Debug.LogError("Medium.tryInstantiateFromXml failed to load because "
                         + "_numberId=" + _numberId
                         + "& _name=" + _name
                         + "& _reactionsSet=" + _reactionsSet
                         + "& _moleculesSet=" + _moleculesSet
                         );
            return false;
        }
        else
        {
            // Debug.Log(this.GetType() + " tryInstantiateFromXml(node) loaded this=" + this);
            return true;
        }
    }




    public override string ToString()
    {

        string moleculeString = null == _molecules ? "" : _molecules.Count.ToString();
        string reactionString = null == _reactions ? "" : _reactions.Count.ToString();

        return "[Medium "
                + "name:" + _name
                + "; id:" + _numberId
                + "; molecules:" + moleculeString
                + "; reactions:" + reactionString
                + "]";
    }

    public string ToStringDetailed()
    {

        string moleculeString = null == _molecules ? "" : Logger.ToString<Molecule>("Molecule", _molecules);
        string reactionString = null == _reactions ? "" : Logger.ToString<Reaction>(_reactions);

        return "[Medium "
                + "name:" + _name
                + "; id:" + _numberId
                + "; molecules:" + moleculeString
                + "; reactions:" + reactionString
                + "]";
    }
}
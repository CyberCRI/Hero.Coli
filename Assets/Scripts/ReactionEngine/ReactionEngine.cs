using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;

//!
/*!
 *  \brief     The main class that computes all the reactions.
 *  \details     This class initializes from files and execute all the reactions.
 The reactions that are currently implemented are :

 - Degradation
 - Diffusion (Fick)
 - Enzyme reaction with effectors (EnzymeReaction)
 - Promoter expressions (Promoter)

   
   
 */
public class ReactionEngine : MonoBehaviour {

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "ReactionEngine";
    private static ReactionEngine _instance;
    public static ReactionEngine get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("ReactionEngine::get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<ReactionEngine>();
        }
        return _instance;
    }

    void Awake()
    {
        Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError(this.GetType() + " has two running instances");
        }
        _instance = this;
        initializeIfNecessary();
    }

    void OnDestroy()
    {
        Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            FileLoader fileLoader = new FileLoader();
            _reactionsSets = new LinkedList<ReactionSet>();
            _moleculesSets = new LinkedList<MoleculeSet>();
            _mediums = new LinkedList<Medium>();

            //TODO there is only one file in _moleculesFiles and in _reactionsFiles
            foreach (string file in _reactionsFiles)
            {
                LinkedList<ReactionSet> lr = fileLoader.loadObjectsFromFile<ReactionSet>(file, "reactions");
                if (null != lr)
                    LinkedListExtensions.AppendRange<ReactionSet>(_reactionsSets, lr);
            }
            foreach (string file in _moleculesFiles)
            {
                // Debug.Log("ReactionEngine::Awake() loading molecules from file");

                LinkedList<MoleculeSet> lm = fileLoader.loadObjectsFromFile<MoleculeSet>(file, "molecules");
                if (null != lm)
                    LinkedListExtensions.AppendRange<MoleculeSet>(_moleculesSets, lm);

                // Debug.Log("ReactionEngine::Awake() loading molecules from file done"
                //            + ": _moleculesSets=" + Logger.ToString<MoleculeSet>(_moleculesSets));
            }

            foreach (string file in _mediumsFiles)
            {
                LinkedList<Medium> lmed = fileLoader.loadObjectsFromFile<Medium>(file, "Medium");
                if (null != lmed)
                    LinkedListExtensions.AppendRange<Medium>(_mediums, lmed);
            }

            foreach (Medium medium in _mediums)
            {
                medium.Init(_reactionsSets, _moleculesSets);
                medium.enableSequential(enableSequential);
                medium.enableNoise(enableNoise);
                medium.enableEnergy(enableEnergy);
                medium.enableShufflingReactionOrder = enableShufflingReactionOrder;
            }

            // Debug.Log("ReactionEngine::Awake() FickReactions starting");

            _fick = new Fick();
            _fick.loadFicksReactionsFromFiles(_fickFiles, _mediums);

            // Debug.Log("ReactionEngine::Awake() activeTransport starting");

            _activeTransport = new ActiveTransport();
            _activeTransport.loadActiveTransportReactionsFromFiles(_activeTransportFiles, _mediums);

            // Debug.Log("ReactionEngine::Awake() done");
            _initialized = true;
        }
    }

    void Start()
    {
        Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private Fick _fick;                                   //!< The Fick class that manages molecules diffusions between medium
  private ActiveTransport       _activeTransport;       //!< The class that manages Active transport reactions.
  private LinkedList<Medium>    _mediums;               //!< The list that contains all the mediums
  private LinkedList<ReactionSet> _reactionsSets;      //!< The list that contains the reactions sets
  private LinkedList<MoleculeSet> _moleculesSets;      //!< The list that contains the molecules sets
  public string[]      _mediumsFiles;                   //!< all the medium files
  public string[]      _reactionsFiles;                 //!< all the reaction files
  public string[]      _moleculesFiles;                 //!< all the molecule files
  public string[]      _fickFiles;                      //!< all the Fick diffusion files
  public string[]      _activeTransportFiles;           //!< all the Fick diffusion files
  public static float  reactionsSpeed = 0.9f;           //!< Global reaction speed
  public bool enableSequential;                         //!< Enable sequential mode (if reactions are computed one after the other)
  public bool enableNoise;                              //!< Add Noise in each Reaction
  public bool enableEnergy;                             //!< Enable energy consumption
  public bool enableShufflingReactionOrder;             //!< Randomize reaction computation order in mediums
  public bool enableShufflingMediumOrder;               //!< Randomize medium computation order
	
  private bool _paused;                                 //!< Simulation state

  public Fick getFick() { return _fick; }
  
  /*!
    \brief Adds an IReaction to a medium
    \param mediumId The medium ID.
    \param reaction The reaction to add.
   */
  public void addReactionToMedium(int mediumId, IReaction reaction)
  {
	// Debug.Log("ReactionEngine::addReactionToMedium("+mediumId+", "+reaction+")");
    Medium med = ReactionEngine.getMediumFromId(mediumId, _mediums);

    if (med == null) {
	  Debug.LogWarning("ReactionEngine::addReactionToMedium medium #"+mediumId+"not found");
      return ;
	}
	
	/*TODO FIXME USEFULNESS?/////////////////////////////////////////////////////////////////////
	ReactionSet reactionsSet = null;
	string medName = med.getName()+"Reactions";
	foreach (ReactionSet rs in _reactionsSets) {
	  if (rs.id == medName) reactionsSet = rs;
	}
	if (reactionsSet != null) {
	  reactionsSet.reactions.AddLast(IReaction.copyReaction(reaction));
	} else {
	  Debug.LogWarning("ReactionEngine::addReactionToMedium reactionsSet == null");
	}
	//////////////////////////////////////////////////////////////////////////////////////////*/
		
    med.addReaction(IReaction.copyReaction(reaction));
  }

  /*!
    \brief remove a reaction from a medium
    \param mediumId The medium ID.
    \param name The reaction's name.
   */  
  public void removeReactionFromMediumByName(int mediumId, string name)
  {
    Medium med = ReactionEngine.getMediumFromId(mediumId, _mediums);

    if (med == null)
      return ;
    med.removeReactionByName(name);    
  }

  /* !
    \brief Remove from the specified medium the reaction that has the same characteristics as the one given as parameter
    \param mediumId The Id of the medium to remove from.
    \param reaction The model of reaction to remove.
    \param checkNameAndMedium Whether the name and medium must be taken into account or not.
   */
  public void removeReaction(int mediumId, IReaction reaction, bool checkNameAndMedium = false)
  {
    Medium med = ReactionEngine.getMediumFromId(mediumId, _mediums);

    if (med == null)
    {
      Debug.LogWarning("ReactionEngine::removeReaction could not find medium with id "+mediumId);
      return ;
    }

    med.removeReaction(reaction, checkNameAndMedium);
  }

  //! Return the Medium reference corresponding to the given id
  /*!
      \param id The id of searched Medium.
      \param list The Medium list where to search in.
  */
  public static Medium        getMediumFromId(int id, LinkedList<Medium> list)
  {
    foreach (Medium med in list)
      if (med.getId() == id)
        return med;
    return null;
  }
  
//! Return the Molecule reference corresponding to the given name
  /*!
      \param name The name of the molecule
      \param molecules The molecule list where to search in.
  */
  public static Molecule        getMoleculeFromName(string name, ArrayList molecules)
  {
    if(null != molecules)
    {
      foreach (Molecule mol in molecules)
        if (mol.getName() == name)
          return mol;
    }
    return null;
  }

//! Return the ReactionSet reference corresponding to the given id
  /*!
      \param id The id of the ReactionSet
      \param list The list of ReactionSet where to search in
  */
  public static ReactionSet    getReactionSetFromId(string id, LinkedList<ReactionSet> list)
  {
    foreach (ReactionSet reactSet in list)
      if (reactSet.getStringId() == id)
        return reactSet;
    return null;
  }

//! Tell if a Molecule is already present in a Molecule list (based on the name attribute)
  /*!
      \param mol Molecule to match.
      \param list Molecule list where to search in.
  */
  public static bool    isMoleculeDuplicated(Molecule mol, ArrayList list)
  {
    foreach (Molecule mol2 in list)
      if (mol2.getName() == mol.getName())
        return true;
    return false;
  }

  /*!
    \brief Return the List of mediums.
    \return Return the List of mediums.
  */
  public LinkedList<Medium>    getMediumList()
  {
    return _mediums;
  }

  //! Return an ArrayList that contains all the differents molecules from a list of MoleculeSet
  /*!
      \param list the list of MoleculeSet
  */
  public static ArrayList    getAllMoleculesFromMoleculeSets(LinkedList<MoleculeSet> list)
  {

    ArrayList molecules = new ArrayList();

    
    foreach (MoleculeSet molSet in list)
      {
        foreach (Molecule mol in molSet.molecules)
          if (!isMoleculeDuplicated(mol, molecules))
            molecules.Add(mol);
      }
    return molecules;
  }

  //! Return the MoleculeSet of a list of MoleculeSet corresponding to an id
  /*!
    \param id The id of the MoleculeSet
    \param list The list of MoleculeSet
  */
  public static MoleculeSet    getMoleculeSetFromId(string id, LinkedList<MoleculeSet> list)
  {
    foreach (MoleculeSet molSet in list)
      if (molSet.getStringId() == id)
        return molSet;
    return null;
  }
	
    public ArrayList getMoleculesFromMedium(int id) {
        //"warn" parameter is true to indicate that there is no such Medium
        //as the one needed to get molecules
    Medium medium = LinkedListExtensions.Find<Medium>(
            _mediums
            , m => m.getId() == id
            , true
            , " RE::getMoleculesFromMedium("+id+")");
		if (medium != null) {
	  return medium.getMolecules();
	} else {
	  return null;
	}
  }
	
  //TODO manage reaction speed for smooth pausing
  public void Pause(bool pause) {
    _paused = pause;
  }

  public static bool isPaused()
  {
    return _instance._paused;
  }

  //! This function is called at each frame
  public void Update()
  {		
	  if(_paused) {
	    // Debug.Log("ReactionEngine::Update paused");
	  } else {
	    _fick.react();
      if (enableShufflingMediumOrder)
        LinkedListExtensions.Shuffle<Medium>(_mediums);

      foreach (Medium medium in _mediums)
        medium.Update();

	    // Debug.Log("ReactionEngine::Update() update of mediums done");
      if (!enableSequential) {
        foreach (Medium medium in _mediums)
          medium.updateMoleculesConcentrations();
		    // Debug.Log("ReactionEngine::Update() update of mol cc in mediums done");
      }
	  }
  }
}

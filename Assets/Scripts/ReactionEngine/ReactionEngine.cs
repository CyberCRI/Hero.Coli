using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;

//!
/*!
 *  \brief     The main class that compute all the reactions.
 *  \details     This class initialize from files and execute all the reactions.
 The reactions that are currently implemented are :

 - Degradation
 - Diffusion (Fick)
 - Enzyme reaction with effectors (EnzymeReaction)
 - Promoter expressions (Promoter)

   
   
 */
public class ReactionEngine : MonoBehaviour {

  // singleton fields & methods
  public static string gameObjectName = "ReactionEngine";
  private static ReactionEngine _instance;
  public static ReactionEngine get() {
    if(_instance == null) {
      Logger.Log("ReactionEngine::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<ReactionEngine>();
    }
    return _instance;
  }

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
	Logger.Log("ReactionEngine::addReactionToMedium("+mediumId+", "+reaction+")", Logger.Level.INFO);
    Medium med = ReactionEngine.getMediumFromId(mediumId, _mediums);

    if (med == null) {
	  Logger.Log("ReactionEngine::addReactionToMedium medium #"+mediumId+"not found", Logger.Level.WARN);
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
	  Logger.Log("ReactionEngine::addReactionToMedium reactionsSet == null", Logger.Level.WARN);
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
      Logger.Log("ReactionEngine::removeReaction could not find medium with id "+mediumId, Logger.Level.WARN);
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
  public static bool    isMoleculeIsDuplicated(Molecule mol, ArrayList list)
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

  //! Return an ArrayList that contain all the differents molecules an list of MoleculeSet
  /*!
      \param list the list of MoleculeSet
  */
  public static ArrayList    getAllMoleculesFromMoleculeSets(LinkedList<MoleculeSet> list)
  {

    ArrayList molecules = new ArrayList();

    
    foreach (MoleculeSet molSet in list)
      {
        foreach (Molecule mol in molSet.molecules)
          if (!isMoleculeIsDuplicated(mol, molecules))
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
    Medium medium = LinkedListExtensions.Find<Medium>(_mediums, m => m.getId() == id);
		if (medium != null) {
	  return medium.getMolecules();
	} else {
	  return null;
	}
  }

  //! This function is called at the initialisation of the simulation (like a Constructor)
  void Awake()
  {
    _instance = this;

    FileLoader fileLoader = new FileLoader();
    _reactionsSets = new LinkedList<ReactionSet>();
    _moleculesSets = new LinkedList<MoleculeSet>();
    _mediums = new LinkedList<Medium>();
    

		//TODO there is only one file in _moleculesFiles and in _reactionsFiles
    foreach (string file in _reactionsFiles)
		{
			LinkedList<ReactionSet> lr = fileLoader.loadObjectsFromFile<ReactionSet>(file,"reactions");
      LinkedListExtensions.AppendRange<ReactionSet>(_reactionsSets, lr);
		}
    foreach (string file in _moleculesFiles)
		{
      Logger.Log("ReactionEngine::Awake() loading molecules from file", Logger.Level.DEBUG);

			LinkedList<MoleculeSet> lm = fileLoader.loadObjectsFromFile<MoleculeSet>(file,"molecules");
			LinkedListExtensions.AppendRange<MoleculeSet>(_moleculesSets, lm);

            Logger.Log("ReactionEngine::Awake() loading molecules from file done"
                       +": _moleculesSets="+Logger.ToString<MoleculeSet>(_moleculesSets)
                       , Logger.Level.DEBUG);
		}
    MediumLoader mediumLoader = new MediumLoader();
    foreach (string file in _mediumsFiles)
		{
			LinkedList<Medium> lmed = mediumLoader.loadObjectsFromFile<Medium>(file,"Mediums");
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

        Logger.Log("ReactionEngine::Awake() FickReactions starting", Logger.Level.INFO);

    _fick = new Fick();
    _fick.loadFicksReactionsFromFiles(_fickFiles, _mediums);

        Logger.Log("ReactionEngine::Awake() activeTransport starting", Logger.Level.INFO);

    _activeTransport = new ActiveTransport();        
    _activeTransport.loadActiveTransportReactionsFromFiles(_activeTransportFiles, _mediums);

        Logger.Log("ReactionEngine::Awake() done", Logger.Level.INFO);
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
	    Logger.Log("ReactionEngine::Update paused", Logger.Level.TRACE);
	  } else {
	    _fick.react();
      if (enableShufflingMediumOrder)
        LinkedListExtensions.Shuffle<Medium>(_mediums);

      foreach (Medium medium in _mediums)
        medium.Update();

	    Logger.Log("ReactionEngine::Update() update of mediums done", Logger.Level.TRACE);
      if (!enableSequential) {
        foreach (Medium medium in _mediums)
          medium.updateMoleculesConcentrations();
		    Logger.Log("ReactionEngine::Update() update of mol cc in mediums done", Logger.Level.TRACE);
      }
	  }
  }
}

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

   \author    Pierre COLLET
   \mail pierre.collet91@gmail.com
 */
public class ReactionEngine : MonoBehaviour {

  private Fick _fick;                                   //!< The Fick class that manage molecules diffusions between medium
  private ActiveTransport       _activeTransport;       //!< The class that manage Active transport reactions.
  private LinkedList<Medium>    _mediums;               //!< The list that contain all the mediums
  private LinkedList<ReactionsSet> _reactionsSets;      //!< The list that contain the reactions sets
  private LinkedList<MoleculesSet> _moleculesSets;      //!< The list that contain the molecules sets
  public string[]      _mediumsFiles;                   //!< all the medium files
  public string[]      _reactionsFiles;                 //!< all the reactions files
  public string[]      _moleculesFiles;                 //!< all the molecules files
  public string[]      _fickFiles;                      //!< all the Fick diffusion files
  public string[]      _activeTransportFiles;           //!< all the Fick diffusion files
  public static float  reactionsSpeed = 0.9f;           //!< Global reactions speed
  public bool enableSequential;                         //!< Enable sequential mode (if reaction is compute one's after the others)
  public bool enableNoise;                              //!< Add Noise in each Reaction
  public bool enableEnergy;                             //!< Enable energy consomation
  public bool enableShufflingReactionOrder;             //!< Randomize reaction computation order in middles
  public bool enableShufflingMediumOrder;               //!< Randomize middles computation order
	
  private bool _paused;                                 //!< Simulation state
	
	
  //debug
  public bool _debug;
  private float _timeAtLastDebug = 0f;
  private float _timeAtCurrentFrame = 0f;
  private float _deltaTime = 0f;
  private float _deltaTimeThreshold = 0.5f;
	
  public Fick getFick() { return _fick; }
  
  /*!
    \brief Add an IReaction to a medium
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
	ReactionsSet reactionsSet = null;
	string medName = med.getName()+"Reactions";
	foreach (ReactionsSet rs in _reactionsSets) {
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
    foreach (Molecule mol in molecules)
      if (mol.getName() == name)
        return mol;
    return null;
  }

//! Return the ReactionsSet reference corresponding to the given id
  /*!
      \param id The id of the ReactionsSet
      \param list The list of ReactionsSet where to search in
  */
  public static ReactionsSet    getReactionsSetFromId(string id, LinkedList<ReactionsSet> list)
  {
    foreach (ReactionsSet reactSet in list)
      if (reactSet.id == id)
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

  //! Return an ArrayList that contain all the differents molecules an list of MoleculesSet
  /*!
      \param list the list of MoleculesSet
  */
  public static ArrayList    getAllMoleculesFromMoleculeSets(LinkedList<MoleculesSet> list)
  {
    ArrayList molecules = new ArrayList();

    
    foreach (MoleculesSet molSet in list)
      {
        foreach (Molecule mol in molSet.molecules)
          if (!isMoleculeIsDuplicated(mol, molecules))
            molecules.Add(mol);
      }
    return molecules;
  }

  //! Return the MoleculesSet of a list of MoleculesSet corresponding to an id
  /*!
    \param id The id of the MoleculesSet
    \param list The list of MoleculesSet
  */
  public static MoleculesSet    getMoleculesSetFromId(string id, LinkedList<MoleculesSet> list)
  {
    foreach (MoleculesSet molSet in list)
      if (molSet.id == id)
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
  public void Awake()
  {
    FileLoader fileLoader = new FileLoader();
    _reactionsSets = new LinkedList<ReactionsSet>();
    _moleculesSets = new LinkedList<MoleculesSet>();
    _mediums = new LinkedList<Medium>();
    
    foreach (string file in _reactionsFiles)
      LinkedListExtensions.AppendRange<ReactionsSet>(_reactionsSets, fileLoader.loadReactionsFromFile(file));
    foreach (string file in _moleculesFiles)
      LinkedListExtensions.AppendRange<MoleculesSet>(_moleculesSets, fileLoader.loadMoleculesFromFile(file));

    MediumLoader mediumLoader = new MediumLoader();
    foreach (string file in _mediumsFiles)
      LinkedListExtensions.AppendRange<Medium>(_mediums, mediumLoader.loadMediumsFromFile(file));
    foreach (Medium medium in _mediums)
      {
        medium.Init(_reactionsSets, _moleculesSets);
        medium.enableSequential(enableSequential);
        medium.enableNoise(enableNoise);
        medium.enableEnergy(enableEnergy);
        medium.enableShufflingReactionOrder = enableShufflingReactionOrder;
      }

    _fick = new Fick();
    _fick.loadFicksReactionsFromFiles(_fickFiles, _mediums);
    _activeTransport = new ActiveTransport();
    _activeTransport.loadActiveTransportReactionsFromFiles(_activeTransportFiles, _mediums);
  }
	
  //TODO manage reaction speed for smooth pausing
  public void Pause(bool pause) {
	_paused = pause;
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
	  
	  //TODO REMOVE
	  if(_debug) {
	  	_timeAtCurrentFrame = Time.realtimeSinceStartup;
	      _deltaTime = _timeAtCurrentFrame - _timeAtLastDebug;
	  		
	  	bool debug = (_deltaTime > _deltaTimeThreshold);
	  	if(debug) {
	  		_timeAtLastDebug = _timeAtCurrentFrame;
	  		//debug
	  		foreach (Medium medium in _mediums)
	  		  medium.Log();
	  	}
	  }
			
	  if (Input.GetKey(KeyCode.U)) {
	    //dump all reactions
		Logger.Log("ReactionEngine::Update Press U reactions="+getReactionsSetFromId("CelliaReactions", _reactionsSets)
					, Logger.Level.WARN
					);	
      }
	}
  }
}

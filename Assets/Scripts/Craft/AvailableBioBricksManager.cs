using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class AvailableBioBricksManager : MonoBehaviour {


  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "BioBrickInventory";
  private static AvailableBioBricksManager _instance;
  public static AvailableBioBricksManager get() {
    if(_instance == null) {
      Logger.Log("AvailableBioBricksManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<AvailableBioBricksManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("AvailableBioBricksManager::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////


  string[] _bioBrickFiles = new string[]{ "Assets/Data/raph/biobricks.xml" };

  //width of a displayed BioBrick
  //set in Unity editor
  public int _width;

  // the panel on which the BioBricks will be drawn
  public GameObject bioBricksPanel;

  //prefab for available biobricks
  public GameObject availableBioBrick;

  //visual, clickable biobricks currently displayed
  LinkedList<AvailableDisplayedBioBrick>  _displayedBioBricks   = new LinkedList<AvailableDisplayedBioBrick>();

  //biobrick data catalog
  /*
   * TODO use for optimization
  private static LinkedList<PromoterBrick>        _availablePromoters   = new LinkedList<PromoterBrick>();
  private static LinkedList<RBSBrick>             _availableRBS         = new LinkedList<RBSBrick>();
  private static LinkedList<GeneBrick>            _availableGenes       = new LinkedList<GeneBrick>();
  private static LinkedList<TerminatorBrick>      _availableTerminators = new LinkedList<TerminatorBrick>();
  */
  private static LinkedList<BioBrick>             _availableBioBricks   = new LinkedList<BioBrick>();

  //visual, clickable biobrick catalog
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailablePromoters   = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailableRBS         = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailableGenes       = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailableTerminators = new LinkedList<AvailableDisplayedBioBrick>();

  private void updateDisplayedBioBricks() {
    Logger.Log ("AvailableBioBricksManager::updateDisplayedBioBricks", Logger.Level.DEBUG);

    LinkedList<BioBrick> availablePromoters = new LinkedList<BioBrick>();
    LinkedListExtensions.AppendRange<BioBrick>(availablePromoters, getAvailableBioBricksOfType(BioBrick.Type.PROMOTER));
    LinkedList<BioBrick> availableRBS = new LinkedList<BioBrick>();
    LinkedListExtensions.AppendRange<BioBrick>(availableRBS, getAvailableBioBricksOfType(BioBrick.Type.RBS));
    LinkedList<BioBrick> availableGenes = new LinkedList<BioBrick>();
    LinkedListExtensions.AppendRange<BioBrick>(availableGenes, getAvailableBioBricksOfType(BioBrick.Type.GENE));
    LinkedList<BioBrick> availableTerminators = new LinkedList<BioBrick>();
    LinkedListExtensions.AppendRange<BioBrick>(availableTerminators, getAvailableBioBricksOfType(BioBrick.Type.TERMINATOR));

    _displayableAvailablePromoters   = getDisplayableAvailableBioBricks(
      availablePromoters
      , getDisplayableAvailableBioBrick
      );
    _displayableAvailableRBS         = getDisplayableAvailableBioBricks(
      availableRBS
      , getDisplayableAvailableBioBrick
      );
    _displayableAvailableGenes       = getDisplayableAvailableBioBricks(
      availableGenes
      , getDisplayableAvailableBioBrick
      );
    _displayableAvailableTerminators = getDisplayableAvailableBioBricks(
      availableTerminators
      , getDisplayableAvailableBioBrick
      );

    Logger.Log("AvailableBioBricksManager::updateDisplayedBioBricks"
      +"\n\navailablePromoters="+Logger.ToString<BioBrick>(availablePromoters)
      +",\n\navailableRBS="+Logger.ToString<BioBrick>(availableRBS)
      +",\n\navailableGenes="+Logger.ToString<BioBrick>(availableGenes)
      +",\n\navailableTerminators="+Logger.ToString<BioBrick>(availableTerminators)
    
      +",\n\n_displayableAvailablePromoters="+Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailablePromoters)
      +",\n\n_displayableAvailableRBS="+Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableRBS)
      +",\n\n_displayableAvailableGenes="+Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableGenes)
      +",\n\n_displayableAvailableTerminators="+Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableTerminators)
      , Logger.Level.TRACE);

    displayPromoters();

  }

  private LinkedList<BioBrick> getAvailableBioBricksOfType(BioBrick.Type type) {
    Logger.Log("AvailableBioBricksManager::getAvailableBioBricksOfType("+type+")", Logger.Level.TRACE);
    return LinkedListExtensions.Filter<BioBrick>(_availableBioBricks, b => (b.getType() == type));
  }

  public bool addAvailableBioBrick(BioBrick brick, bool updateView = true)
  {
    Logger.Log("AvailableBioBricksManager::addAvailableBioBrick("+brick+")", Logger.Level.DEBUG);
    string bbName = brick.getName();
    if ((null != brick)
     && (null == LinkedListExtensions.Find<BioBrick>(_availableBioBricks, b => b.getName() == bbName)))
    // TODO deeper safety check
    // && !LinkedListExtensions.Find<BioBrick>(_availableBioBricks, b => b..Equals(brick))
    {
      _availableBioBricks.AddLast(brick);
      if(updateView)
      {
        updateDisplayedBioBricks();
      }
      return true;
    }
    return false;
  }

  public void OnPanelEnabled()
  {
    Logger.Log("AvailableBioBricksManager::OnEnable", Logger.Level.DEBUG);
    updateDisplayedBioBricks();
  }

  public Vector3 getNewPosition(int index) {
    int bricksPerRow = 4;
    return availableBioBrick.transform.localPosition + new Vector3(
      (index%bricksPerRow)*_width,
      -(index/bricksPerRow)*_width,
      -0.1f);
  }

  private delegate AvailableDisplayedBioBrick DisplayableAvailableBioBrickCreator(BioBrick brick, int index);

  private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks(
    LinkedList<BioBrick> bioBricks
    , DisplayableAvailableBioBrickCreator creator
  ) {
    LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
    foreach (BioBrick brick in bioBricks) {
      AvailableDisplayedBioBrick availableBrick = creator(brick, result.Count);
      availableBrick.display(false);
      result.AddLast(availableBrick);
    }
    return result;
  }

  private AvailableDisplayedBioBrick getDisplayableAvailableBioBrick(BioBrick brick, int index) {

    Transform parentTransformParam = bioBricksPanel.transform;
    Vector3 localPositionParam = getNewPosition(index);
    string spriteNameParam = AvailableDisplayedBioBrick.getSpriteName(brick);
    BioBrick biobrickParam = brick;

    Logger.Log("AvailableBioBricksManager::getDisplayableAvailableBioBrick(brick="+brick+", index="+index+"),"
      +", parentTransformParam="+parentTransformParam
      +", localPositionParam="+localPositionParam
      +" (width="+_width+")"
      +", spriteNameParam="+spriteNameParam
      +", biobrickParam="+biobrickParam
      , Logger.Level.TRACE
      );

    AvailableDisplayedBioBrick resultBrick = AvailableDisplayedBioBrick.Create(
      parentTransformParam
      ,localPositionParam
      ,spriteNameParam
      ,biobrickParam
    );
    return resultBrick;
  }

  public void displayPromoters() {
    Logger.Log("AvailableBioBricksManager::displayPromoters", Logger.Level.TRACE);
    switchTo(_displayableAvailablePromoters);
  }
  public void displayRBS() {
    Logger.Log("AvailableBioBricksManager::displayRBS", Logger.Level.TRACE);
    switchTo(_displayableAvailableRBS);
  }
  public void displayGenes() {
    Logger.Log("AvailableBioBricksManager::displayGenes", Logger.Level.TRACE);
    switchTo(_displayableAvailableGenes);
  }
  public void displayTerminators() {
    Logger.Log("AvailableBioBricksManager::displayTerminators", Logger.Level.TRACE);
    switchTo(_displayableAvailableTerminators);
  }
  private void switchTo(LinkedList<AvailableDisplayedBioBrick> list) {
    string listToString = "list=[";
    foreach(AvailableDisplayedBioBrick brick in list) {
      listToString += brick.ToString()+", ";
    }
    listToString += "]";
    Logger.Log("AvailableBioBricksManager::switchTo("+listToString+")", Logger.Level.TRACE);
    display(_displayedBioBricks, false);
    _displayedBioBricks.Clear();
    _displayedBioBricks.AppendRange(list);
    display(_displayedBioBricks, true);
  }

  private void display(LinkedList<AvailableDisplayedBioBrick> bricks, bool enabled) {
    foreach (AvailableDisplayedBioBrick brick in bricks) {
      brick.display(enabled);
    }
  }
		
  public LinkedList<BioBrick> getAvailableBioBricks() {
	Logger.Log("AvailableBioBricksManager::getAvailableBioBricks", Logger.Level.DEBUG);
	if(_availableBioBricks == null || _availableBioBricks.Count == 0) {
	  loadAvailableBioBricks();
	}
	return _availableBioBricks;
  }
	
  private void loadAvailableBioBricks() {
	Logger.Log("AvailableBioBricksManager::loadAvailableBioBricks", Logger.Level.DEBUG);
    //load biobricks from xml
    BioBrickLoader bLoader = new BioBrickLoader();

    _availableBioBricks   = new LinkedList<BioBrick>();
	string files = "";

    foreach (string file in _bioBrickFiles) {
      Logger.Log("AvailableBioBricksManager::loadAvailableBioBricks loads biobrick file "+file, Logger.Level.TRACE);
      LinkedListExtensions.AppendRange<BioBrick>(_availableBioBricks, bLoader.loadBioBricksFromFile(file));
	  if(!string.IsNullOrEmpty(files)) {
		files += ", ";
	  }
	  files += file;
    }
    Logger.Log("AvailableBioBricksManager::loadAvailableBioBricks loaded "+files, Logger.Level.DEBUG);
  }

  // Use this for initialization
  void Start () {
    Logger.Log("AvailableBioBricksManager::Start()", Logger.Level.TRACE);
    displayPromoters();
  }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO:
 * Replace LinkedList by an array or fields
 * OnHover for DisplayedBioBrick
 * OnPress for AvailableDisplayedBioBrick + Update of _currentDisplayedBricks in CraftZone
 * OnPress for CraftZoneDisplayedBioBrick + Update of _currentDisplayedBricks in CraftZone
 * Update of state of CraftFinalizationButton
 */

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class CraftZoneManager : MonoBehaviour {

  private LinkedList<BioBrick>              _currentBioBricks = new LinkedList<BioBrick>();
  private LinkedList<DisplayedBioBrick>     _currentDisplayedBricks = new LinkedList<DisplayedBioBrick>();
  private Device                            _currentDevice = null;

  private PromoterBrick                     _promoter;
  private RBSBrick                          _RBS;
  private GeneBrick                         _gene;
  private TerminatorBrick                   _terminator;

  public GameObject                         displayedBioBrick;
  public LastHoveredInfoManager             lastHoveredInfoManager;
  public FinalizationInfoPanelManager       finalizationInfoPanelManager;
  public Inventory                          inventory;

  //width of a displayed BioBrick
  public int _width = 200;

  public static Dictionary<Inventory.AddingFailure, string>   statusMessagesDictionary =
    new Dictionary<Inventory.AddingFailure, string>() {
      {Inventory.AddingFailure.DEFAULT,         "invalid device!"},
      {Inventory.AddingFailure.NONE,            "new device"},
      {Inventory.AddingFailure.SAME_BRICKS,     "device with same bricks already exists!"},
      {Inventory.AddingFailure.SAME_NAME,       "device with same name already exists!"}
    };


  public LinkedList<DisplayedBioBrick> getCurrentDisplayedBricks() {
    return new LinkedList<DisplayedBioBrick>(_currentDisplayedBricks);
  }

  private Vector3 getNewPosition(int index ) {
      return displayedBioBrick.transform.localPosition + new Vector3(index*_width, 0.0f, -0.1f);
  }

	// Use this for initialization
	void Start () {
    Logger.Log("CraftZoneManager::Start starting...", Logger.Level.TRACE);
    //promoter
    float beta = 10.0f;
    string formula = "![0.8,2]LacI";
    //rbs
    float rbsFactor = 1.0f;
    //gene
    string proteinName = DevicesDisplayer.getRandomProteinName();
    //terminator
    float terminatorFactor = 1.0f;

    string notNullName = "craftTest";
    LinkedList<BioBrick> bioBrickList = new LinkedList<BioBrick>();
    PromoterBrick promoter = new PromoterBrick(notNullName+"_promoter", beta, formula);
    bioBrickList.AddLast(promoter);

    RBSBrick RBS = new RBSBrick(notNullName+"_rbs", rbsFactor);
    bioBrickList.AddLast(RBS);

    GeneBrick gene = new GeneBrick(notNullName+"_gene", proteinName);
    bioBrickList.AddLast(gene);

    TerminatorBrick terminator = new TerminatorBrick(notNullName+"_terminator", terminatorFactor);
    bioBrickList.AddLast(terminator);

    setBioBricks(bioBrickList);

    Logger.Log("CraftZoneManager::Start ...ending!", Logger.Level.TRACE);
	}


  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //  BioBricks
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  public void setBioBricks(LinkedList<BioBrick> bricks) {
    _currentBioBricks.Clear();
    _currentBioBricks.AppendRange(bricks);
    displayBioBricks();

    _currentDevice = getDeviceFromBricks(bricks);
    displayDevice();
  }

  private void displayBioBricks() {
    Debug.Log("CraftZoneManager::displayBioBricks() with _currentBioBricks="+Logger.ToString<BioBrick>(_currentBioBricks));
    removePreviousDisplayedBricks();

    //add new biobricks
    int index = 0;
    foreach (BioBrick brick in _currentBioBricks) {
      Debug.Log("CraftZoneManager::displayBioBricks brick="+brick);
      _currentDisplayedBricks.AddLast(DisplayedBioBrick.Create(transform, getNewPosition(index), null, brick));
      index++;
    }

    //initialize the "last hovered biobrick" info window
    lastHoveredInfoManager.setHoveredBioBrick(_currentBioBricks.First.Value);
  }

  private void removePreviousDisplayedBricks() {
    Logger.Log("CraftZoneManager::removePreviousDisplayedBricks()", Logger.Level.WARN);
    //remove all previous biobricks
    foreach (DisplayedBioBrick brick in _currentDisplayedBricks) {
      Destroy(brick.gameObject);
    }
    _currentDisplayedBricks.Clear();
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // utilities

  private DisplayedBioBrick findFirstBioBrick(BioBrick.Type type) {
    foreach(DisplayedBioBrick brick in _currentDisplayedBricks) {
      if(brick._biobrick.getType() == type) return brick;
    }
    Logger.Log("CraftZoneManager::findFirstBioBrick("+type+") failed with current bricks="+_currentDisplayedBricks);
    return null;
  }

  public void replaceWithBrick(DisplayedBioBrick dBioBrick) {
    DisplayedBioBrick toReplace = findFirstBioBrick(dBioBrick._biobrick.getType());
    LinkedListNode<DisplayedBioBrick> toReplaceNode = _currentDisplayedBricks.Find(toReplace);

    DisplayedBioBrick newBrick = DisplayedBioBrick.Create(
      transform,
      toReplace.transform.localPosition,
      dBioBrick._sprite.spriteName,
      dBioBrick._biobrick
      );

    _currentDisplayedBricks.AddAfter(toReplaceNode, newBrick);
    _currentDisplayedBricks.Remove(toReplace);
    Destroy(toReplace.gameObject);
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //  Device
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  public void setDevice(Device device) {
    _currentDevice = device;
    displayDevice();

    _currentBioBricks.Clear();
    _currentBioBricks.AppendRange(device.getExpressionModules().First.Value.getBioBricks());
    displayBioBricks();
  }

  private void displayDevice() {
    Debug.Log("CraftZoneManager::displayDevice() with _currentDevice="+_currentDevice+")");
    string status = getDeviceStatus(_currentDevice);
    finalizationInfoPanelManager.setDisplayedDevice(_currentDevice, status);
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // utilities

  private string getDeviceStatus(Device device){
    Logger.Log("CraftZoneManager::getDeviceStatus("+device+")", Logger.Level.WARN);
    if(device!=null) {
      Inventory.AddingFailure failure = inventory.canAddDevice(device);
      return statusMessagesDictionary[failure];
    } else {
      Logger.Log("CraftZoneManager::getDeviceStatus: invalid device", Logger.Level.WARN);
      return statusMessagesDictionary[Inventory.AddingFailure.DEFAULT];
    }
  }

  private Device getDeviceFromBricks(LinkedList<BioBrick> bricks){
    Logger.Log("CraftZoneManager::getDeviceFromBricks("+Logger.ToString<BioBrick>(bricks)+")", Logger.Level.WARN);
    ExpressionModule module = new ExpressionModule("test", bricks);
    LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
    modules.AddLast(module);
    Device device = Device.buildDevice("test", modules);
    Logger.Log("CraftZoneManager::getDeviceFromBricks produced "+device, Logger.Level.WARN);
    return device;
  }
}

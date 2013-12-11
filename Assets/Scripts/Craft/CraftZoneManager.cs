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


  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "CraftZoneManager";
  private static CraftZoneManager _instance;
  public static CraftZoneManager get() {
    if(_instance == null) {
      Logger.Log("CraftZoneManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<CraftZoneManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("CraftZoneManager::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  private LinkedList<BioBrick>              _currentBioBricks = new LinkedList<BioBrick>();
  private LinkedList<DisplayedBioBrick>     _currentDisplayedBricks = new LinkedList<DisplayedBioBrick>();
  private Device                            _currentDevice = null;

  private PromoterBrick                     _promoter;
  private RBSBrick                          _RBS;
  private GeneBrick                         _gene;
  private TerminatorBrick                   _terminator;

  public GameObject                         displayedBioBrick;
  public LastHoveredInfoManager             lastHoveredInfoManager;
  public CraftFinalizer                     craftFinalizer;

  //width of a displayed BioBrick
  public int _width = 200;


  public LinkedList<DisplayedBioBrick> getCurrentDisplayedBricks() {
    return new LinkedList<DisplayedBioBrick>(_currentDisplayedBricks);
  }

  private Vector3 getNewPosition(int index ) {
      return displayedBioBrick.transform.localPosition + new Vector3(index*_width, 0.0f, -0.1f);
  }


  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //  BioBricks
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  public void setBioBricks(LinkedList<BioBrick> bricks) {
    Logger.Log("CraftZoneManager::setBioBricks("+Logger.ToString<BioBrick>(bricks)+")", Logger.Level.TRACE);
    _currentBioBricks.Clear();
    _currentBioBricks.AppendRange(bricks);
    OnBioBricksChanged();
  }

  private void OnBioBricksChanged() {
    Logger.Log("CraftZoneManager::OnBioBricksChanged()", Logger.Level.TRACE);
    displayBioBricks();

    _currentDevice = getDeviceFromBricks(_currentBioBricks);
    displayDevice();
  }

  private void displayBioBricks() {
    Logger.Log("CraftZoneManager::displayBioBricks() with _currentBioBricks="+Logger.ToString<BioBrick>(_currentBioBricks), Logger.Level.TRACE);
    removePreviousDisplayedBricks();

    //add new biobricks
    int index = 0;
    foreach (BioBrick brick in _currentBioBricks) {
      Logger.Log("CraftZoneManager::displayBioBricks brick="+brick, Logger.Level.TRACE);
      _currentDisplayedBricks.AddLast(DisplayedBioBrick.Create(transform, getNewPosition(index), null, brick));
      index++;
    }

    //set to true to initialize the "last hovered biobrick" info window
    bool initializeHovered = false;
    if(initializeHovered) {
      lastHoveredInfoManager.setHoveredBioBrick(_currentBioBricks.First.Value);
    }
  }

  private void removePreviousDisplayedBricks() {
    Logger.Log("CraftZoneManager::removePreviousDisplayedBricks()", Logger.Level.TRACE);
    //remove all previous biobricks
    foreach (DisplayedBioBrick brick in _currentDisplayedBricks) {
      Destroy(brick.gameObject);
    }
    _currentDisplayedBricks.Clear();
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // utilities

  private BioBrick findFirstBioBrick(BioBrick.Type type) {
    foreach(BioBrick brick in _currentBioBricks) {
      if(brick.getType() == type) return brick;
    }
    Logger.Log("CraftZoneManager::findFirstBioBrick("+type+") failed with current bricks="+_currentBioBricks, Logger.Level.TRACE);
    return null;
  }

  public void replaceWithBioBrick(BioBrick brick) {
    Logger.Log("CraftZoneManager::replaceWithBioBrick("+brick+")", Logger.Level.TRACE);
    BioBrick toReplace = findFirstBioBrick(brick.getType());
    LinkedListNode<BioBrick> toReplaceNode = _currentBioBricks.Find(toReplace);
    _currentBioBricks.AddAfter(toReplaceNode, brick);
    _currentBioBricks.Remove(toReplace);
    OnBioBricksChanged();
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //  Device
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  public void askSetDevice(Device device) {
    Logger.Log("CraftZoneManager::askSetDevice("+device+")", Logger.Level.TRACE);
    if(_currentDevice == null) {
      setDevice(device);
    }
  }

  public void setDevice(Device device) {
    Logger.Log("CraftZoneManager::setDevice("+device+")", Logger.Level.TRACE);
    _currentDevice = device;
    displayDevice();

    _currentBioBricks.Clear();
    _currentBioBricks.AppendRange(device.getExpressionModules().First.Value.getBioBricks());
    displayBioBricks();
  }

  public void displayDevice() {
    Logger.Log("CraftZoneManager::displayDevice() with _currentDevice="+_currentDevice+")", Logger.Level.TRACE);
    craftFinalizer.setDisplayedDevice(_currentDevice);
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // utilities

  private Device getDeviceFromBricks(LinkedList<BioBrick> bricks){
    Logger.Log("CraftZoneManager::getDeviceFromBricks("+Logger.ToString<BioBrick>(bricks)+")", Logger.Level.TRACE);
    ExpressionModule module = new ExpressionModule("test", bricks);
    LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
    modules.AddLast(module);
    //Device device = Device.buildDevice(Inventory.get().getAvailableDeviceName(), modules);
    Device device = Device.buildDevice(GameplayNames.generateRealNameFromBricks(bricks), modules);
    if(device != null)
    {
      Logger.Log("CraftZoneManager::getDeviceFromBricks produced "+device, Logger.Level.TRACE);
    }
    else
    {
      Logger.Log ("CraftZoneManager::getDeviceFromBricks device==null with bricks="+Logger.ToString<BioBrick>(bricks)
        , Logger.Level.WARN);
    }
    return device;
  }

  public Device getCurrentDevice() {
    return _currentDevice;
  }
}

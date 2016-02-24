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
  private LinkedList<CraftZoneDisplayedBioBrick>     _currentDisplayedBricks = new LinkedList<CraftZoneDisplayedBioBrick>();
  private Device                            _currentDevice = null;

  private PromoterBrick                     _promoter;
  private RBSBrick                          _RBS;
  private GeneBrick                         _gene;
  private TerminatorBrick                   _terminator;

  public GameObject                         displayedBioBrick;
  public LastHoveredInfoManager             lastHoveredInfoManager;
  public CraftFinalizer                     craftFinalizer;
  public GameObject                         assemblyZonePanel;

  //width of a displayed BioBrick
  private int _width = 118;


  public LinkedList<CraftZoneDisplayedBioBrick> getCurrentDisplayedBricks() {
    return new LinkedList<CraftZoneDisplayedBioBrick>(_currentDisplayedBricks);
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

  private static int getIndex(BioBrick brick)
  {
    int idx;
    switch(brick.getType())
      {
        case BioBrick.Type.PROMOTER:
          idx = 0;
          break;
        case BioBrick.Type.RBS:
          idx = 1;
          break;
        case BioBrick.Type.GENE:
          idx = 2;
          break;
        case BioBrick.Type.TERMINATOR:
          idx = 3;
          break;
      default:
        idx = 0;
        Logger.Log("CraftZoneManager getIndex unknown type "+brick.getType(), Logger.Level.WARN);
        break;
      }
    return idx;
  }

  private void displayBioBricks() {
    Logger.Log("CraftZoneManager::displayBioBricks() with _currentBioBricks="+Logger.ToString<BioBrick>(_currentBioBricks), Logger.Level.TRACE);
    removePreviousDisplayedBricks();

    //add new biobricks
    //int index = 0;
    foreach (BioBrick brick in _currentBioBricks) {
      Logger.Log("CraftZoneManager::displayBioBricks brick="+brick, Logger.Level.TRACE);
      _currentDisplayedBricks.AddLast(
        CraftZoneDisplayedBioBrick.Create(
          assemblyZonePanel.transform,
          getNewPosition(getIndex(brick)),
          null,
          brick
        )
      );
      //index++;
    }

    /*
    //set to true to initialize the "last hovered biobrick" info window
    bool initializeHovered = false;
    if(initializeHovered) {
      lastHoveredInfoManager.setHoveredBioBrick(_currentBioBricks.First.Value);
    }
    */
  }

  private void removePreviousDisplayedBricks() {
    Logger.Log("CraftZoneManager::removePreviousDisplayedBricks()", Logger.Level.TRACE);
    //remove all previous biobricks
    foreach (CraftZoneDisplayedBioBrick brick in _currentDisplayedBricks) {
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
    insertOrdered(brick);
    OnBioBricksChanged();
  }

  private void insertOrdered(BioBrick toInsert)
  {
    foreach(BioBrick brick in _currentBioBricks)
    {
      if(brick.getType() > toInsert.getType())
      {
        LinkedListNode<BioBrick> afterNode = _currentBioBricks.Find(brick);
        _currentBioBricks.AddBefore(afterNode, toInsert);
        return;
      }
      else if(brick.getType() == toInsert.getType())
      {
        LinkedListNode<BioBrick> toReplaceNode = _currentBioBricks.Find(brick);
        _currentBioBricks.AddAfter(toReplaceNode, toInsert);
        _currentBioBricks.Remove(brick);
        return;
      }
    }
    _currentBioBricks.AddLast(toInsert);
  }

  public void removeBioBrick(BioBrick brick) {
    Logger.Log("CraftZoneManager::removeBioBrick("+brick+")", Logger.Level.TRACE);
    _currentBioBricks.Remove(brick);
    OnBioBricksChanged();
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //  Device
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  /*
  public void askSetDevice(Device device) {
    Logger.Log("CraftZoneManager::askSetDevice("+device+")", Logger.Level.TRACE);
    if(_currentDevice == null) {
      setDevice(device);
    }
  }
  */

  public void setDevice(Device device) {
    Logger.Log("CraftZoneManager::setDevice("+device+")", Logger.Level.TRACE);
    _currentDevice = device;
    displayDevice();

    _currentBioBricks.Clear();
    if(device != null)
    {
      _currentBioBricks.AppendRange(device.getExpressionModules().First.Value.getBioBricks());
    }
    displayBioBricks();
  }

  public void displayDevice() {
        if(null != craftFinalizer) {
            craftFinalizer.setDisplayedDevice(_currentDevice);
        }
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // utilities

    private Device getDeviceFromBricks(LinkedList<BioBrick> bricks){
        Logger.Log("CraftZoneManager::getDeviceFromBricks("+Logger.ToString<BioBrick>(bricks)+")", Logger.Level.TRACE);

        if(!ExpressionModule.isBioBricksSequenceValid(bricks))
        {
            Logger.Log("CraftZoneManager::getDeviceFromBricks invalid biobricks sequence", Logger.Level.TRACE);
            return null;
        }

        ExpressionModule module = new ExpressionModule("test", bricks);
        LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
        modules.AddLast(module);

        Device device = Device.buildDevice(modules);
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

  public static bool isOpenable()
  {
    //FIXME doesn't work with test null != _instance._currentDevice
    return 0 != AvailableBioBricksManager.get().getAvailableBioBricks().Count;
  }

  void Start()
  {
    displayDevice();
  }
}

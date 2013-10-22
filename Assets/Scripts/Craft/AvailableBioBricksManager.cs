using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class AvailableBioBricksManager : MonoBehaviour {

  string[] _bioBrickFiles = new string[]{ "Assets/Data/raph/biobricks.xml" };
  string[] _deviceFiles = new string[]{ "Assets/Data/raph/devices.xml" };

  //width of a displayed BioBrick
  public int _width = 12;

  //prefab for available biobricks
  public GameObject availableBioBrick;

  public Inventory inventory;

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

  }

  private LinkedList<BioBrick> getAvailableBioBricksOfType(BioBrick.Type type) {
    Logger.Log("AvailableBioBricksManager::getAvailableBioBricksOfType("+type+")", Logger.Level.TRACE);
    return LinkedListExtensions.Filter<BioBrick>(_availableBioBricks, b => (b.getType() == type));
  }

  public Vector3 getNewPosition(int index) {
    //TODO manage rows and columns
      return availableBioBrick.transform.localPosition + new Vector3(index*_width, 0.0f, -0.1f);
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

    Transform parentTransformParam = transform;
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

	// Use this for initialization
	void Start () {
    Logger.Log("AvailableBioBricksManager::Start() starts", Logger.Level.TRACE);
	  updateDisplayedBioBricks();
    displayPromoters();

    //Loads devices
    //Assumes that referenced biobricks have already been loaded
    List<Device> devices = new List<Device>();

    DeviceLoader dLoader = new DeviceLoader(_availableBioBricks);
    foreach (string file in _deviceFiles) {
      Logger.Log("AvailableBioBricksManager::Start loads device file "+file, Logger.Level.TRACE);
      devices.AddRange(dLoader.loadDevicesFromFile(file));
    }
    Logger.Log("AvailableBioBricksManager: calling inventory.UpdateData(List("+Logger.ToString<Device>(devices)+"), List(), List())", Logger.Level.TRACE);
    inventory.UpdateData(devices, new List<Device>(), new List<Device>());

    //SAVE test
    DeviceSaver dSaver = new DeviceSaver();
    dSaver.saveDevicesToFile(devices, "Assets/Data/raph/userDevices.xml");

    Logger.Log("AvailableBioBricksManager::Start() ends", Logger.Level.TRACE);
	}

  void Awake () {
    Logger.Log("AvailableBioBricksManager::Awake", Logger.Level.INFO);
    //load biobricks from xml
    BioBrickLoader bLoader = new BioBrickLoader();

    _availableBioBricks   = new LinkedList<BioBrick>();

    foreach (string file in _bioBrickFiles) {
      Logger.Log("AvailableBioBricksManager::Awake loads biobrick file "+file, Logger.Level.TRACE);
      LinkedListExtensions.AppendRange<BioBrick>(_availableBioBricks, bLoader.loadBioBricksFromFile(file));
    }
    Logger.Log("AvailableBioBricksManager::Awake loaded "+_bioBrickFiles, Logger.Level.INFO);
  }

}

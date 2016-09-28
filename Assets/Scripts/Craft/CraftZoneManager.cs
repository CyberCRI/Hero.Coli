using UnityEngine;
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
    protected const string gameObjectName = "CraftZoneManager";
    protected static CraftZoneManager _instance;
    public static CraftZoneManager get()
    {
        //Debug.LogError("CraftZoneManager get");
        if (_instance == null)
        {
            Logger.Log("CraftZoneManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<CraftZoneManager>();
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError(this.GetType() + " has two running instances");
        }
        _instance = this;
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    protected bool _initialized = false;
    public virtual void initializeIfNecessary()
    {
        if (!_initialized)
        {
            if(null != displayedBioBrick)
            {
                displayedBioBrick.SetActive(false);
            }
                _initialized = true;
            }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");

        initializeIfNecessary();
        displayDevice();
    }
  
  ////////////////////////////////////////////////////////////////////////////////////////////

  protected LinkedList<BioBrick>                      _currentBioBricks = new LinkedList<BioBrick>();
  protected LinkedList<CraftZoneDisplayedBioBrick>    _currentDisplayedBricks = new LinkedList<CraftZoneDisplayedBioBrick>();
  protected Device                                    _currentDevice = null;

/*
  protected PromoterBrick                     _promoter;
  protected RBSBrick                          _RBS;
  protected GeneBrick                         _gene;
  protected TerminatorBrick                   _terminator;
*/

  public GameObject                         displayedBioBrick;
  public LastHoveredInfoManager             lastHoveredInfoManager;
  public CraftFinalizer                     craftFinalizer;
  public GameObject                         assemblyZonePanel;
  
  protected static EditMode editMode = EditMode.UNLOCKED;
  public static bool isDeviceEditionOn() {
      return EditMode.UNLOCKED == editMode;
  }
  public static void setDeviceEdition(bool editionOn)
  {
      editMode = editionOn?EditMode.UNLOCKED:EditMode.LOCKED;
  }

  //width of a displayed BioBrick
  protected int _width = 118;

  protected enum ProcessType {
      
      // 'craft' creates new recipes,
      // equip only in inventory
      MODE1,
      
      // 'activate' creates new recipes and equips and locks edition,
      // 'inactivate' unequips and unlocks edition
      MODE2,
      
      // 'activate' creates new recipes and equips,
      // 'inactivate' unequips,
      // clicking on bricks of an equiped device always trigger unequips
      MODE3
      
  }
  
  protected enum EditMode {
      LOCKED,
      UNLOCKED
  }

  public LinkedList<CraftZoneDisplayedBioBrick> getCurrentDisplayedBricks() {
    return new LinkedList<CraftZoneDisplayedBioBrick>(_currentDisplayedBricks);
  }

  protected Vector3 getNewPosition(int index ) {
      return displayedBioBrick.transform.localPosition + new Vector3(index*_width, 0.0f, -0.1f);
  }


  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //  BioBricks
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  public void setBioBricks(LinkedList<BioBrick> bricks) {
    Logger.Log("CraftZoneManager::setBioBricks("+Logger.ToString<BioBrick>(bricks)+")", Logger.Level.TRACE);
    removeAllBricksFromCraftZone();
    
    foreach(BioBrick brick in bricks)
    {
        AvailableBioBricksManager.get().addBrickAmount(brick, -1);
    }    
    _currentBioBricks.AppendRange(bricks);
    
    OnBioBricksChanged();
  }
  
  public Equipment.AddingResult equip()
  {
      Equipment.AddingResult result = Equipment.get().askAddDevice(getCurrentDevice());
      OnBioBricksChanged();
      return result;
  }
  
  public void unequip()
  {
      Equipment.get().removeDevice(getCurrentDevice());
      OnBioBricksChanged();
  }

  public virtual void unequip(Device device)
  {
      unequip();
  }

  public virtual void OnBioBricksChanged() {
    Logger.Log("CraftZoneManager::OnBioBricksChanged()", Logger.Level.TRACE);
    displayBioBricks();

    _currentDevice = getDeviceFromBricks(_currentBioBricks);
    displayDevice();
  }

  protected static int getIndex(BioBrick brick)
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

  protected void displayBioBricks() {
      //Debug.LogError("CraftZoneManager::displayBioBricks()");
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

  protected void removePreviousDisplayedBricks() {
    Logger.Log("CraftZoneManager::removePreviousDisplayedBricks()", Logger.Level.TRACE);
    //remove all previous biobricks
    foreach (CraftZoneDisplayedBioBrick brick in _currentDisplayedBricks) {
      Destroy(brick.gameObject);
    }
    _currentDisplayedBricks.Clear();
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // utilities

  protected BioBrick findFirstBioBrick(BioBrick.Type type) {
    foreach(BioBrick brick in _currentBioBricks) {
      if(brick.getType() == type) return brick;
    }
    Logger.Log("CraftZoneManager::findFirstBioBrick("+type+") failed with current bricks="+_currentBioBricks, Logger.Level.TRACE);
    return null;
  }

  public virtual void replaceWithBioBrick(BioBrick brick) {
      //Debug.LogError("replaceWithBioBrick("+brick.getName()+")");
    Logger.Log("CraftZoneManager::replaceWithBioBrick("+brick+")", Logger.Level.TRACE);
    insertOrdered(brick);
    OnBioBricksChanged();
  }

    protected void insertOrdered(BioBrick toInsert)
    {
        //Debug.LogError("insertOrdered("+toInsert.getName()+")");
        BioBrick sameBrick = LinkedListExtensions.Find(_currentBioBricks, b => b.getName() == toInsert.getName());

        if (null != sameBrick)
        {
            // the brick is already present on the crafting table: remove it
            removeBioBrick(toInsert);
        }
        else
        {
            bool inserted = false;

            foreach (BioBrick brick in _currentBioBricks)
            {
                if (brick.getType() > toInsert.getType())
                {
                    // the brick is inserted before the next brick
                    LinkedListNode<BioBrick> afterNode = _currentBioBricks.Find(brick);
                    _currentBioBricks.AddBefore(afterNode, toInsert);
                    inserted = true;
                    break;
                }
                else if (brick.getType() == toInsert.getType())
                {
                    // the brick will replace a brick of the same type
                    LinkedListNode<BioBrick> toReplaceNode = _currentBioBricks.Find(brick);
                    _currentBioBricks.AddAfter(toReplaceNode, toInsert);
                    _currentBioBricks.Remove(brick);

                    //the brick is put out of the crafting table and is therefore available for new crafts
                    AvailableBioBricksManager.get().addBrickAmount(brick, 1);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
            {
                // the brick is inserted in the last position
                _currentBioBricks.AddLast(toInsert);
            }

            // the brick was inserted: there's one less brick to use
            AvailableBioBricksManager.get().addBrickAmount(toInsert, -1);
        }
    }

  public virtual void removeBioBrick(BioBrick brick) {
      //Debug.LogError("CraftZoneManager::removeBioBrick");
      string debug = null != brick? "contains="+_currentBioBricks.Contains(brick):"brick==null";
      //Debug.LogError("CraftZoneManager::removeBioBrick with "+debug);
      if(null != brick && _currentBioBricks.Contains(brick))
      {
        Logger.Log("CraftZoneManager::removeBioBrick("+brick+")", Logger.Level.TRACE);
        unequip();
        
        _currentBioBricks.Remove(brick);
        AvailableBioBricksManager.get().addBrickAmount(brick, 1);         
        OnBioBricksChanged();
      }
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

  public virtual void addAndEquipDevice(Device device, bool replace = true)
  {
      setDevice(device);
  }

  public virtual void setDevice(Device device) {
    Logger.Log("CraftZoneManager::setDevice("+device+")", Logger.Level.TRACE);
    //Debug.LogError("CraftZoneManager::setDevice("+device+")");
    if(device != null)
    {
        removeAllBricksFromCraftZone();
        
        _currentBioBricks.AppendRange(device.getExpressionModules().First.Value.getBioBricks());
        consumeBricks();
        
        _currentDevice = device;
        displayDevice();
        
        displayBioBricks();
    }    
  }
  
  protected virtual void removeAllBricksFromCraftZone() {
      //Debug.LogError("CraftZoneManager::removeAllBricksFromCraftZone()");
      foreach(BioBrick brick in _currentBioBricks)
      {
          AvailableBioBricksManager.get().addBrickAmount(brick, 1);
      }
      
      _currentBioBricks.Clear();
  }
  
  public virtual void craft () {
      //consumeBricks();
      displayDevice();
  }
  
  protected void consumeBricks() {
      //Debug.LogError("CraftZoneManager::consumeBricks()");
      foreach(BioBrick brick in _currentBioBricks) {
           AvailableBioBricksManager.get().addBrickAmount(brick, -1);
      }
  }

  protected void displayDevice() {
      //Debug.LogError("CraftZoneManager::displayDevice()");
        if(null != craftFinalizer) {
            craftFinalizer.setDisplayedDevice(_currentDevice);
        }
  }

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // utilities

    public Device getDeviceFromBricks(LinkedList<BioBrick> bricks){
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

  public virtual Device getCurrentDevice() {
    return _currentDevice;
  }

  public static bool isOpenable()
  {
    //FIXME doesn't work with test null != _instance._currentDevice
    return !(0 == AvailableBioBricksManager.get().getAvailableBioBricks().Count || Hero.isBeingInjured);
  }
}

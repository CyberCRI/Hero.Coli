using UnityEngine;
using System.Collections.Generic;

public class CraftFinalizer : MonoBehaviour {
    
  //////////////////////////////// singleton fields & methods ////////////////////////////////
  protected const string gameObjectName = "FinalizationZonePanel";
    protected static CraftFinalizer _instance;
    public static CraftFinalizer get()
    {
        //Debug.LogError("CraftFinalizer get");
        if (_instance == null)
        {
            Logger.Log("CraftFinalizer::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<CraftFinalizer>();
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
        initializeIfNecessary();
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            _initialized = true;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        
        craftZoneManager = CraftZoneManager.get();
    }
  ////////////////////////////////////////////////////////////////////////////////////////////
  private CraftZoneManager _craftZoneManager;

    private CraftZoneManager craftZoneManager
    {
        get
        {
            if (null == _craftZoneManager)
            {
                craftZoneManager = CraftZoneManager.get();
            }
            return _craftZoneManager;
        }
        set
        {
            _craftZoneManager = value;
        }
    }
  
  // these three objects are affected by the craft status of the currently displayed device
  public CraftFinalizationButton        craftFinalizationButton;
  public GameObject                     isCraftedStatus;
  public GameObject                     isUncraftedStatus;

  private void setCraftedStatus(CraftFinalizationButton.CraftMode mode) {
      if(isCraftedStatus && isUncraftedStatus)
      {
          craftFinalizationButton.setCraftMode(mode);
          
          isCraftedStatus.SetActive(CraftFinalizationButton.CraftMode.UNCRAFT != mode);
          isUncraftedStatus.SetActive(CraftFinalizationButton.CraftMode.UNCRAFT == mode);
      }
  }

  public static Dictionary<Inventory.AddingResult, string>   statusMessagesDictionary =
    new Dictionary<Inventory.AddingResult, string>() {
        {Inventory.AddingResult.SUCCESS,                "CRAFT.FINALIZATION.SUCCESS"},
        {Inventory.AddingResult.FAILURE_SAME_NAME,      "CRAFT.FINALIZATION.FAILURE_SAME_NAME"},
        {Inventory.AddingResult.FAILURE_SAME_BRICKS,    "CRAFT.FINALIZATION.FAILURE_SAME_BRICKS"},
        {Inventory.AddingResult.FAILURE_SAME_DEVICE,    "CRAFT.FINALIZATION.FAILURE_SAME_DEVICE"},
        {Inventory.AddingResult.FAILURE_DEFAULT,        "CRAFT.FINALIZATION.FAILURE_DEFAULT"}
    };

    public void uncraft() {
        //Debug.LogError("uncraft");
    }

  public bool finalizeCraft() {
      bool newCraft = false;
    //create new device from current biobricks in craft zone
    Logger.Log("CraftFinalizer::finalizeCraft()", Logger.Level.DEBUG);
    Device currentDevice = craftZoneManager.getCurrentDevice();
    if(currentDevice != null){
        // TODO pipeline when recipe is unknown
      Inventory.AddingResult addingResult = Inventory.get().askAddDevice(currentDevice, true);
      if(addingResult == Inventory.AddingResult.SUCCESS) {
        craftZoneManager.craft();
        newCraft = true;
      }
      Logger.Log("CraftFinalizer::finalizeCraft(): device="+currentDevice, Logger.Level.TRACE);
    } else {
      Logger.Log("CraftFinalizer::finalizeCraft() failed: invalid device (null)", Logger.Level.WARN);
    }
    return newCraft;
        //TODO RedMetrics reporting
  }
  
  public void equip()
  {
    craftZoneManager.equip();
  }
  
  public void unequip()
  {
      craftZoneManager.unequip();
  }
  
  
  // Checks wether a device can be put onto the crafting table
  // check that all biobricks are available    
  // checks whether it is a grammatically correct device
  // doesn't check for a device that contains several times the same brick
  public bool isEquipable(Device device)
  { 
    if(null == device)
        return false;
      
    foreach(ExpressionModule module in device.getExpressionModules())
    {
        if(!ExpressionModule.isBioBricksSequenceValid(module.getBioBricks()))
            return false;
             
        foreach(BioBrick brick in module.getBioBricks())
        {
            if(brick.amount < 1)
                return false;
        }
    }
    
    return true;
  }
  
  public bool isEquiped(Device device)
  {
      return (null != device) && Equipment.get().exists (d => d.Equals(device));
  }
  
  public bool isEquiped(string deviceName)
  {
      return (!string.IsNullOrEmpty(deviceName) && Equipment.get().exists (d => d.getInternalName() == deviceName));
  }

  public void setDisplayedDevice(Device device){
    Logger.Log("CraftFinalizer::setDisplayedDevice("+device+")", Logger.Level.TRACE);

    //Inventory.AddingResult addingResult = Inventory.get().canAddDevice(device);
    //string status = statusMessagesDictionary[addingResult];
    //Logger.Log("CraftFinalizer::updateButtonStatus(): addingResult="+addingResult+", status="+status, Logger.Level.TRACE);
    
    bool equiped = isEquiped(device);
    bool equipable = isEquipable(device);
    bool newRecipe = (Inventory.get().canAddDevice(device) == Inventory.AddingResult.SUCCESS);
    
    //CraftFinalizationButton.CraftMode mode = enabled?CraftFinalizationButton.CraftMode.CRAFT:CraftFinalizationButton.CraftMode.UNCRAFT;
    CraftFinalizationButton.CraftMode mode = equiped?CraftFinalizationButton.CraftMode.INACTIVATE:
                        (equipable?CraftFinalizationButton.CraftMode.ACTIVATE:CraftFinalizationButton.CraftMode.NOTHING);
    
    if(null == device)
    {
        mode = CraftFinalizationButton.CraftMode.NOTHING;   
    }    
            
    if(null == craftFinalizationButton)
        craftFinalizationButton = GameObject.Find("CraftButton").GetComponent<CraftFinalizationButton>();
    //craftFinalizationButton.setEnabled(enabled);
    setCraftedStatus(mode);
    //Debug.LogError("new mode="+mode);
    //Logger.Log("CraftFinalizer::setDisplayedDevice(): "+craftFinalizationButton+".setEnabled("+enabled+")", Logger.Level.TRACE);
  }

    public void randomRename() {
        Logger.Log("CraftFinalizer::randomRename", Logger.Level.TRACE);
        Device currentDevice = craftZoneManager.getCurrentDevice();
        string newName = Inventory.get().getAvailableDeviceDisplayedName();
        Device newDevice = Device.buildDevice(newName, currentDevice.getExpressionModules());
        if(newDevice != null){
            Logger.Log("CraftFinalizer::randomRename craftZoneManager.setDevice("+newDevice+")", Logger.Level.TRACE);
            craftZoneManager.setDevice(newDevice);
        } else {
            Logger.Log("CraftFinalizer::randomRename failed Device.buildDevice(name="+newName
                       +", modules="+Logger.ToString<ExpressionModule>(currentDevice.getExpressionModules())+")", Logger.Level.WARN);
        }
    }


}

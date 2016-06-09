using UnityEngine;
using System.Collections.Generic;

public class CraftFinalizer : MonoBehaviour {
  private CraftZoneManager _craftZoneManager;

	public CraftZoneManager ToCraftZoneManager {
			get {
				return _craftZoneManager;
			}
			set {
				_craftZoneManager = value;
			}
		}
  public FinalizationInfoPanelManager   finalizationInfoPanelManager;
  
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
        Debug.LogError("uncraft");
    }

  public void finalizeCraft() {
    //create new device from current biobricks in craft zone
    Logger.Log("CraftFinalizer::finalizeCraft()", Logger.Level.DEBUG);
    Device currentDevice = _craftZoneManager.getCurrentDevice();
    if(currentDevice != null){
        // TODO pipeline when recipe is unknown
      Inventory.AddingResult addingResult = Inventory.get().askAddDevice(currentDevice, true);
      if(addingResult == Inventory.AddingResult.SUCCESS) {
        _craftZoneManager.craft();
      }
      Logger.Log("CraftFinalizer::finalizeCraft(): device="+currentDevice, Logger.Level.TRACE);
    } else {
      Logger.Log("CraftFinalizer::finalizeCraft() failed: invalid device (null)", Logger.Level.WARN);
    }

        //TODO RedMetrics reporting
  }
  
  public void equip()
  {
    _craftZoneManager.equip();
  }
  
  public void unequip()
  {
      _craftZoneManager.unequip();
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
    Debug.LogError("new mode="+mode);
    //Logger.Log("CraftFinalizer::setDisplayedDevice(): "+craftFinalizationButton+".setEnabled("+enabled+")", Logger.Level.TRACE);
    //finalizationInfoPanelManager.setDisplayedDevice(device, status);
    //Logger.Log("CraftFinalizer::setDisplayedDevice(): finalizationInfoPanelManager.setDisplayedDevice("+device+", "+status+")", Logger.Level.TRACE);
  }

    public void randomRename() {
        Logger.Log("CraftFinalizer::randomRename", Logger.Level.TRACE);
        Device currentDevice = _craftZoneManager.getCurrentDevice();
        string newName = Inventory.get().getAvailableDeviceDisplayedName();
        Device newDevice = Device.buildDevice(newName, currentDevice.getExpressionModules());
        if(newDevice != null){
            Logger.Log("CraftFinalizer::randomRename _craftZoneManager.setDevice("+newDevice+")", Logger.Level.TRACE);
            _craftZoneManager.setDevice(newDevice);
        } else {
            Logger.Log("CraftFinalizer::randomRename failed Device.buildDevice(name="+newName
                       +", modules="+Logger.ToString<ExpressionModule>(currentDevice.getExpressionModules())+")", Logger.Level.WARN);
        }
    }

  void Start()
  {
    _craftZoneManager = CraftZoneManager.get();
  }


}

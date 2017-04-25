using UnityEngine;
using System.Collections.Generic;

//TODO adapt script for Device/Molecules pair in new interface
public class DevicesDisplayer : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "DeviceDisplayer";
    private static DevicesDisplayer _instance;
    public static DevicesDisplayer get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("DevicesDisplayer get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<DevicesDisplayer>();
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
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    public bool initializeIfNecessary()
    {
        if (!_initialized)
        {
            if (null != listedInventoryPanel)
            {
                for (int index = 0; index < listedDevicesGrid.childCount; index++)
                {
                    GameObject child = listedDevicesGrid.GetChild(index).gameObject;
                    Destroy(child);
                    // DisplayedDevice device = child.GetComponent<DisplayedDevice>();
                    // if(!_listedInventoriedDevices.Contains(device))
                    // { 
                    //     Destroy(child);
                    // }
                }
                _initialized = true;
            }
        }
        return _initialized;
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public enum TextureQuality
    {
        HIGH,
        NORMAL,
        LOW,
        DEFAULT
    }

    private static TextureQuality textureQuality = TextureQuality.LOW;

    public static TextureQuality getTextureQuality()
    {
        return textureQuality;
    }

    public enum DeviceType
    {
        Equipped = 0,
        Inventoried = 1, // deprecated
        Listed = 2,
        CraftSlot = 3
    }

    private List<DisplayedDevice> _equippedDevices = new List<DisplayedDevice>();
    private List<DisplayedDevice> _inventoriedDevices = new List<DisplayedDevice>();
    private List<DisplayedDevice> _listedInventoriedDevices = new List<DisplayedDevice>();

    //TODO use game object texture dimensions
    private const float _inventoriedWidth = 68.0f;
    private const float _inventoriedHeight = 68.0f;
    private const float _listedInventoriedHeight = 66.0f;
    private const float _listedInventoriedWidth = 66.0f;

    public UIPanel listedInventoryPanel;
    public Transform listedDevicesGrid;

    public GraphMoleculeList graphMoleculeList;

    private GUITransitioner transitioner;
    private CraftZoneManager craftZoneManager;


    /*
     *  ADD
     *
     */

    public void addInventoriedDevice(Device device)
    {
        // Debug.Log(this.GetType() + " addInventoriedDevice(" + device + ") starts with _listedInventoriedDevices=" + Logger.ToString<DisplayedDevice>(_listedInventoriedDevices, d => d._device.getInternalName()));

        if (device == null)
        {
            Debug.LogWarning(this.GetType() + " addInventoriedDevice device==null");
            return;
        }

        bool alreadyInventoried = (_inventoriedDevices.Exists(inventoriedDevice => inventoriedDevice._device == device));
        if (!alreadyInventoried)
        {

            DisplayedDevice newListedDevice =
              ListedDevice.Create(
                listedDevicesGrid
                , Vector3.zero
                , null
                , device
                , this
                , DevicesDisplayer.DeviceType.Listed
              );

            // Debug.Log(this.GetType() + " addInventoriedDevice: newListedDevice=" + newListedDevice.gameObject.name + " " + newListedDevice._device.getInternalName());

            _listedInventoriedDevices.Add(newListedDevice);
            listedDevicesGrid.GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " addInventoriedDevice failed: alreadyInventoried=" + alreadyInventoried);
        }
        // Debug.Log(this.GetType() + " addInventoriedDevice(" + device + ") ends with _listedInventoriedDevices=" + Logger.ToString<DisplayedDevice>(_listedInventoriedDevices));
    }

    public void addEquippedDevice(Device device)
    {
        // Debug.Log(this.GetType() + " addEquippedDevice(" + device.getInternalName() + ")");
        if (device == null)
        {
            Debug.LogWarning(this.GetType() + " addEquippedDevice device == null");
        }
        bool newEquipped = (!_equippedDevices.Exists(equipped => equipped._device == device));
        if (newEquipped)
        {

            DisplayedDevice newDevice = DisplayedDevice.Create(
                                            null,
                                            Vector3.zero,
                                            null,
                                            device,
                                            this,
                                            DevicesDisplayer.DeviceType.Equipped
                                        );

            _equippedDevices.Add(newDevice);

            graphMoleculeList.addDeviceAndMoleculesComponent(newDevice);
        }
        else
        {
            // Debug.Log(this.GetType() + " addDevice failed: alreadyEquipped=" + newEquipped);
        }
    }

    public DeviceContainer.AddingResult askAddEquippedDevice(Device device)
    {
        if (device == null)
        {
            Debug.LogWarning(this.GetType() + " askAddEquippedDevice device==null");
            return DeviceContainer.AddingResult.FAILURE_DEFAULT;
        }
        return Equipment.get().askAddDevice(device);
    }

    public bool IsScreen(int screen)
    {
        safeGetTransitioner();
        return (((transitioner._currentScreen == GUITransitioner.GameScreen.screen1) && (screen == 1))
            || ((transitioner._currentScreen == GUITransitioner.GameScreen.screen2) && (screen == 2))
            || ((transitioner._currentScreen == GUITransitioner.GameScreen.screen3) && (screen == 3)));
    }

    public bool IsEquipScreen()
    {
        return (safeGetTransitioner()._currentScreen == GUITransitioner.GameScreen.screen2);
    }

    private bool _initialized;
    private bool initialized
    {
        get
        {
            _initialized = _initialized || initializeIfNecessary();
            return _initialized;
        }
    }

    public void UpdateScreen()
    {
        // Debug.Log(this.GetType() + " UpdateScreen " + SafeGetTransitioner()._currentScreen);
        if (initialized)
        {
            if (IsScreen(1))
            {
                listedInventoryPanel.gameObject.SetActive(false);
            }
            else if (IsScreen(2))
            {
                listedInventoryPanel.gameObject.SetActive(false);
            }
            else if (IsScreen(3))
            {
                listedInventoryPanel.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning(this.GetType() + " UpdateScreen unknown current screen");
            }
        }
    }

    /*
     *  REMOVE
     *
     */



    public bool askRemoveEquippedDevice(Device device)
    {
        Equipment.get().removeDevice(device);
        return true;
    }

    public void removeDevice(int deviceID)
    {
        DisplayedDevice toRemove = _equippedDevices.Find(device => device.getID() == deviceID);
        List<DisplayedDevice> devices = _equippedDevices;
        DeviceType deviceType = DeviceType.Equipped;
        if (toRemove == null)
        {
            toRemove = _inventoriedDevices.Find(device => device.getID() == deviceID);
            devices = _inventoriedDevices;
            deviceType = DeviceType.Inventoried;
        }
        else
        {
        }

        removeDevice(toRemove, devices, deviceType);
    }

    public void removeEquippedDevice(Device toRemove)
    {
        removeDevice(DevicesDisplayer.DeviceType.Equipped, toRemove);
        graphMoleculeList.removeDeviceAndMoleculesComponent(toRemove);
    }

    public void removeInventoriedDevice(Device toRemove)
    {
        removeDevice(DevicesDisplayer.DeviceType.Inventoried, toRemove);
    }

    public void removeListedDevice(Device toRemove)
    {
        removeDevice(DevicesDisplayer.DeviceType.Listed, toRemove);
    }

    public void setListedDeviceStatus(Device device, bool isEquipped)
    {
        // Debug.Log(this.GetType() + " setListedDeviceStatus");
        if (null != device)
        {
            // Debug.Log(this.GetType() + " setListedDeviceStatus isEquipped=" + isEquipped + " for device " + device.getInternalName());
            ListedDevice found = (ListedDevice)_listedInventoriedDevices.Find(d => d._device.Equals(device));
            if (found != null)
            {
                // Debug.Log(this.GetType() + " setListedDeviceStatus found != null isEquipped=" + isEquipped + " for device " + device.getInternalName());
                found.isEquipped = isEquipped;
            }
        }
    }

    // dev test method
    public void setAllListedStatuses()
    {
        // Debug.Log(this.GetType() + " setAllListedStatuses");
        // string allProcess = this.GetType() + " setAllListedStatuses";
        foreach(DisplayedDevice device in _listedInventoriedDevices)
        {
            // allProcess += "\n    treating " + device._device.getInternalName();
            ListedDevice listed = (ListedDevice)device;
            if (null != listed)
            {
                // allProcess += "\n        null != listed";
                // allProcess += "\n        searching match in _equippedDevices=" + Logger.ToString<DisplayedDevice>(_equippedDevices);
                listed.isEquipped = _equippedDevices.Exists(d => d._device.Equals(listed._device));
                // allProcess += "\n        listed.isEquipped=" + listed.isEquipped;
            }
        }
        // Debug.Log(allProcess);
    }

    public void removeDevice(DevicesDisplayer.DeviceType type, Device toRemove)
    {
        List<DisplayedDevice> devices;
        DisplayedDevice found;
        if (type == DevicesDisplayer.DeviceType.Equipped)
        {
            devices = _equippedDevices;
        }
        else if (type == DevicesDisplayer.DeviceType.Inventoried)
        {
            devices = _inventoriedDevices;
        }
        else if (type == DevicesDisplayer.DeviceType.Listed)
        {
            devices = _listedInventoriedDevices;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " removeDevice unknown type=" + type);
            devices = new List<DisplayedDevice>();
        }
        found = devices.Find(device => device._device.Equals(toRemove));

        if (found != null)
        {
            removeDevice(found, devices, type);
        }
        else
        {
            Debug.LogWarning("removeDevice(type=" + type + ", toRemove=" + toRemove + ") found no matching device");
        }
    }

    private void removeDevice(DisplayedDevice toRemove, List<DisplayedDevice> devices, DeviceType deviceType)
    {
        if (toRemove != null)
        {
            devices.Remove(toRemove);
            toRemove.remove();
        }
    }

    private GUITransitioner safeGetTransitioner()
    {
        if (null == transitioner)
        {
            transitioner = GUITransitioner.get();
        }
        return transitioner;
    }
}

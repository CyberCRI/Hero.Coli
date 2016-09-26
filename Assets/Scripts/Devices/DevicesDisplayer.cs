using UnityEngine;
using System.Collections.Generic;

//TODO adapt script for Device/Molecules pair in new interface
public class DevicesDisplayer : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "DeviceDisplayer";
    private static DevicesDisplayer _instance;
    public static DevicesDisplayer get()
    {
        if (_instance == null)
        {
            Logger.Log("DevicesDisplayer::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<DevicesDisplayer>();
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("DevicesDisplayer::Awake", Logger.Level.DEBUG);
        _instance = this;
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
        Equiped = 0,
        Inventoried = 1, // deprecated
        Listed = 2,
        CraftSlot = 3
    }

    private List<DisplayedDevice> _equipedDevices = new List<DisplayedDevice>();
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


    public bool initialize()
    {
        if (!_initialized)
        {
            if (
                  (null != listedInventoryPanel)
              )
            {

                for (int index = 0; index < listedDevicesGrid.childCount; index++)
                {
                    Destroy(listedDevicesGrid.GetChild(index).gameObject);
                }

                SafeGetTransitioner();

                _initialized = true;
            }
        }
        return _initialized;
    }


    /*
     *  ADD
     *
     */
     
    public void addInventoriedDevice(Device device)
    {
        Debug.Log("DevicesDisplayer::addInventoriedDevice(" + device + ") starts with _listedInventoriedDevices=" + Logger.ToString<DisplayedDevice>(_listedInventoriedDevices));

        if (device == null)
        {
            Debug.LogWarning("DevicesDisplayer::addInventoriedDevice device==null");
            return;
        }

        bool alreadyInventoried = (_inventoriedDevices.Exists(inventoriedDevice => inventoriedDevice._device == device));
        if (!alreadyInventoried)
        {

            Transform parent = listedDevicesGrid;
            Debug.Log("DevicesDisplayer::addInventoriedDevice: parent=" + parent);

            DisplayedDevice newListedDevice =
              ListedDevice.Create(
                parent
                , Vector3.zero
                , null
                , device
                , this
                , DevicesDisplayer.DeviceType.Listed
              );

            Debug.Log("DevicesDisplayer::addInventoriedDevice: newListedDevice=" + newListedDevice);

            _listedInventoriedDevices.Add(newListedDevice);
            listedDevicesGrid.GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            Debug.LogWarning("DevicesDisplayer::addInventoriedDevice failed: alreadyInventoried=" + alreadyInventoried);
        }
        Debug.Log("DevicesDisplayer::addInventoriedDevice(" + device + ") ends with _listedInventoriedDevices=" + Logger.ToString<DisplayedDevice>(_listedInventoriedDevices));
    }

    public void addEquipedDevice(Device device)
    {
        Logger.Log("addEquipedDevice(" + device.ToString() + ")", Logger.Level.TRACE);
        if (device == null)
        {
            Logger.Log("DevicesDisplayer::addEquipedDevice device == null", Logger.Level.WARN);
        }
        bool newEquiped = (!_equipedDevices.Exists(equiped => equiped._device == device));
        if (newEquiped)
        {

            DisplayedDevice newDevice = DisplayedDevice.Create(
                                            null,
                                            Vector3.zero,
                                            null,
                                            device,
                                            this,
                                            DevicesDisplayer.DeviceType.Equiped
                                        );

            _equipedDevices.Add(newDevice);

            graphMoleculeList.addDeviceAndMoleculesComponent(newDevice);
        }
        else
        {
            Logger.Log("addDevice failed: alreadyEquiped=" + newEquiped, Logger.Level.TRACE);
        }
    }

    public DeviceContainer.AddingResult askAddEquipedDevice(Device device)
    {
        if (device == null)
        {
            Logger.Log("DevicesDisplayer::askAddEquipedDevice device==null", Logger.Level.WARN);
            return DeviceContainer.AddingResult.FAILURE_DEFAULT;
        }
        return Equipment.get().askAddDevice(device);
    }

    public bool IsScreen(int screen)
    {
        SafeGetTransitioner();
        return (((transitioner._currentScreen == GUITransitioner.GameScreen.screen1) && (screen == 1))
            || ((transitioner._currentScreen == GUITransitioner.GameScreen.screen2) && (screen == 2))
            || ((transitioner._currentScreen == GUITransitioner.GameScreen.screen3) && (screen == 3)));
    }

    public bool IsEquipScreen()
    {
        return (SafeGetTransitioner()._currentScreen == GUITransitioner.GameScreen.screen2);
    }

    private bool _initialized;
    private bool initialized
    {
        get
        {
            _initialized = _initialized || initialize();
            return _initialized;
        }
    }

    public void UpdateScreen()
    {
        Logger.Log("DevicesDisplayer::UpdateScreen " + SafeGetTransitioner()._currentScreen, Logger.Level.TRACE);
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
                Logger.Log("DevicesDisplayer::UpdateScreen unknown current screen", Logger.Level.WARN);
            }
        }
    }

    /*
     *  REMOVE
     *
     */



    public bool askRemoveEquipedDevice(Device device)
    {
        Equipment.get().removeDevice(device);
        return true;
    }

    public void removeDevice(int deviceID)
    {
        DisplayedDevice toRemove = _equipedDevices.Find(device => device.getID() == deviceID);
        List<DisplayedDevice> devices = _equipedDevices;
        DeviceType deviceType = DeviceType.Equiped;
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
        removeDevice(DevicesDisplayer.DeviceType.Equiped, toRemove);
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

    public void removeDevice(DevicesDisplayer.DeviceType type, Device toRemove)
    {
        List<DisplayedDevice> devices;
        DisplayedDevice found;
        if (type == DevicesDisplayer.DeviceType.Equiped)
        {
            devices = _equipedDevices;
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
            Logger.Log("DevicesDisplayer::removeDevice unknown type=" + type, Logger.Level.WARN);
            devices = new List<DisplayedDevice>();
        }
        found = devices.Find(device => device._device.Equals(toRemove));

        if (found != null)
        {
            removeDevice(found, devices, type);
        }
        else
        {
            Logger.Log("removeDevice(type=" + type + ", toRemove=" + toRemove + ") found no matching device", Logger.Level.WARN);
        }
    }

    private void removeDevice(DisplayedDevice toRemove, List<DisplayedDevice> devices, DeviceType deviceType)
    {
        if (toRemove != null)
        {
            devices.Remove(toRemove);
            toRemove.Remove();
        }
    }

    private GUITransitioner SafeGetTransitioner()
    {
        if (null == transitioner)
        {
            transitioner = GUITransitioner.get();
        }
        return transitioner;
    }

    void Start()
    {
        Logger.Log("DevicesDisplayer::Start()", Logger.Level.DEBUG);

        initialize();
    }
}

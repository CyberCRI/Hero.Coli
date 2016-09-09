using UnityEngine;
using System.Collections.Generic;

public class Inventory : DeviceContainer
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "DeviceInventory";
    private static Inventory _instance;
    public static Inventory get()
    {
        if (_instance == null)
        {
            Logger.Log("Inventory::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<Inventory>();
        }
        return _instance;
    }

    public static bool isOpenable()
    {
        return 0 != _instance._devices.Count;
    }

    void Awake()
    {
        Logger.Log("Inventory::Awake", Logger.Level.DEBUG);
        _deviceAdded = false;
        _instance = this;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    /* array of file paths from which the devices available by default from start will be loaded,
     * provided that the required BioBricks are available - cf. AvailableBioBricksManager
     */
    //private string[] _deviceFiles = new string[]{};
    public string[] deviceFiles;
    public string deviceFilesPathPrefix;
    //private string[] _deviceFiles = new string[]{ "Parameters/Devices/available", Inventory._saveFilePathRead };

    //old device files
    //private string[] _deviceFiles = new string[]{ "Assets/Data/devices"};
    //private string[] _deviceFiles = new string[]{ "Assets/Data/raph/devices", Inventory._saveFilePathRead };
    //private string[] _deviceFiles = new string[]{ "Assets/Data/raph/repressilatorDevices", Inventory._saveFilePathRead };

    private static string _saveFilePathRead = "Parameters/Devices/exported.xml";
    private static string _saveFilePathWrite = "Assets/Resources/" + _saveFilePathRead;

    private string _genericDeviceNamePrefix = "device";

    private bool _deviceAdded;

    public bool getDeviceAdded() { return _deviceAdded; }
    public void setDeviceAdded(bool b) { _deviceAdded = b; }


    private static string[] proteinsIn = new string[]{
    "Collagen",
    "Actin",
    "Keratin",
    "Elastin"
  };
    private static string[] proteinsOut = new string[]{
    "Cadherin",
    "Ependymin",
    "Integrin",
    "Selectin"
  };

    private static float getTestBeta() { return Random.Range(0.0f, 10.0f); }
    private static string getTestFormula()
    {
        string result = "";
        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            result += "!";
        }
        result += "[";
        result += (0.1f + Random.Range(0, 8) / 10);
        result += ",";
        result += Random.Range(0.1f, 3.0f);
        result += "]";
        result += proteinsIn[Random.Range(0, proteinsIn.Length - 1)];
        return result;
    }
    private static float getTestRBS() { return Random.Range(0.9f, 1.1f); }
    private static string getTestProtein() { return proteinsOut[Random.Range(0, proteinsOut.Length)]; }
    private static float getTestTerminator() { return Random.Range(0.9f, 1.1f); }

    private Device getTestDevice(string name)
    {
        Logger.Log("Inventory::getTestDevice()", Logger.Level.TRACE);
        Device testDevice = Device.buildDevice(
        name,
        getTestBeta(),
        getTestFormula(),
        getTestRBS(),
        getTestProtein(),
        getTestTerminator()
        );
        Logger.Log("Inventory::getTestDevice() testDevice=" + testDevice, Logger.Level.TRACE);
        return testDevice;
    }

    private List<Device> getTestDevices()
    {
        Logger.Log("Inventory::getTestDevices()", Logger.Level.TRACE);
        List<Device> result = new List<Device>();
        result.Add(getTestDevice("MyDevice"));
        result.Add(getTestDevice("MonDispositif"));
        result.Add(getTestDevice("MeinGeraet"));
        Logger.Log("Inventory::getTestDevices() result=" + Logger.ToString<Device>(result), Logger.Level.TRACE);
        return result;
    }

    public Inventory()
    {
        Logger.Log("Inventory() done", Logger.Level.TRACE);
    }

    private void addDevice(Device device)
    {
        Logger.Log("Inventory::addDevice(" + device + "), count before=" + _devices.Count, Logger.Level.TRACE);
        Device copy = Device.buildDevice(device);
        if (device == null)
        {
            Logger.Log("Inventory::addDevice device==null", Logger.Level.WARN);
            return;
        }

        string displayerString = _displayer != null ? "name=" + _displayer.name : "null";
        Logger.Log("Inventory::addDevice(" + device + ")"
          + ", copy=" + copy
          + ", count before=" + _devices.Count
          + ", _devices=" + _devices
          + ", _displayer=" + displayerString
          , Logger.Level.TRACE
          );
        _devices.Add(copy);
        Logger.Log("Inventory::addDevice _devices.Add(copy); done", Logger.Level.TRACE);
        _displayer.addInventoriedDevice(copy);
        Logger.Log("Inventory::addDevice(" + device + "), count after=" + _devices.Count, Logger.Level.TRACE);
    }

    public AddingResult canAddDevice(Device device)
    {
        Logger.Log("Inventory::canAddDevice(" + device + ") with _devices=" + Logger.ToString<Device>(_devices), Logger.Level.TRACE);

        if (device == null)
        {
            Logger.Log("Inventory::canAddDevice: device is null: AddingResult.FAILURE_DEFAULT", Logger.Level.WARN);
            return AddingResult.FAILURE_DEFAULT;
        }
        else
        {
            //TODO test BioBricks equality (cf next line)
            if (_devices.Exists(d => d.Equals(device)))
            //if (_devices.Exists(d => d.getInternalName() == device.getInternalName()))
            {
                if (_devices.Exists(d => d.hasSameBricks(device)))
                {
                    Logger.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_DEVICE", Logger.Level.TRACE);
                    return AddingResult.FAILURE_SAME_DEVICE;
                }
                else
                {
                    Logger.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_NAME", Logger.Level.TRACE);
                    return AddingResult.FAILURE_SAME_NAME;
                }
            }
            else if (_devices.Exists(d => d.hasSameBricks(device)))
            {
                Logger.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_BRICKS", Logger.Level.TRACE);
                return AddingResult.FAILURE_SAME_BRICKS;
            }
            else
            {
                Logger.Log("Inventory::canAddDevice: AddingResult.SUCCESS", Logger.Level.TRACE);
                return AddingResult.SUCCESS;
            }
        }
    }

    public override AddingResult askAddDevice(Device device, bool reportToRedMetrics = false)
    {
        Logger.Log("Inventory::askAddDevice", Logger.Level.TRACE);
        AddingResult addingResult = canAddDevice(device);
        if (addingResult == AddingResult.SUCCESS)
        {
            Logger.Log("Inventory::askAddDevice: AddingResult.SUCCESS, will add device=" + device, Logger.Level.INFO);
            addDevice(device);

            if (reportToRedMetrics)
            {
                RedMetricsManager.get().sendEvent(TrackingEvent.CRAFT, new CustomData(CustomDataTag.DEVICE, device.getInternalName()));
            }

            //uncomment to save user-created devices
            //TODO FIXME uncommenting this entails bugs on loading devices from _saveFilePath
            //DeviceSaver dSaver = new DeviceSaver();
            //dSaver.saveDevicesToFile(_devices, _saveFilePathWrite);
        }
        else
        {
            Logger.Log("Inventory::askAddDevice: " + addingResult + ", didn't add device=" + device, Logger.Level.INFO);
        }
        return addingResult;
    }

    public override void removeDevice(Device device)
    {
        _devices.Remove(device);
        safeGetDisplayer().removeInventoriedDevice(device);
        safeGetDisplayer().removeListedDevice(device);
    }

    public override void removeDevices(List<Device> toRemove)
    {
        Logger.Log("Inventory::removeDevices", Logger.Level.INFO);

        foreach (Device device in toRemove)
        {
            safeGetDisplayer().removeInventoriedDevice(device);
        }
        _devices.RemoveAll((Device obj) => toRemove.Contains(obj));
    }

    public override void editDevice(Device device)
    {
        //TODO
        Debug.Log("Inventory::editeDevice NOT IMPLEMENTED");
    }

    //
    public string getAvailableDeviceDisplayedName()
    {
        Logger.Log("Inventory::getAvailableDeviceDisplayedName()", Logger.Level.TRACE);
        bool taken;
        string currentName;
        int number = _devices.Count;
        do
        {
            currentName = _genericDeviceNamePrefix + number;
            taken = _devices.Exists(d => (d.displayedName == currentName));
            if (taken)
            {
                number++;
            }
            else
            {
                return currentName;
            }
        } while (taken);
        Logger.Log("Inventory::getAvailableDeviceDisplayedName() returns " + currentName, Logger.Level.TRACE);
        return currentName;
    }

    // Warning: loads from inputFiles is an array of names of files inside 'biobrickFilesPathPrefix'
    void loadDevices()
    {
        LinkedList<BioBrick> availableBioBricks = AvailableBioBricksManager.get().getAvailableBioBricks();
        LinkedList<BioBrick> allBioBricks = AvailableBioBricksManager.get().getAllBioBricks();

        LevelInfo levelInfo = null;
        MemoryManager.get().tryGetCurrentLevelInfo(out levelInfo);

        List<Device> devices = new List<Device>();

        DeviceLoader dLoader = new DeviceLoader(availableBioBricks, allBioBricks);

        string[] filesToLoad;
        string currentMapDevicesFilePath = MemoryManager.get().configuration.getGameMapName();

        if (null == levelInfo || !levelInfo.areAllDevicesAvailable)
        {
            filesToLoad = new string[] { currentMapDevicesFilePath };
        }
        else
        {
            List<string> fileList = new List<string>(deviceFiles);
            fileList.Add(currentMapDevicesFilePath);
            filesToLoad = fileList.ToArray();
        }
        foreach (string file in filesToLoad)
        {
            string fullPathFile = deviceFilesPathPrefix + file;
            Logger.Log("Inventory::loadDevices loads device file " + fullPathFile, Logger.Level.TRACE);
            devices.AddRange(dLoader.loadDevicesFromFile(fullPathFile));
        }
        UpdateData(devices, new List<Device>(), new List<Device>());
    }

    public void switchDeviceKnowledge()
    {
        if (0 == _devices.Count)
        {
            Logger.Log("Inventory::switchDeviceKnowledge calls loadDevices", Logger.Level.DEBUG);
            loadDevices();
        }
        else
        {
            Logger.Log("Inventory::switchDeviceKnowledge calls forgetDevices", Logger.Level.DEBUG);
            forgetDevices();
        }
    }

    private void forgetDevices()
    {
        Logger.Log("Inventory::forgetDevices calls inventory.UpdateData(List(), " + Logger.ToString<Device>(_devices) + "), List())", Logger.Level.INFO);
        foreach (Device device in _devices)
        {
            //ask Equipment instead
            _displayer.askRemoveEquipedDevice(device);
            //ask Crafting instead
            _displayer.removeListedDevice(device);
            //ok
            _displayer.removeInventoriedDevice(device);
        }
        UpdateData(new List<Device>(), _devices, new List<Device>());
    }

    protected override void Start()
    {
        base.Start();
        Logger.Log("Inventory::Start()", Logger.Level.DEBUG);
        loadDevices();
    }
}

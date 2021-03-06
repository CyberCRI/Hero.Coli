using UnityEngine;
using System.Collections.Generic;

public class Equipment : DeviceContainer
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "DeviceEquipment";
    private static Equipment _instance;
    public static Equipment get()
    {
        if (_instance == null)
        {
            // Debug.LogWarning("Equipment get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<Equipment>();
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

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            _initialized = true;
        }
    }

    new void Start()
    {
        base.Start();
        // Debug.Log(this.GetType() + " Start()");
        _reactionEngine = ReactionEngine.get();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private ReactionEngine _reactionEngine;
    public int _characterMediumID = 1;

    public Equipment()
    {
        //by default, nothing's equipped
        _devices = new List<Device>();
    }

    private void addToReactionEngine(Device device)
    {
        // Debug.Log(this.GetType() + " addToReactionEngine reactions from device "+device.getInternalName()+" ("+device.ToString ()+")");

        LinkedList<Reaction> reactions = device.getReactions();
        // Debug.Log(this.GetType() + " addToReactionEngine reactions="+Logger.ToString<IReaction>(reactions)+" from "+device);

        foreach (Reaction reaction in reactions)
        {
            // Debug.Log(this.GetType() + " addToReactionEngine adding reaction="+reaction);
            _reactionEngine.addReactionToMedium(_characterMediumID, reaction);
        }
    }

    public override AddingResult askAddDevice(Device device, bool reportToRedMetrics = false)
    {
        if (device == null)
        {
            // Debug.Log(this.GetType() + " askAddDevice device == null");
            return AddingResult.FAILURE_DEFAULT;
        }
        Device copy = Device.buildDevice(device);
        if (copy == null)
        {
            // Debug.Log(this.GetType() + " askAddDevice copy == null");
            return AddingResult.FAILURE_DEFAULT;
        }

        //TODO test BioBricks equality (cf next line)
        if (_devices.Exists(d => d.Equals(copy)))
        //if(_devices.Exists(d => d.getInternalName() == copy.getInternalName()))
        {
            // Debug.Log(this.GetType() + " askAddDevice device already present");
            return AddingResult.FAILURE_SAME_DEVICE;
        }

        _devices.Add(copy);
        // Debug.Log(this.GetType() + " added " + device.getInternalName());
        safeGetDisplayer().addEquippedDevice(copy);
        // TODO replace by event broadcasting
        if (!PhenoAmpicillinProducer.get().isAmpicillinDeviceEquipped(device))
        {
            addToReactionEngine(copy);
        }
        return AddingResult.SUCCESS;
    }

    //TODO
    private void removeFromReactionEngine(Device device)
    {
        // Debug.Log(this.GetType() + " removeFromReactionEngine reactions from device "+device);

        LinkedList<Reaction> reactions = device.getReactions();
        // Debug.Log(this.GetType() + " removeFromReactionEngine device implies reactions="+Logger.ToString<IReaction>(reactions));

        //LinkedList<Medium> mediums = _reactionEngine.getMediumList();
        //Medium characterMedium = ReactionEngine.getMediumFromId(_characterMediumID, mediums);

        //LinkedList<IReaction> characterReactions = characterMedium.getReactions();
        // Debug.Log(this.GetType() + " removeFromReactionEngine initialcharacterReactions="+Logger.ToString<IReaction>(characterReactions)
        //  );

        foreach (Reaction reaction in reactions)
        {
            // Debug.Log(this.GetType() + " removeFromReactionEngine removing reaction="+reaction);
            _reactionEngine.removeReaction(_characterMediumID, reaction, false);
        }

        //characterReactions = characterMedium.getReactions();
        // Debug.Log(this.GetType() + " removeFromReactionEngine finalcharacterReactions="+Logger.ToString<IReaction>(characterReactions)
        //  );
    }

    public override void removeDevice(Device device)
    {
        // Debug.Log(this.GetType() + " removeDevice("+device+") start");
        if (_devices.Contains(device))
        {
            _devices.RemoveAll(d => d.Equals(device));
            safeGetDisplayer().removeEquippedDevice(device);
            removeFromReactionEngine(device);
            CraftZoneManager.get().unequip(device);
            PhenoAmpicillinProducer.get().onUnequippedDevice(device);
        }
    }

    public void removeDevice(Device device, bool removeBricks)
    {
        removeDevice(device);
    }

    // not optimized
    public override void removeDevices(List<Device> toRemove)
    {
        // Debug.Log(this.GetType() + " removeDevices");

        foreach (Device device in toRemove)
        {
            removeDevice(device);
        }
    }

    public override void editDevice(Device device)
    {
        //TODO
        Debug.LogError(this.GetType() + " editeDevice NOT IMPLEMENTED");
    }

    public static string getInternalDevicesString()
    {
        return Logger.ToString<Device>(_instance._devices, d => d.getInternalName());
    }

    public override string ToString()
    {
        string res = "";
        foreach (Device d in _devices)
        {
            if (res != "")
            {
                res = res + "; ";
            }
            res = res + d.ToString();
        }
        res = "[Equipment " + res + "]";
        return res;
    }
}


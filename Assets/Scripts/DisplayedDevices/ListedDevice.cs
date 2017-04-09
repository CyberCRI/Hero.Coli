using UnityEngine;

class ListedDevice : DisplayedDevice
{
    private const string _deviceStatusStem = "CRAFT.LISTEDDEVICE.";
    private const string _deviceStatusEquippedKey = _deviceStatusStem + "EQUIPPED";
    private const string _deviceStatusUnequippedKey = _deviceStatusStem + "UNEQUIPPED";
    private const float _alphaEquipped = 1.0f;
    private const float _alphaUnequipped = 0.2f;

    [SerializeField]
    private UILocalize _status;

    private bool _displayFeedback = true;
    private bool _isEquipped = false;
    public bool isEquipped
    {
        get
        {
            return _isEquipped;
        }
        set
        {
            _isEquipped = value;
            // Debug.Log(this.GetType() + " isEquipped set to " + value);
            _status.key = value ? _deviceStatusEquippedKey : _deviceStatusUnequippedKey;
            _status.Localize();
            deviceBackgroundSprite.alpha = value ? _alphaEquipped : _alphaUnequipped;
        }
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake isEquipped=" + isEquipped);
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start isEquipped=" + isEquipped);
        if (null != _device)
        {
            // Debug.Log(this.GetType() + " Start\nis this._device=" + this._device.getInternalName() + " in " + Logger.ToString<Device>(Equipment.get().devices) + "?");
            isEquipped = Equipment.get().contains(this._device);
        }
        else
        {
            isEquipped = isEquipped;
        }

        if (_displayFeedback)
        {
            playFeedback();
            _displayFeedback = false;
        }
    }

    public override void OnPress(bool isPressed)
    {
        if (CraftZoneManager.isDeviceEditionOn())
        {
            if (isPressed)
            {
                // Debug.Log(this.GetType() + " OnPress()");
                CraftZoneManager.get().setDevice(_device);
                RedMetricsManager.get().sendRichEvent(TrackingEvent.ADD, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
            }
        }
    }

    protected override void OnHover(bool isOver)
    {
        base.OnHover(isOver);
    }

    // protected override void OnDestroy()
    // {
    //     base.OnDestroy();
    //     // Debug.Log(this.gameObject.name + " gets destroyed");
    // }
}
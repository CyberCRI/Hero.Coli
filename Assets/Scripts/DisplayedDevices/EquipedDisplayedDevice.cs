using UnityEngine;
using System.Collections.Generic;

public class EquipedDisplayedDevice : DisplayedDevice
{
    private LinkedList<GenericDisplayedBioBrick> _currentDisplayedBricks = new LinkedList<GenericDisplayedBioBrick>();

    private static GameObject equipedDevice;
    private static GameObject tinyBioBrickIcon;
    private static GameObject tinyBioBrickIcon2;
    private static float _tinyIconVerticalShift = 0.0f;
    private static float _width = 0.0f;

    private const string _equipedDeviceButtonPrefabPosString = "EquipedDeviceButtonPrefabPos";
    private const string _tinyBioBrickPosString = "TinyBioBrickIconPrefabPos";
    private const string _tinyBioBrickPosString2 = _tinyBioBrickPosString + "2";

    private bool _displayBricks;
    private bool _isEquipScreen;


    public EquipedDeviceCloseButton closeButton;

    private bool _initialized;

    void OnEnable()
    {
        // Debug.Log(this.GetType() + " OnEnable " + _device);

        //quick fix to remove closeButton
        //TODO fixme
        closeButton = null;

        createBioBricksIfNecessary();
        updateVisibility();
    }

    void OnDisable()
    {
        // Debug.Log(this.GetType() + " OnDisable " + _device);
        setBricksVisibilityTo(false);
    }

    protected void setBricksVisibilityTo(bool visible)
    {
        foreach (GenericDisplayedBioBrick brick in _currentDisplayedBricks)
        {
            brick.gameObject.SetActive(visible);
        }
    }

    protected void updateVisibility()
    {
        setBricksVisibilityTo(_displayBricks);
    }

    public override void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            // Debug.Log(this.GetType() + " OnPress() " + getDebugInfos());
            if (_device == null)
            {
                Debug.LogWarning(this.GetType() + " OnPress _device == null");
                return;
            }
            if (_devicesDisplayer.IsEquipScreen())
            {
                if (askRemoveDevice())
                {
                    RedMetricsManager.get().sendEvent(TrackingEvent.UNEQUIP, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
                }
            }
        }
    }

    public bool askRemoveDevice()
    {
        TooltipManager.displayTooltip();
        return _devicesDisplayer.askRemoveEquipedDevice(_device);
    }

    void initIfNecessary()
    {
        // Debug.Log(this.GetType() + " initIfNecessary starts");

        if (!_initialized)
        {
            if (
                (null == equipedDevice)
                || (null == tinyBioBrickIcon)
                || (null == tinyBioBrickIcon2)
                || (0 == _tinyIconVerticalShift)
                || (0 == _width)
              )
            {
                // equipedDevice = DevicesDisplayer.get().equipedDevice;

                // tinyBioBrickIcon = InterfaceLinkManager.get().tinyBioBrickIconPrefabPos;
                // tinyBioBrickIcon2 = InterfaceLinkManager.get().tinyBioBrickIconPrefabPos2;
            }

            if ((null != tinyBioBrickIcon) && (null != equipedDevice) && (null != tinyBioBrickIcon2))
            {
                _tinyIconVerticalShift = (tinyBioBrickIcon.transform.localPosition - equipedDevice.transform.localPosition).y;
                _width = tinyBioBrickIcon2.transform.localPosition.x - tinyBioBrickIcon.transform.localPosition.x;
                tinyBioBrickIcon.SetActive(false);
                tinyBioBrickIcon2.SetActive(false);
                _initialized = true;
                // Debug.Log(this.GetType() + " initIfNecessary ends");
            }
        }
    }

    void createBioBricksIfNecessary()
    {
        // Debug.Log(this.GetType() + " createBioBricksIfNecessary");
        initIfNecessary();
        if (0 == _currentDisplayedBricks.Count)
        {
            if (_device != null)
            {
                //add biobricks
                int index = 0;
                foreach (ExpressionModule module in _device.getExpressionModules())
                {
                    foreach (BioBrick brick in module.getBioBricks())
                    {
                        GenericDisplayedBioBrick dbbrick = TinyBioBrickIcon.Create(transform, getNewPosition(index), null, brick);
                        _currentDisplayedBricks.AddLast(dbbrick);
                        index++;
                    }
                }
            }
            else
            {
                Debug.LogWarning(this.GetType() + " createBioBricksIfNecessary _device == null");
            }
        }
    }

    //needs tinyBioBrickIcon to be initialized, e.g. using initIfNecessary()
    private Vector3 getNewPosition(int index)
    {
        Vector3 shiftPos = new Vector3(index * _width, _tinyIconVerticalShift, -1.0f);
        if (tinyBioBrickIcon == null)
        {
            Debug.LogWarning(this.GetType() + " getNewPosition tinyBioBrickIcon == null");
            return new Vector3(index * _width, -95.0f, -0.1f) + shiftPos;
        }
        else
        {
            //return tinyBioBrickIcon.transform.localPosition + shiftPos;
            return shiftPos;
        }
    }

    public void setDisplayBricks(bool display)
    {
        _displayBricks = display;
        updateVisibility();
    }

    protected override void OnHover(bool isOver)
    {
        // Debug.Log(this.GetType() + " OnHover(" + isOver + ") with _device=" + _device);
        base.OnHover(isOver);

        if (null != closeButton && !_devicesDisplayer.IsEquipScreen())
        {
            //TODO fix interaction with Update
            closeButton.gameObject.SetActive(isOver);
        }
    }
    
    void Update()
    {
        if (_initialized)
        {
            bool newIsEquipScreen = _devicesDisplayer.IsEquipScreen();

            if (_isEquipScreen && _isEquipScreen != newIsEquipScreen)
            {
                closeButton.gameObject.SetActive(false);
            }
            else
            {
                if (!_isEquipScreen && _isEquipScreen != newIsEquipScreen)
                {
                    //TODO fix interaction with OnHover
                    closeButton.gameObject.SetActive(true);
                }
            }
            _isEquipScreen = newIsEquipScreen;
        }
        else
        {
            initIfNecessary();
        }
        //no-hover version
        //closeButton.gameObject.SetActive(_devicesDisplayer.IsEquipScreen());    
    }

    // Use this for initialization
    void Start()
    {
        Debug.LogWarning("EquipedDisplayedDevice Starts");
        // Debug.Log(this.GetType() + " Start");
        createBioBricksIfNecessary();
        updateVisibility();
    }
}
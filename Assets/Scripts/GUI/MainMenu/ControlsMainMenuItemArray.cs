using UnityEngine;

public class ControlsMainMenuItemArray : MainMenuItemArray
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "ControlsItems";
    private static ControlsMainMenuItemArray _instance;
    public static ControlsMainMenuItemArray get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("ControlsMainMenuItemArray::get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<ControlsMainMenuItemArray>();
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

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public GameObject cellControlPanel;
    public GameObject selectedKeyboardControlSprite;
    public GameObject selectedMouseControlSprite;
    public CellControl cellControl;

    private void updateIconsPosition(bool active)
    {
        ControlMainMenuItem cmmi;
        foreach (MainMenuItem item in _items)
        {
            cmmi = item as ControlMainMenuItem;
            if (null != cmmi)
            {
                cmmi.resetIcon(active);
            }
        }
    }

    void OnEnable()
    {
        //hide control selection sprites before moving them
        selectedKeyboardControlSprite.SetActive(false);
        selectedMouseControlSprite.SetActive(false);

        //move control sprites
        updateIconsPosition(true);

        //show control sprites
        cellControlPanel.SetActive(true);

        //select control
        cellControl.refreshControlType();

        //show control selection sprites
        selectedKeyboardControlSprite.SetActive(true);

        selectedMouseControlSprite.SetActive(true);

    }

    void OnDisable()
    {
        cellControlPanel.SetActive(false);
        selectedKeyboardControlSprite.SetActive(false);
        selectedMouseControlSprite.SetActive(false);
    }


    public void switchControlTypeToAbsoluteWASD()
    {
        RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.CONTROLS, CellControl.ControlType.AbsoluteWASD.ToString()));
        cellControl.switchControlTypeToAbsoluteWASD();
    }

    public void switchControlTypeToRelativeWASD()
    {
        RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.CONTROLS, CellControl.ControlType.RelativeWASD.ToString()));
        cellControl.switchControlTypeToRelativeWASD();
    }

    public void switchControlTypeToLeftClickToMove()
    {
        RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.CONTROLS, CellControl.ControlType.LeftClickToMove.ToString()));
        cellControl.switchControlTypeToLeftClickToMove();
    }

    public void switchControlTypeToRightClickToMove()
    {
        RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.CONTROLS, CellControl.ControlType.RightClickToMove.ToString()));
        cellControl.switchControlTypeToRightClickToMove();
    }
}

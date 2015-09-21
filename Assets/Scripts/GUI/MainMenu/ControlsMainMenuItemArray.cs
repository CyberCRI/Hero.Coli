using UnityEngine;
using System.Collections;

public class ControlsMainMenuItemArray : MainMenuItemArray {    
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "ControlsItems";
    private static ControlsMainMenuItemArray _instance;
    public static ControlsMainMenuItemArray get() {
        if(_instance == null) {
            Logger.Log("ControlsMainMenuItemArray::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<ControlsMainMenuItemArray>();
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("ControlsMainMenuItemArray::Awake", Logger.Level.DEBUG);
        _instance = this;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public GameObject cellControlPanel;
    public GameObject selectedKeyboardControlSprite;
    public GameObject selectedMouseControlSprite;
    public CellControl cellControl;

    private void updateIconsPosition(bool active)
    {
        ControlMainMenuItem cmmi;
        foreach(MainMenuItem item in _items) {
            cmmi = item as ControlMainMenuItem;
            if(null != cmmi) {
                cmmi.resetIcon(active);
            }
        }
    }

    void OnEnable ()
    {
        //hide control selection sprites before moving them
        selectedKeyboardControlSprite.SetActive(false);
        selectedMouseControlSprite.SetActive(false);

        //move control sprites
        updateIconsPosition(true);

        //show control sprites
        cellControlPanel.SetActive(true);

        //select control
        cellControl.refreshControlType ();

        //show control selection sprites
        selectedKeyboardControlSprite.SetActive(true);

        //if(!Application.isWebPlayer) {
        selectedMouseControlSprite.SetActive(true);
        //}

    }

    void OnDisable ()
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

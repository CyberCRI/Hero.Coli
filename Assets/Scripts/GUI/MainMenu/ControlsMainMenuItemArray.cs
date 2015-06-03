using UnityEngine;
using System.Collections;

public class ControlsMainMenuItemArray : MainMenuItemArray {

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
        selectedMouseControlSprite.SetActive(true);
    }

    void OnDisable ()
    {
        cellControlPanel.SetActive(false);
        selectedKeyboardControlSprite.SetActive(false);
        selectedMouseControlSprite.SetActive(false);
    }
    
    public void switchControlTypeToAbsoluteWASD()
    {
        cellControl.switchControlTypeToAbsoluteWASD();
    }
    
    public void switchControlTypeToRelativeWASD()
    {
        cellControl.switchControlTypeToRelativeWASD();
    }
    
    public void switchControlTypeToLeftClickToMove()
    {
        cellControl.switchControlTypeToLeftClickToMove();
    }
    
    public void switchControlTypeToRightClickToMove()
    {
        cellControl.switchControlTypeToRightClickToMove();
    }
}

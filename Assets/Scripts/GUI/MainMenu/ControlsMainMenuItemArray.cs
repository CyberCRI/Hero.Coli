using UnityEngine;
using System.Collections;

public class ControlsMainMenuItemArray : MainMenuItemArray {

    public GameObject cellControlPanel;
    public GameObject selectedKeyboardControlSprite;
    public GameObject selectedMouseControlSprite;
    public CellControl cellControl;

    void OnEnable ()
    {
        Debug.LogError("Controls are visible");
        cellControl.refreshControlType ();
        cellControlPanel.SetActive(true);
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

using UnityEngine;
using System.Collections;

public class ControlMainMenuItem : MainMenuItem {

    public Vector3 offset;
    public GameObject controlIcon;
    public ControlsMainMenuItemArray controlsArray;

    public void resetIcon(bool active)
    {
        string iconName = (null != controlIcon)?controlIcon.name:"(None)";
        if (null != controlIcon) {
            controlIcon.transform.position = this.gameObject.transform.position + offset;
            controlIcon.gameObject.SetActive(active);
        }
    }
}

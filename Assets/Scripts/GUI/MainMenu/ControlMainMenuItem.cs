using UnityEngine;
using System.Collections;

public class ControlMainMenuItem : MainMenuItem {

    public Vector3 offset;
    public GameObject controlIcon;
    public ControlsMainMenuItemArray controlsArray;

    void Update ()
    {
        //TODO remove
        if (null != controlIcon) {
            controlIcon.transform.position = this.gameObject.transform.position + offset;
        }
    }

    void OnEnable ()
    {
        if (null != controlIcon) {
            controlIcon.transform.position = this.gameObject.transform.position + offset;
            controlIcon.gameObject.SetActive(true);
        }
    }

    void OnDisable ()
    {
        if (null != controlIcon) {
            controlIcon.SetActive(false);
        }
    }
}

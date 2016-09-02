using UnityEngine;
using System.Collections;

public class MouseControlMainMenuItem : ControlMainMenuItem {    
    public override void initialize() {
        base.initialize();
        /*
        if(Application.platform == RuntimePlatform.WebGLPlayer) {
            resetIcon(false);
            displayed = false;
            MainMenuManager.get ().redraw ();
        }
        */
    }
}

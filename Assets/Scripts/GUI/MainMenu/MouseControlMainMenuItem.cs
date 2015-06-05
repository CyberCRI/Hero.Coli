using UnityEngine;
using System.Collections;

public class MouseControlMainMenuItem : ControlMainMenuItem {    
    public override void initialize() {
        base.initialize();
        //*/
        if(true) {
        /*/
        if(Application.isWebPlayer) {
        //*/
            resetIcon(false);
            gameObject.SetActive(false);
        }
    }
}

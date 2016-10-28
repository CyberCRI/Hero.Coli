using UnityEngine;
using System.Collections;

public class EndRestartButton : ModalButton
{
    public override void press ()
    {
        // Debug.Log(this.GetType() + " press()");

        //TODO manage stack of modal elements in ModalManager
        ModalManager.unsetModal ();
        ModalManager.setModal ("EndRestartGame");
    }
}

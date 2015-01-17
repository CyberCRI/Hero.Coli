using UnityEngine;
using System.Collections;

public class EndRestartButton : ModalButton
{
    public override void press ()
    {
        Logger.Log ("EndRestartButton::press()", Logger.Level.INFO);

        //TODO manage stack of modal elements in ModalManager
        ModalManager.unsetModal ();
        ModalManager.setModal ("EndRestartGame");
    }
}

using UnityEngine;
using System.Collections;

//ModalButton inheritor that quits modal window when isPressed is true,
//contrary to CancelModal
public class QuitModalWindow : ModalButton
{
    public override void press ()
    {
        Logger.Log ("QuitModalWindow::press()", Logger.Level.INFO);
        GameStateController.get ().tryUnlockPause ();
        ModalManager.unsetModal ();
    }
}

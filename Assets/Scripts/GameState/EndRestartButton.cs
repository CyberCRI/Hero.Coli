using UnityEngine;
using System.Collections;

public class EndRestartButton : ModalButton
{
    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("EndRestartButton::OnPress()", Logger.Level.INFO);

            //TODO manage stack of modal elements in ModalManager
            ModalManager.unsetModal ();
            ModalManager.setModal ("EndRestartGame");
        }
    }
}

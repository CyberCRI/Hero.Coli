using UnityEngine;
using System.Collections;

public class BackToEndPanelButton : ModalButton
{
    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("BackToEndPanelButton::OnPress()", Logger.Level.INFO);
            GameStateController gsc = GameStateController.get ();
            ModalManager.setModal(gsc.end, true, gsc.endRestartButton.gameObject, gsc.endRestartButton.GetType().Name);
        }
        //FIXME usefulness?
        base.OnPress (isPressed);
    }
}

using UnityEngine;
using System.Collections;

public class BackToEndPanelButton : ModalButton
{
    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("BackToEndPanelButton::OnPress()", Logger.Level.INFO);
            Debug.LogWarning("BackToEndPanelButton::OnPress");
            GameStateController gsc = GameStateController.get ();
            ModalManager.setModal(gsc.endWindow, true, gsc.endRestartButton.gameObject, gsc.endRestartButton.GetType().Name);
            Debug.LogWarning("BackToEndPanelButton::OnPress - DONE");
        }
    }
}

using UnityEngine;
using System.Collections;

public class BackToEndPanelButton : ModalButton
{
    protected override void OnPress (bool isPressed)
    {
        //test on isPressed must be the opposite of CancelModal's so that
        //it's not possible that both buttons are clicked in a row
        //at the end of the game.
        //TODO find more elegant and robust solution
        if (isPressed) {
            Logger.Log ("BackToEndPanelButton::OnPress()", Logger.Level.INFO);
            Debug.LogWarning ("BackToEndPanelButton::OnPress");
            GameStateController gsc = GameStateController.get ();
            ModalManager.setModal (gsc.endWindow, true, gsc.endRestartButton.gameObject, gsc.endRestartButton.GetType ().Name);
            Debug.LogWarning ("BackToEndPanelButton::OnPress - DONE");
        }
    }
}

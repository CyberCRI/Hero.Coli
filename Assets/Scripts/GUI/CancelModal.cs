using UnityEngine;
using System.Collections;

public class CancelModal : ModalButton
{
    protected override void OnPress (bool isPressed)
    {
        
        //test on isPressed must be the opposite of BackToEndPanelButton's so that
        //it's not possible that both buttons are clicked in a row
        //at the end of the game.
        //TODO find more elegant and robust solution

        if (!isPressed) {
            Logger.Log ("CancelModal::OnPress()", Logger.Level.INFO);

            GameStateController.get ().tryUnlockPause ();
            ModalManager.unsetModal ();
        }
    }
}

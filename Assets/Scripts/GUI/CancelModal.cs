using UnityEngine;
using System.Collections;

//ModalButton inheritor that quits modal window when isPressed is false,
//contrary to QuitModalWindow
public class CancelModal : ModalButton
{
    protected override void OnPress (bool isPressed)
    {
        
        //test on isPressed must be the opposite of BackToEndPanelButton's so that
        //it's not possible that both buttons are clicked in a row
        //at the end of the game.
        //TODO find more elegant and robust solution

        if (!isPressed) {
            Debug.Log(this.GetType() + " CancelModal::OnPress()");
            press ();
        }
    }

    public override void press ()
    {
        Debug.Log(this.GetType() + " CancelModal::press()");
        GameStateController.get ().tryUnlockPause ();
        ModalManager.unsetModal ();
    }
}

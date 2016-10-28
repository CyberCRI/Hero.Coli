using UnityEngine;
using System.Collections;

//deprecated class temporarily kept for documentation
public class BackToEndPanelButton : ModalButton
{
    
    //test on isPressed must be the opposite of CancelModal's so that
    //it's not possible that both buttons are clicked in a row
    //at the end of the game.
    //See OnPress in mother class ModalButton and other class CancelModal
    //TODO find more elegant and robust solution

    public override void press ()
    {
        Logger.Log ("BackToEndPanelButton::press()");
        //GameStateController gsc = GameStateController.get ();
        //ModalManager.setModal (gsc.endWindow, true, gsc.endRestartButton.gameObject, gsc.endRestartButton.GetType ().AssemblyQualifiedName);
    }
}

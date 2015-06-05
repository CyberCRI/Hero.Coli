using UnityEngine;
using System.Collections;

public class EndMainMenuButton : ModalButton
{
    public override void press ()
    {
        Logger.Log ("EndMainMenuButton::press()", Logger.Level.INFO);
        //if commented out, causes "enter" in the main menu at the end of the game to restart the game
        ModalManager.unsetModal ();
        GameStateController.get ().endGame();
    }
}


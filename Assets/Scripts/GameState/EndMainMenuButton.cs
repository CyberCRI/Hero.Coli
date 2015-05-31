using UnityEngine;
using System.Collections;

public class EndMainMenuButton : ModalButton
{
    public override void press ()
    {
        Logger.Log ("EndMainMenuButton::press()", Logger.Level.INFO);
        GameStateController.get ().endGame();
    }
}


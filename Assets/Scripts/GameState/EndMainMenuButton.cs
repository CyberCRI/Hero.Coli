using UnityEngine;
using System.Collections;

public class EndMainMenuButton : ModalButton
{
    private string studyURLKey = "STUDY.LEARN.LINK"; 
    
    public override void press ()
    {
        Logger.Log ("EndMainMenuButton::press()", Logger.Level.INFO);
        URLOpener.open(studyURLKey, false);        
        
        //if commented out, causes "enter" in the main menu at the end of the game to restart the game
        ModalManager.unsetModal ();
        GameStateController.get ().endGame();
    }
}


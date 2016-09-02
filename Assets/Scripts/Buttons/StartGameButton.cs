using UnityEngine;
using System.Collections;

public class StartGameButton : ModalButton
{
  
    public Fade fadeSprite;
    public GameObject parentPanel;

    public override void press ()
    {
            Logger.Log ("StartGameButton::press()", Logger.Level.INFO);
            fadeSprite.FadeOut (7f);

            //TODO manage stack of modal elements in ModalManager
            //ModalManager.unsetModal(parentPanel);
            ModalManager.unsetModal ();
            GameStateController.get ().tryUnlockPause ();
    }
}

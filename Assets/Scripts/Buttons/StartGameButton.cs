using UnityEngine;
using System.Collections;

public class StartGameButton : ModalButton
{
  
    public Fade fadeSprite;
    public GameObject parentPanel;

    public override void press ()
    {
            Debug.Log(this.GetType() + " StartGameButton::press()");
            fadeSprite.FadeOut (7f);

            //TODO manage stack of modal elements in ModalManager
            //ModalManager.unsetModal(parentPanel);
            ModalManager.unsetModal ();
            GameStateController.get ().tryUnlockPause ();
    }
}

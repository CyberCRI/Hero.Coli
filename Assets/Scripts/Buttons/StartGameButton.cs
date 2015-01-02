using UnityEngine;
using System.Collections;

public class StartGameButton : ModalButton
{
  
    public GameStateController gameStateController;
    public Fade fadeSprite;
    public GameObject parentPanel;

    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("StartGameButton::OnPress()", Logger.Level.INFO);
            fadeSprite.FadeOut ();

            //TODO manage stack of modal elements in ModalManager
            //ModalManager.unsetModal(parentPanel);
            ModalManager.unsetModal ();
            gameStateController.tryUnlockPause ();
        }
    }
}

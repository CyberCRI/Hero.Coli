using UnityEngine;
using System.Collections;

public class StartGameButton : ModalButton
{
  
    public GameStateController gameStateController;
    public Fade fadeSprite;
    public GameObject panel;

    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("StartGameButton::OnPress()", Logger.Level.INFO);
            fadeSprite.FadeOut ();
            panel.SetActive (false);
            gameStateController.tryUnlockPause ();
        }
    }
}

using UnityEngine;
using System.Collections;

public class StartGameButton : MonoBehaviour {
	
	public GameStateController gameStateController;
	public Fade fadeSprite;
  public GameObject panel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("StartGameButton::OnPress()", Logger.Level.INFO);
      fadeSprite.FadeOut();
      panel.SetActive(false);
      gameStateController.changeState(GameState.Game);
      gameStateController.popPauseInStack();
	  }
  }
				
}

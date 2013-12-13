using UnityEngine;
using System.Collections;

public class OKButton : MonoBehaviour {
	public GameObject OKPanel;
	public GameStateController gameStateController;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnPress(bool isPressed) {
	    if(isPressed) {
	      Logger.Log("OKButton::OnPress()", Logger.Level.INFO);
	      OKPanel.SetActive(false);
		  gameStateController.StateChange(GameState.Game);
	    }
  }
}

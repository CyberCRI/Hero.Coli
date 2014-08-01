using UnityEngine;
using System.Collections;

public class ContinueButton : MonoBehaviour {
	
	public GameObject currentInfoPanel;
	public GameObject nextInfoPanel;
	public GameStateController gameStateController;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("ContinueButton::OnPress()", Logger.Level.INFO);
      currentInfoPanel.SetActive(false);
	  nextInfoPanel.SetActive(true);
	  gameStateController.changeState(GameState.Pause);
	  gameStateController.dePauseForbidden = true;
    }
  }
}

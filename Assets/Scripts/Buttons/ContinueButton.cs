using UnityEngine;
using System.Collections;

public class ContinueButton : MonoBehaviour {
	
	public GameObject nextInfoPanel;
	public GameStateController gameStateController;
	
	void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("ContinueButton::OnPress()", Logger.Level.INFO);
      nextInfoPanel.SetActive(true);
      gameObject.transform.parent.gameObject.SetActive(false);
    }
  }
}

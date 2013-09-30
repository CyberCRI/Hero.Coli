using UnityEngine;
using System.Collections;

public class WorldEquipButton : MonoBehaviour {

  public GUITransitioner transitioner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  private void OnPress(bool isPressed) {
    Logger.Log("WorldEquipButton::OnPress("+isPressed+")");
    if(isPressed) {
      transitioner.SwitchScreen(GUITransitioner.GameScreen.screen1, GUITransitioner.GameScreen.screen2);
    }
  }
}

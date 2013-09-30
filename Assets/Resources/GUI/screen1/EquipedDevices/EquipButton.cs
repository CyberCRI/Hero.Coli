using UnityEngine;
using System.Collections;

public class EquipButton : MonoBehaviour {

  public GUITransitioner transitioner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  //public override void OnPress (bool isPressed) { if (isEnabled) base.OnPress(isPressed); }
  private void OnPress(bool isPressed) {
    Logger.Log("EquipButton::OnPress("+isPressed+")");
    if(isPressed) {
      transitioner.SwitchScreen(GUITransitioner.GameScreen.screen1, GUITransitioner.GameScreen.screen2);
    }
  }
}

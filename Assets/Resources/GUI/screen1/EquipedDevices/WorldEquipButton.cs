using UnityEngine;
using System.Collections;

public class WorldEquipButton : MonoBehaviour {

  private void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("WorldEquipButton::OnPress()", Logger.Level.INFO);
      GUITransitioner.get().SwitchScreen(GUITransitioner.GameScreen.screen1, GUITransitioner.GameScreen.screen2);
    }
  }

}

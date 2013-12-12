using UnityEngine;
using System.Collections;

public class EquipCraftButton : MonoBehaviour {

  private void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("EquipCraftButton::OnPress()", Logger.Level.INFO);
      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
    }
  }
  
}


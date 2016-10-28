using UnityEngine;
using System.Collections;

public class CraftEquipButton : MonoBehaviour {

  private void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " CraftEquipButton::OnPress()");
      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen2);
    }
  }

}


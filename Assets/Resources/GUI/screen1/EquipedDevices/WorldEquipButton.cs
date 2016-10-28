using UnityEngine;
using System.Collections;

public class WorldEquipButton : MonoBehaviour {

  private void OnPress(bool isPressed) {
    if(isPressed && Inventory.isOpenable()) {
      Debug.Log(this.GetType() + " OnPress()");
      GUITransitioner.get().SwitchScreen(GUITransitioner.GameScreen.screen1, GUITransitioner.GameScreen.screen2);
    }
  }
}
using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer _craftFinalizer;

  void OnPress (bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.INFO);
      _craftFinalizer.finalizeCraft();
    }
  }

  public void SetActive(bool active) {
    gameObject.SetActive(active);
  }
}

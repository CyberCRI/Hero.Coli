using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer _craftFinalizer;

  void OnPress (bool isPressed) {
    Debug.Log ("CraftFinalizationButton::OnPress("+isPressed+")");
    if(isPressed) _craftFinalizer.finalizeCraft();
  }

  public void SetActive(bool active) {
    gameObject.SetActive(active);
  }
}

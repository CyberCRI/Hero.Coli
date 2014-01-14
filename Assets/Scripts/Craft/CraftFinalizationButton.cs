using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer craftFinalizer;
  private UIImageButton _button;

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.DEBUG);
      craftFinalizer.finalizeCraft();
    }
  }

  public void setEnabled(bool enabled) {
    if(_button == null) {
      _button = GetComponent<UIImageButton>();
    }
    _button.enabled = enabled;
    if(enabled)
      gameObject.GetComponentInChildren<UISprite>().alpha = 1f;
    else
      gameObject.GetComponentInChildren<UISprite>().alpha = 0.5f;
  }

  // Use this for initialization
  void Start () {
    if(_button == null) {
      _button = GetComponent<UIImageButton>();
    }
    //hack to correctly initialize button state
    setEnabled(!_button.enabled);
    setEnabled(!_button.enabled);
  }
}

using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer craftFinalizer;
  private UIImageButton _button;

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.INFO);
      craftFinalizer.finalizeCraft();
    }
  }

  public void setEnabled(bool enabled) {
    if(_button == null) {
      _button = GetComponent<UIImageButton>();
    }
    Logger.Log("CraftFinalizationButton::setEnabled("+enabled+") starts with _button.isEnabled="+_button.enabled, Logger.Level.TRACE);
    _button.enabled = enabled;
    Logger.Log("CraftFinalizationButton::setEnabled("+enabled+") ends with _button.isEnabled="+_button.enabled, Logger.Level.TRACE);
  }

  // Use this for initialization
  void Start () {
    Logger.Log("CraftFinalizationButton::Start", Logger.Level.TRACE);
    if(_button == null) {
      _button = GetComponent<UIImageButton>();
    }
    //hack to correctly initialize button state
    setEnabled(!_button.enabled);
    setEnabled(!_button.enabled);
  }
}

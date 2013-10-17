using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer craftFinalizer;
  private UIButton _button;

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.INFO);
      craftFinalizer.finalizeCraft();
    }
  }

  public void setEnabled(bool enabled) {
    if(_button == null) {
      _button = GetComponent<UIButton>();
    }
    Logger.Log("CraftFinalizationButton::setEnabled("+enabled+") starts with _button.isEnabled="+_button.isEnabled, Logger.Level.TRACE);
    _button.isEnabled = enabled;
    Logger.Log("CraftFinalizationButton::setEnabled("+enabled+") ends with _button.isEnabled="+_button.isEnabled, Logger.Level.TRACE);
  }

  // Use this for initialization
  void Start () {
    Logger.Log("CraftFinalizationButton::Start", Logger.Level.TRACE);
    if(_button == null) {
      _button = GetComponent<UIButton>();
    }
    //hack to correctly initialize button state
    setEnabled(!_button.isEnabled);
    setEnabled(!_button.isEnabled);
  }
}

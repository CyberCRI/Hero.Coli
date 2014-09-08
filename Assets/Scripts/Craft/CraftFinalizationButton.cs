using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer craftFinalizer;
  private UIButton _button;

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.DEBUG);
      craftFinalizer.finalizeCraft();
    }
  }

  public void setEnabled(bool enabled) {
    if(_button == null) {
      _button = GetComponent<UIButton>();
    }
		//Logger.Log ("bool enabled ====>"+enabled,Logger.Level.WARN);
    _button.enabled = enabled;
    if(enabled)
      gameObject.GetComponentInChildren<UISprite>().alpha = 1f;
    else{
			transform.Find("Background").GetComponent<UISprite>().alpha = 0.5f;
      //gameObject.GetComponentInChildren<UISprite>().alpha = 0.5f;
		}
  }

  // Use this for initialization
  void Start () {
    if(_button == null) {
      _button = GetComponent<UIButton>();
    }
    //hack to correctly initialize button state
    setEnabled(!_button.enabled);
    setEnabled(!_button.enabled);
  }
}

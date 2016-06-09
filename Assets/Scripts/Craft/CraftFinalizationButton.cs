using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer craftFinalizer;
  private UIButton _button;
  private UILocalize _localize;
  private CraftMode _mode = CraftMode.NOTHING;
  
  public enum CraftMode {
      CRAFT,
      UNCRAFT,
      ACTIVATE,
      INACTIVATE,
      NOTHING,
      DEFAULT
  }

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.DEBUG);
      switch(_mode)
      {
          case CraftMode.CRAFT:
            craftFinalizer.finalizeCraft();
          break;
          case CraftMode.UNCRAFT:
            craftFinalizer.uncraft();
          break;
          case CraftMode.ACTIVATE:
            craftFinalizer.equip();
          break;
          case CraftMode.INACTIVATE:
            craftFinalizer.unequip();
          break;
          case CraftMode.NOTHING:
          break;
          case CraftMode.DEFAULT:
          break;
      }
    }
  }

  // sets the clickability of the button
  private void setEnabled(bool enabled) {
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
  
  // manages all aspects when craft mode changes
  public void setCraftMode(CraftMode mode)
  {
      if(null == _localize)
        {
            _localize = GetComponentInChildren<UILocalize>();
        }
        
      switch(mode)
      {
          case CraftMode.CRAFT:
            _localize.key = "CRAFT.PROCESS.CRAFT";
            setEnabled(true);
          break;
          case CraftMode.UNCRAFT:
            _localize.key = "CRAFT.PROCESS.UNCRAFT";
            setEnabled(true);
          break;
          case CraftMode.ACTIVATE:
            _localize.key = "CRAFT.PROCESS.ACTIVATE";
            setEnabled(true);
          break;
          case CraftMode.INACTIVATE:
            _localize.key = "CRAFT.PROCESS.INACTIVATE";
            setEnabled(true);
          break;
          case CraftMode.NOTHING:
            _localize.key = "CRAFT.PROCESS.NOTHING";
            setEnabled(false);
          break;
          case CraftMode.DEFAULT:
            _localize.key = "CRAFT.PROCESS.NOTHING";
            setEnabled(false);
          break;
      }
      _mode = mode;
      _localize.Localize();
  }

  // Use this for initialization
  void Start () {
    if(null == _button) {
      _button = GetComponent<UIButton>();
    }
    //hack to correctly initialize button state
    setEnabled(!_button.enabled);
    setEnabled(!_button.enabled);
  }
}

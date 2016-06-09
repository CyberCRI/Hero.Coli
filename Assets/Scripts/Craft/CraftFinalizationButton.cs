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
  
  private const string _prefix          = "CRAFT.PROCESS.";
  private const string _keyCraft        = _prefix+"CRAFT";
  private const string _keyUncraft      = _prefix+"UNCRAFT";
  private const string _keyActivate     = _prefix+"ACTIVATE";
  private const string _keyInactivate   = _prefix+"INACTIVATE";
  private const string _keyNothing      = _prefix+"NOTHING";

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.DEBUG);
      switch(_mode)
      {
          // for craft / uncraft process
          case CraftMode.CRAFT:
            craftFinalizer.finalizeCraft();
          break;
          case CraftMode.UNCRAFT:
            craftFinalizer.uncraft();
          break;
          
          // for activate/inactivate process
          case CraftMode.ACTIVATE:
            craftFinalizer.finalizeCraft();
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
            _localize.key = _keyCraft;
            setEnabled(true);
          break;
          case CraftMode.UNCRAFT:
            _localize.key = _keyUncraft;
            setEnabled(true);
          break;
          case CraftMode.ACTIVATE:
            _localize.key = _keyActivate;
            setEnabled(true);
          break;
          case CraftMode.INACTIVATE:
            _localize.key = _keyInactivate;
            setEnabled(true);
          break;
          case CraftMode.NOTHING:
            _localize.key = _keyNothing;
            setEnabled(false);
          break;
          case CraftMode.DEFAULT:
            _localize.key = _keyNothing;
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

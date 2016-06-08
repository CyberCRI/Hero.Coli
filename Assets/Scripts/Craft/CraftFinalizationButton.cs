using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer craftFinalizer;
  private UIButton _button;
  private UILocalize _localize;
  private CraftMode _mode = CraftMode.NOTHING;
  
  public enum CraftMode {
      CRAFTING,
      UNCRAFTING,
      NOTHING
  }

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CraftFinalizationButton::OnPress()", Logger.Level.DEBUG);
      switch(_mode)
      {
          case CraftMode.CRAFTING:
            craftFinalizer.finalizeCraft();
          break;
          case CraftMode.UNCRAFTING:
            craftFinalizer.uncraft();
          break;
          case CraftMode.NOTHING:
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
          case CraftMode.CRAFTING:
            _localize.key = "CRAFT.PROCESS.CRAFT";
            setEnabled(true);
          break;
          case CraftMode.UNCRAFTING:
            _localize.key = "CRAFT.PROCESS.UNCRAFTING";
            setEnabled(true);
          break;
          case CraftMode.NOTHING:
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

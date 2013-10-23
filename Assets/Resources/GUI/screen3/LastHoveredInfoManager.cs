using UnityEngine;
using System.Collections;

public class LastHoveredInfoManager : MonoBehaviour {

  public UIAtlas        _atlas;
  public UISprite       _sprite;

  public UILabel                  _nameLabel;
  public UILabel                  _lengthLabel;
  //TODO refactor with FinalizationInfoPanelManager
  private string                  _lengthPrefix = "Length: ";
  private string                  _lengthPostfix = " bp";
  private string                  _defaultName = "";
  private string                  _defaultLength = "";
  private string                  _defaultSpriteName ="";

  public void setHoveredBioBrick<T>(T bioBrick) where T:BioBrick {
    Logger.Log("LastHoveredInfoManager::setHoveredBioBrick("+bioBrick+")", Logger.Level.TRACE);
    _nameLabel.text = bioBrick.getName();
    _lengthLabel.text = _lengthPrefix+bioBrick.getSize()+_lengthPostfix;
    _sprite.spriteName = DisplayedBioBrick.getSpriteName(bioBrick);
    _sprite.gameObject.SetActive(true);
  }

  public void setHoveredDefault() {
    Logger.Log("LastHoveredInfoManager::setHoveredDefault()", Logger.Level.TRACE);
    _nameLabel.text = _defaultName;
    _lengthLabel.text = _defaultLength;
    _sprite.spriteName = _defaultSpriteName;
    _sprite.gameObject.SetActive(false);
  }

	// Use this for initialization
	void Start () {
	  setHoveredDefault();
	}
}

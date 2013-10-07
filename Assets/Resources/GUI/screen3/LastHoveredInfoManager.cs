using UnityEngine;
using System.Collections;

public class LastHoveredInfoManager : MonoBehaviour {

  public UIAtlas        _atlas;
  public UISprite       _sprite;

  public UILabel                  _nameLabel;
  public UILabel                  _lengthLabel;
  private string                  _lengthPrefix = "Length: ";
  private string                  _lengthPostfix = " bp";

  public void setHoveredBioBrick<T>(T bioBrick) where T:BioBrick {
    Logger.Log("LastHoveredInfoManager::setHoveredBioBrick("+bioBrick+")", Logger.Level.TRACE);
    _nameLabel.text = bioBrick.getName();
    _lengthLabel.text = _lengthPrefix+bioBrick.getSize()+_lengthPostfix;
    _sprite.spriteName = DisplayedBioBrick.getSpriteName<T>(bioBrick);
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteInitializer : MonoBehaviour {
	public UIAtlas _atlas;
	public string _spriteName;
	
	private static string _defaultAtlas = "Atlases/TestAtlas/TestAtlas";
	private static List<string> _defaultSpriteNames = new List<string>{
		"Backdrop"
		,"brick"
		,"brickNM"
		,"burlap"
		,"sand"
	};

	// Use this for initialization
	void Start () {
		UISprite sprite = gameObject.GetComponent<UISprite>();
		if(_atlas != null) {
			sprite.atlas = _atlas;
		} else {
			GameObject atlas = Resources.Load(_defaultAtlas) as GameObject;
			sprite.atlas = atlas.GetComponent<UIAtlas>();
		}
		
		if(_spriteName != null && _spriteName != "") {
			sprite.spriteName = _spriteName;
		} else {
			int randomIdx = Random.Range(0, _defaultSpriteNames.Count);
			sprite.spriteName = _defaultSpriteNames[randomIdx];
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

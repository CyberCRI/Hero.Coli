using UnityEngine;
using System.Collections;

public class PotionsPanelBackground : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UISprite sprite = gameObject.GetComponent<UISprite>() as UISprite;
		sprite.type = UISprite.Type.Tiled;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

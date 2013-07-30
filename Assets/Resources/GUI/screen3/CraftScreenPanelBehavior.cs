using UnityEngine;
using System.Collections;

public class CraftScreenPanelBehavior : MonoBehaviour {
	private static Vector3 localPosition = new Vector3(0.0f, 0.0f, 0.0f);
	
	public GameObject _craftBackground;

	// Use this for initialization
	void Start () {
		gameObject.transform.localPosition = localPosition;
		
		UISprite sprite = _craftBackground.GetComponent<UISprite>() as UISprite;
		sprite.type = UISprite.Type.Tiled;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

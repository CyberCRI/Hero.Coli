using UnityEngine;
using System.Collections;

public class FX : MonoBehaviour {
	public GameObject FXSprite;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void LowLife(){
			TweenColor.Begin(FXSprite,2,new Color(1f,0f,0f,0.4f));
	}
}

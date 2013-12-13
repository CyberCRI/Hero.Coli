using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {
	public GameObject blackSprite;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void FadeOut(){
			TweenColor.Begin(blackSprite,7,new Color(0,0,0,0));
	}
	
	public void FadeIn(){
			TweenColor.Begin(blackSprite,7,new Color(0,0,0,1));
	}

}

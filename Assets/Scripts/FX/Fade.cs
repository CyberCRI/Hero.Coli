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
	
	public void FadeOut(float fadeSpeed){
            this.GetComponent<UISprite>().alpha = 1;
			TweenColor.Begin(blackSprite, fadeSpeed, new Color(0,0,0,0));
      //TODO don't enable instantly, enable after a few seconds
      gameObject.GetComponent<Collider>().enabled = false;
	}
	
	public void FadeIn(float fadeSpeed){
        this.GetComponent<UISprite>().alpha = 0;
        TweenColor.Begin(blackSprite,fadeSpeed,new Color(0,0,0,1));
      gameObject.GetComponent<Collider>().enabled = true;
	}

}

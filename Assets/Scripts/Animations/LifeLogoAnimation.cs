using UnityEngine;
using System.Collections;

public class LifeLogoAnimation : MonoBehaviour {
	public bool isPlaying = false;
	private UISprite sprite;
	private float time, animationTime = 10f, miTime, dum;
	private Vector3 originalScale;
	// Use this for initialization
	void Start () {
		time =  0f;
		miTime = animationTime/2;
		sprite = GetComponent<UISprite>();
		originalScale = sprite.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying){
			time += 1;
			dum = 2*time/animationTime;
			if(time < miTime){
				sprite.color = new Color(1,1-dum,1-dum,1);
				sprite.transform.localScale = originalScale*(1+dum);
			}
			if(time > miTime){
				sprite.color = new Color(1,dum,dum,1);
				sprite.transform.localScale = originalScale*(3-dum);
			}
		}
		//Logger.Log("time="+time, Logger.Level.ONSCREEN);
		if (time >= animationTime ){
			isPlaying = false;
		}
	}
	
	public void Play() {
		//Logger.Log("PLAY", Logger.Level.ONSCREEN);
		isPlaying = true;
		time =0f;
	}
}

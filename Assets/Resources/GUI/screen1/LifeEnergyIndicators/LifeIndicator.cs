using UnityEngine;
using System.Collections;

public class LifeIndicator : MonoBehaviour {
	
	public Hero hero;
	private Vector3 _initialScale;
	public float maxXScale = 200.0f;
	public float startLife = 1.0f;
	private Transform _foreground;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Perso").GetComponent<Hero>();
		GameObject lifeIndicator = GameObject.Find ("LifeIndicator");
		Transform progressBar = lifeIndicator.transform.Find("ProgressBar");
		_foreground = progressBar.transform.Find("Foreground");
		_initialScale = _foreground.localScale;
		
		hero.setLife(startLife);
	}
	
	// Update is called once per frame
	void Update () {
		_foreground.localScale = new Vector3(hero.getLife ()*maxXScale, _initialScale.y, _initialScale.z);
	}
}

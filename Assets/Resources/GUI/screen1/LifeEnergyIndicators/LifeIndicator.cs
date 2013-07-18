using UnityEngine;
using System.Collections;

public class LifeIndicator : MonoBehaviour {
	
	public Hero hero;
	private Vector3 initialScale;
	public float maxXScale = 200.0f;
	public float startLife = 1.0f;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Hero").GetComponent<Hero>();
		GameObject lifeIndicator = GameObject.Find ("LifeIndicator");
		Transform progressBar = lifeIndicator.transform.Find("ProgressBar");
		Transform background = progressBar.transform.Find("Background");
		maxXScale = background.transform.localScale.x;
		initialScale = transform.localScale;
		
		hero.setLife(startLife);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(hero.getLife ()*maxXScale, initialScale.y, initialScale.z);
	}
}

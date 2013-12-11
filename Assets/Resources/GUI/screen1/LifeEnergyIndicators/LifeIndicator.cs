using UnityEngine;
using System.Collections;

public class LifeIndicator : MonoBehaviour {
	
	public Hero hero;
	private Vector3 _initialScale;
	public float maxXScale = 200.0f;
	public int maxLife = 100;
	public float startLife = 1.0f;
	
	private Transform _foreground;
	private UILabel _lifeValueLabel;
	private int _lifeValue;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Perso").GetComponent<Hero>();
		GameObject lifeIndicator = GameObject.Find ("LifeIndicator");
		Transform progressBar = lifeIndicator.transform.Find("ProgressBar");
		_foreground = progressBar.transform.Find("Foreground");
		_initialScale = _foreground.localScale;
		_lifeValueLabel = GameObject.Find("LifeValue").GetComponent<UILabel>();
		_lifeValue = maxLife;
		
		hero.setLife(startLife);
	}
	
	// Update is called once per frame
	void Update () {
		_lifeValue = (int)(hero.getLife()*maxLife);
		_foreground.localScale = new Vector3(hero.getLife ()*maxXScale, _initialScale.y, _initialScale.z);
		_lifeValueLabel.text = _lifeValue.ToString();
	}
}

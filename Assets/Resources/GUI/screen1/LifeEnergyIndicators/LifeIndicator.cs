using UnityEngine;
using System.Collections;

public class LifeIndicator : MonoBehaviour {
	
	public Hero hero;
	public int maxLife = 100;
	public float startLife = 1.0f;
	
	private UILabel _lifeValueLabel;
	private int _lifeValue;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Perso").GetComponent<Hero>();
		_lifeValueLabel = GameObject.Find("LifeValue").GetComponent<UILabel>();
		_lifeValue = maxLife;
		
		hero.setLife(startLife);
	}
	
	// Update is called once per frame
	void Update () {
		_lifeValue = Mathf.CeilToInt(hero.getLife()*maxLife);
		_lifeValueLabel.text = _lifeValue.ToString();
	}
}

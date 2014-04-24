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

		if (GameObject.Find ("Perso") != null)
		{
			hero = GameObject.Find ("Perso").GetComponent<Hero>();
			hero.setLife(startLife);
		}
		_lifeValueLabel = GameObject.Find("LifeValue").GetComponent<UILabel>();
		_lifeValue = maxLife;
		

	}
	
	// Update is called once per frame
	void Update () {
		if(hero != null)
		{
			_lifeValue = Mathf.CeilToInt(hero.getLife()*maxLife);
			_lifeValueLabel.text = _lifeValue.ToString();
		}
	}
}

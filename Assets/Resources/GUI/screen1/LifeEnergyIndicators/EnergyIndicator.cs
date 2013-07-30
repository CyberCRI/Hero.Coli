using UnityEngine;
using System.Collections;

public class EnergyIndicator : MonoBehaviour {
	
	public Hero hero;
	private Vector3 _initialScale;
	public float maxXScale = 200.0f;
	public float startEnergy = 1.0f;
	
	private Transform _foreground;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Hero").GetComponent<Hero>();
		GameObject energyIndicator = GameObject.Find ("EnergyIndicator");
		Transform progressBar = energyIndicator.transform.Find("ProgressBar");
		_foreground = progressBar.transform.Find("Foreground");
		_initialScale = _foreground.localScale;
		
		hero.setEnergy(startEnergy);
	}
	
	// Update is called once per frame
	void Update () {
		_foreground.localScale = new Vector3(hero.getEnergy ()*maxXScale, _initialScale.y, _initialScale.z);
	}
}

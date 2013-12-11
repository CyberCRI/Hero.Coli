using UnityEngine;
using System.Collections;

public class EnergyIndicator : MonoBehaviour {
	
	public Hero hero;
	private Vector3 _initialScale;
	public float maxXScale = 200.0f;
	public int maxEnergy = 100;
	public float startEnergy = 1.0f;
	
	private Transform _foreground;
	private UILabel _energyValueLabel;
	private int _energyValue;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Perso").GetComponent<Hero>();
		GameObject energyIndicator = GameObject.Find ("EnergyIndicator");
		Transform progressBar = energyIndicator.transform.Find("ProgressBar");
		_foreground = progressBar.transform.Find("Foreground");
		_initialScale = _foreground.localScale;
		_energyValueLabel = GameObject.Find("EnergyValue").GetComponent<UILabel>();
		_energyValue = maxEnergy;
			
		hero.setEnergy(startEnergy);
	}
	
	// Update is called once per frame
	void Update () {
		_energyValue = (int)(hero.getEnergy()*maxEnergy);
		_foreground.localScale = new Vector3(hero.getEnergy ()*maxXScale, _initialScale.y, _initialScale.z);
		_energyValueLabel.text = _energyValue.ToString();
	}
}

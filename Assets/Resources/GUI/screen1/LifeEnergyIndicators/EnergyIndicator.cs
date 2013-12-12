using UnityEngine;
using System.Collections;

public class EnergyIndicator : MonoBehaviour {
	
	public Hero hero;
	public int maxEnergy = 100;
	public float startEnergy = 1.0f;
	
	private UILabel _energyValueLabel;
	private int _energyValue;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Perso").GetComponent<Hero>();
		_energyValueLabel = GameObject.Find("EnergyValue").GetComponent<UILabel>();
		_energyValue = maxEnergy;
		hero.setEnergy(startEnergy);
	}
	
	// Update is called once per frame
	void Update () {
		_energyValue = (int)(hero.getEnergy()*maxEnergy);
		_energyValueLabel.text = _energyValue.ToString();
	}
}

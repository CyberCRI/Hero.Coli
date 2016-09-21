using UnityEngine;

public class EnergyIndicator : MonoBehaviour {
	
	public Hero hero;
	public int maxEnergy = 100;
	public float startEnergy = 1.0f;
	
	private UILabel _energyValueLabel;
	private int _energyValue;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = Hero.get();
		_energyValueLabel = GameObject.Find("EnergyValue").GetComponent<UILabel>();
		_energyValue = maxEnergy;
	}
	
	// Update is called once per frame
	void Update () {
		if(hero != null)
		{
			_energyValue = Mathf.CeilToInt(hero.getEnergy()*maxEnergy);
			_energyValueLabel.text = _energyValue.ToString();
		}
	}
}

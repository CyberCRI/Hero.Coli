using UnityEngine;
using System.Collections;

public class EnergyIndicator : MonoBehaviour {
	
	public Hero hero;
	private Vector3 initialScale;
	public float maxXScale = 0.0f;
	public float startEnergy = 1.0f;

	// Use this for initialization
	void Start () {
		//TODO trigger this after resize
		hero = GameObject.Find ("Hero").GetComponent<Hero>();
		GameObject energyIndicator = GameObject.Find ("EnergyIndicator");
		Transform progressBar = energyIndicator.transform.Find("ProgressBar");
		Transform background = progressBar.transform.Find("Background");
		maxXScale = background.transform.localScale.x;
		initialScale = transform.localScale;
		
		hero.setEnergy(startEnergy);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(hero.getEnergy ()*maxXScale, initialScale.y, initialScale.z);
	}
}

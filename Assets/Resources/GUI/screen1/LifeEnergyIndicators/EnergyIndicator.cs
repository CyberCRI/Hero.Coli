using UnityEngine;
using System.Collections;

public class EnergyIndicator : MonoBehaviour {
	
	public Hero hero;
	private Vector3 _initialScale;
	private float maxXScale = 200.0f;
  public Transform foregroundTransform;

	// Use this for initialization
	void Start () {
		_initialScale = foregroundTransform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		foregroundTransform.localScale = new Vector3(hero.getEnergy ()*maxXScale, _initialScale.y, _initialScale.z);
	}
}

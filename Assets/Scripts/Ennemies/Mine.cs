using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {


	private float _radius = 10f;

	private bool _isNear = false;

	private Hashtable _optionsIn = iTween.Hash(
		"scale", Vector3.one,
		"time", 0.8f,
		"easetype", iTween.EaseType.easeOutElastic
		);

	private Hashtable _optionsOut = iTween.Hash(
		"scale", Vector3.zero,
		"time",1.2f,
		"easetype", iTween.EaseType.easeInQuint
		);

	// Use this for initialization
	void Start () {
		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
		detection ();
	}

	void detection() {


		Collider[] hitsColliders = Physics.OverlapSphere(transform.position, _radius);
		
		// If Player is in a small area
		if (System.Array.Find(hitsColliders, (col) => col.gameObject.name == "Perso"))
		{
			if(!_isNear)
			{
				iTween.ScaleTo(this.gameObject, _optionsIn);
				_isNear = true;
			}
		}
		else 
		{
			iTween.ScaleTo (this.gameObject, _optionsOut);
			_isNear = false;
		}


	}

}

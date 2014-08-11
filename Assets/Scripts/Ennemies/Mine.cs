using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {


	private float _radius = 9f;

	private bool _isNear = false;

	private Hashtable _optionsIn = iTween.Hash(
		"scale", Vector3.one,
		"time", 0.8f,
		"easetype", iTween.EaseType.easeOutElastic
		);

	private Hashtable _optionsOut = iTween.Hash(
		"scale", Vector3.zero,
		"time",1f,
		"easetype", iTween.EaseType.easeInQuint
		);

	// Use this for initialization
	void Start () {
		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
		detection ();

		if(_isNear && transform.FindChild("Point light").GetComponent<TriggeredLight>().getIsStarting() == false)
		{
			transform.FindChild("Point light").GetComponent<TriggeredLight>().triggerStart();
		}
		else if (!_isNear && transform.FindChild("Point light").GetComponent<TriggeredLight>().getIsStarting() == true)
		{
			transform.FindChild("Point light").GetComponent<TriggeredLight>().triggerExit();
		}
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

	void OnCollisionEnter(Collision col) {
		/*if(col.gameObject.name == "Perso")
		{
			transform.FindChild("Mine Light Collider").GetComponent<SimpleFracture>().OnCollisionEnter
		}*/


	}

}

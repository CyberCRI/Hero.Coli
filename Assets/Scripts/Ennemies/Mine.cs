using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

	private string _targetMolecule = "FLUO1";  //the name of the molecule that the mine is sensitive
	private float _concentrationTreshold = 2f;

	private float _radius = 9f;
	private bool _isNear = false;

	private Hashtable _optionsIn = iTween.Hash(
		"scale", Vector3.one,
		//"alpha", 255f,
		"time", 0.8f,
		"easetype", iTween.EaseType.easeOutElastic
		);

	private Hashtable _optionsOut = iTween.Hash(
		"scale", Vector3.zero,
		//"alpha", 0f,
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

		//Start the red light of the mine
		if(_isNear && transform.FindChild("Point light").GetComponent<TriggeredLight>().getIsStarting() == false)
		{
			transform.FindChild("Point light").GetComponent<TriggeredLight>().triggerStart();
		}

		//end the red light of the mine
		else if (!_isNear && transform.FindChild("Point light").GetComponent<TriggeredLight>().getIsStarting() == true)
		{
			transform.FindChild("Point light").GetComponent<TriggeredLight>().triggerExit();
		}
	}

	void detection() {


		Collider[] hitsColliders = Physics.OverlapSphere(transform.position, _radius);
		
		// If Player is in a small area
		Collider match = System.Array.Find(hitsColliders, (col) => col.gameObject.name == "Perso");
			if(match)
		{
			if(match.transform.GetComponent<Hero>().getMedium() != null)
			{
				ArrayList molecules= match.transform.GetComponent<Hero>().getMedium().getMolecules();
				
				foreach(Molecule m in molecules)
				{
					//If player has the Green Fluorescence with a sufficient concentration :: the mine appears
					if (m.getName() == _targetMolecule && m.getConcentration() > _concentrationTreshold)
					{
						if(!_isNear)
						{
							iTween.ScaleTo(this.gameObject, _optionsIn);
							//iTween.FadeTo(this.gameObject, _optionsIn);
							_isNear = true;
						}
					}
					else if(m.getName() == _targetMolecule && m.getConcentration() < _concentrationTreshold)
					{
						iTween.ScaleTo (this.gameObject, _optionsOut);
						//iTween.FadeTo (this.gameObject, _optionsOut);
						_isNear = false;
					}
				}
				
			}

		}
		else 
		{
			iTween.ScaleTo (this.gameObject, _optionsOut);
			//iTween.FadeTo (this.gameObject, _optionsOut);
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

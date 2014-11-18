using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {


	private bool _changed = false;
	private float _x;
	private float _z;
	private float _id;

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
	//	"alpha", 0f,
		"time",1f,
		"easetype", iTween.EaseType.easeInQuint
		);


	public void setChanged(bool b) { 
		if (b == true)
			SceneManager3.mineChanged += 1;
		_changed = b;
	}

	public bool getChanged() { return _changed;}

	public float getX() {return _x;}
	public float getZ() {return _z;}
	public float getId() {return _id;}

	public void setId(float f) {_id = f;}

	// Use this for initialization
	void Start () {
		transform.localScale = Vector3.zero;

		_x = transform.position.x;
		_z = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {


		AutoReset();

	
		detection ();

		//Start the red light of the mine

		if(transform.FindChild("Point light"))
		{
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
							//iTween.FadeTo(transform.FindChild("Mine Light Collider").gameObject, _optionsIn);
							//TweenAlpha.Begin(transform.FindChild("Mine Light Collider").gameObject,1f,1f);
							_isNear = true;
						}
					}
					else if(m.getName() == _targetMolecule && m.getConcentration() < _concentrationTreshold)
					{
						iTween.ScaleTo (this.gameObject, _optionsOut);
						//iTween.FadeTo (transform.FindChild("Mine Light Collider").gameObject, _optionsOut);
						//TweenAlpha.Begin(transform.FindChild("Mine Light Collider").gameObject,1f,0f);
						_isNear = false;

					}
				}
				
			}

		}
		else 
		{
			iTween.ScaleTo (this.gameObject, _optionsOut);
			//iTween.FadeTo (transform.FindChild("Mine Light Collider").gameObject, _optionsOut);
			//TweenAlpha.Begin(transform.FindChild("Mine Light Collider").gameObject,1f,0f);
			_isNear = false;
		}


	}
	public void stopAnimation() {
		if(_changed)
		{
			iTween.Stop(transform.FindChild("Point light").gameObject);
		}
	}


	public void AutoReset()
	{
		if(SceneManager3.isReseting && _changed)
		{
			GameObject.Find ("SceneManager").GetComponent<SceneManager3>().resetSelectedMine(_id, gameObject);
		}
	}

}

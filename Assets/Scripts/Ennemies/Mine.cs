using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {


	private MeshRenderer _physicRenderer;
	private MeshRenderer _lightRenderer;
	private float _radius = 5f;
	private bool _found = false;
	// Use this for initialization
	void Start () {

		_physicRenderer = transform.FindChild("Mine Physic Collider").GetComponent<MeshRenderer>();
		_physicRenderer.enabled = false;

		Logger.Log ("_physicrenderer enabled ::"+_physicRenderer.enabled,Logger.Level.WARN);
		_lightRenderer = transform.FindChild("Mine Light Collider").GetComponent<MeshRenderer>();
		_lightRenderer.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
	
		detection ();
	}

	void detection() {

		if(_found)
			_found = false;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius);
		foreach(Collider c in hitColliders)
		{
			Logger.Log ("name::"+c.gameObject.name,Logger.Level.WARN);
			if (c.gameObject.name == "Perso" && _lightRenderer.enabled == false)
			{
				_lightRenderer.enabled = true;
				break;
				_found= true;
			}
		}

	/*	Collider[] hitsColliders = Physics.OverlapSphere(transform.position, _radius);
		
		// If Player is in a small area
		if (System.Array.FindAll(hitsColliders, (col) => {return (col.gameObject.name == "Perso");}))
		{
			Logger.Log ("trouvé",Logger.Level.WARN);
		}*/


		if(_found == false && _lightRenderer.enabled == true)
			_lightRenderer.enabled = false;

	}
}

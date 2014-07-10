using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {


	private MeshRenderer _physicRenderer;
	private MeshRenderer _lightRenderer;
	private float _radius = 10f;
	// Use this for initialization
	void Start () {

		_physicRenderer = transform.FindChild("Mine Physic Collider").GetComponent<MeshRenderer>();
		_physicRenderer.enabled = false;

		_lightRenderer = transform.FindChild("Mine Light Collider").GetComponent<MeshRenderer>();
		_lightRenderer.enabled = false;

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
			if(!_lightRenderer.enabled)
			{
				_physicRenderer.enabled = true;
				_lightRenderer.enabled = true;
			}
		}
		else if(_lightRenderer.enabled == true)
		{
			_physicRenderer.enabled = false;
			_lightRenderer.enabled = false;
		}


	}
}

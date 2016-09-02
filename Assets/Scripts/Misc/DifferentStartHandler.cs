using UnityEngine;
using System.Collections;

public class DifferentStartHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<PhenoSpeed>().setDefaultFlagellaCount(0);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}

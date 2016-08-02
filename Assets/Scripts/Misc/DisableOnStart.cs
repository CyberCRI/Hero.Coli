using UnityEngine;
using System.Collections;

public class DisableOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

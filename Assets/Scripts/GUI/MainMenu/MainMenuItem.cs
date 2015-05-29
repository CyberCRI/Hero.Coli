using UnityEngine;
using System.Collections;

public class MainMenuItem : MonoBehaviour {

    public string name;
    
    public void select() {
        transform.localScale = new Vector3(transform.localScale.x*2, transform.localScale.y, transform.localScale.z);
    }
    public void deselect() {
        transform.localScale = new Vector3(transform.localScale.x/2, transform.localScale.y, transform.localScale.z);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class SwitchZoneOnOff : MonoBehaviour {

    [SerializeField]
    private GameObject[] _activeZone;
    [SerializeField]
    private GameObject[] _inactiveZone;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.tag);
        if (col.tag == "Player")
        {
            Debug.Log(col.tag);
            for (var i = 0; i < _activeZone.Length; i++)
            {
                _activeZone[i].SetActive(true);                
            }
            for (var i = 0; i < _inactiveZone.Length; i++)
            {
                _inactiveZone[i].SetActive(false);
            }
        }
    }
}

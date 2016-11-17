using UnityEngine;

public class RespawnPoint : MonoBehaviour {

    private int _colNumber = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag)
        {
            AmbientLighting ambLight = col.GetComponent<AmbientLighting>();
            if (_colNumber == 0)
            {
                ambLight.saveCurrentLighting();
                _colNumber++;
            }
            else
            {
                ambLight.startReset();
            }
        }
    }
}

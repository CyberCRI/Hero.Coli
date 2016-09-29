using UnityEngine;
using System.Collections;
using System;

public class CutSceneZone8 : CutScene {

    [SerializeField]
    private GameObject _dummy;
    private int _collisionIteration = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void initialize()
    {
        
    }

    public override void startCutScene()
    {
        
    }

    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
        this.transform.parent.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Door")
        {
            if (_collisionIteration == 0)
            {
                start();
                _collisionIteration++;
                StartCoroutine(WaitForOpeningDoor());
            }
            else
            {
                /*Destroy(_dummy.GetComponent<RotationUpdate>());
                Destroy(_dummy.GetComponent<PlatformMvt>());
                _dummy.GetComponent<iTweenEvent>().enabled = true;
                StartCoroutine(WaitForEnd());*/
            }
        }
    }


    IEnumerator WaitForOpeningDoor()
    {
        yield return new WaitForSeconds(3f);
        Destroy(_dummy.GetComponent<RotationUpdate>());
        Destroy(_dummy.GetComponent<PlatformMvt>());
        _dummy.GetComponent<iTweenEvent>().enabled = true;
        StartCoroutine(WaitForEnd());
        yield return null;
    }

    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(3f);
        end();
        yield return null;
    }
}

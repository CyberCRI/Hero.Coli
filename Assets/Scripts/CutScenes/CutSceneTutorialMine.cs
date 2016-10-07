using UnityEngine;
using System.Collections;
using System;

public class CutSceneTutorialMine : CutScene {

    [SerializeField]
    private GameObject[] _dummies;
    private int iteration = 0;
    private bool _once = true;

    override public void initialize()
    {

    }

    public override void startCutScene()
    {
        StartDummy(_dummies[iteration]);
    }

    private void StartDummy(GameObject dummy)
    {
        if (iteration < 3)
        {
            Destroy(dummy.GetComponent<PlatformMvt>());
            dummy.GetComponent<iTweenEvent>().enabled = true;
            dummy.GetComponent<RotationUpdate>().enabled = false;
            StartCoroutine(WaitForDeathDummy(dummy));
            iteration++;
        }
    }

    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
        this.enabled = false;
    }

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Door" && _once == true)
        {
            _once = false;
            start();
        }
    }

    IEnumerator WaitForDeathDummy(GameObject dummy)
    {
        if (iteration <2)
        {
            while (dummy != null)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(5f);
            end();
        }
        StartDummy(_dummies[iteration]);
        yield return null;
    }
}

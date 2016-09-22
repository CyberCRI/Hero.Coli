using UnityEngine;
using System.Collections;
using System;

public class CutSceneConclusion : CutScene {

    [SerializeField]
    private PlatformMvt[] _BigBadGuyPLatformMvt;
    private int iteration = 0;
    private int _collisionIteration = 0;

	// Use this for initialization
	void Start () {
        //ActivateOneByOne();
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
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && _collisionIteration == 0)
        {
            _collisionIteration += 1;
            StartCoroutine(WaitBetweenActivation(2f));
            for (int i = 0; i < _BigBadGuyPLatformMvt.Length; i++)
            {
                _BigBadGuyPLatformMvt[i].GetComponent<BigBadGuy>().WakeUp(true);
            }
        }
    }

    private void ActivateOneByOne()
    {
        if (iteration < _BigBadGuyPLatformMvt.Length)
        {
            _BigBadGuyPLatformMvt[iteration].enabled = true;
            iteration++;
            StartCoroutine(WaitBetweenActivation(0.8f));
        }
    }

    IEnumerator WaitBetweenActivation(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        ActivateOneByOne();
        yield return null;
    }

}

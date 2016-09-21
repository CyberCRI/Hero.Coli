using UnityEngine;
using System.Collections;
using System;

public class CutSceneConclusion : CutScene {

    [SerializeField]
    private PlatformMvt[] _BigBadGuyPLatformMvt;
    private int iteration = 0;

	// Use this for initialization
	void Start () {
        ActivateOneByOne();
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

    private void ActivateOneByOne()
    {
        if (iteration < _BigBadGuyPLatformMvt.Length)
        {
            _BigBadGuyPLatformMvt[iteration].enabled = true;
            iteration++;
            StartCoroutine(WaitBetweenActivation());
        }
    }

    IEnumerator WaitBetweenActivation()
    {
        yield return new WaitForSeconds(0.8f);
        ActivateOneByOne();
        yield return null;
    }

}

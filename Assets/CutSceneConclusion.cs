using UnityEngine;
using System.Collections;
using System;

public class CutSceneConclusion : CutScene {

    [SerializeField]
    private PlatformMvt[] _BigBadGuyPLatformMvt;
    private int iteration = 0;
    private int _collisionIteration = 0;
    [SerializeField]
    private Camera _CutSceneCam;
    [SerializeField]
    private GameObject _wayPoint1;
    [SerializeField]
    private GameObject _wayPoint2;
    [SerializeField]
    private float _offSet;

    public override void initialize()
    {
        
    }

    public override void startCutScene()
    {
        _CutSceneCam.transform.position = _boundCamera.transform.position;
        _CutSceneCam.transform.rotation = _boundCamera.transform.rotation;
        _boundCamera.gameObject.SetActive(false);
        _CutSceneCam.gameObject.SetActive(true);
        _wayPoint1.transform.position = _CutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_wayPoint2.transform.position.x,_wayPoint1.transform.position.y + _offSet,_wayPoint2.transform.position.z);
        _CutSceneCam.GetComponent<PlatformMvt>().enabled = true;
    }

    public override void endCutScene()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && _collisionIteration == 0)
        {
            start();
            _collisionIteration += 1;
            //StartCoroutine(WaitBetweenActivation(2f));
            for (int i = 0; i < _BigBadGuyPLatformMvt.Length; i++)
            {
                //_BigBadGuyPLatformMvt[i].GetComponent<BigBadGuy>().WakeUp(true);
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

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
    private Transform _camPosition;
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
        StartCoroutine(WaitForTravellingCam(1));
    }

    public override void endCutScene()
    {
        StartCoroutine(WaitBetweenActivation(2f));
        _CutSceneCam.gameObject.SetActive(false);
        _boundCamera.gameObject.SetActive(true);
        _cellControl.freezePlayer(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag && _collisionIteration == 0)
        {
            start();
            _collisionIteration += 1;
            //StartCoroutine(WaitBetweenActivation(2f));
            
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

    private void SecondPart()
    {
        _wayPoint1.transform.position = _wayPoint2.transform.position;
        _wayPoint2.transform.position = _camPosition.transform.position;
        _CutSceneCam.GetComponent<PlatformMvt>().restart();
        StartCoroutine(WaitForTravellingCam(2));
    }

    private void ThirdPart()
    {
        Debug.Log("third");
        end();
        _wayPoint1.transform.position = _wayPoint2.transform.position;
        _wayPoint2.transform.position = _boundCamera.transform.position;
    }

    IEnumerator WaitBetweenActivation(float timeToWait)
    {
        _cellControl.freezePlayer(false);
        yield return new WaitForSeconds(timeToWait);
        ActivateOneByOne();
        yield return null;
    }

    IEnumerator WaitForTravellingCam(int iteration)
    {
        while (Vector3.Distance(_CutSceneCam.transform.position, _wayPoint2.transform.position) <= 0.5f)
        {
            yield return null;
        }
        if (iteration == 2)
        {
            for (int i = 0; i < _BigBadGuyPLatformMvt.Length; i++)
            {
                _BigBadGuyPLatformMvt[i].GetComponent<BigBadGuy>().WakeUp(true);
            }
        }
        yield return new WaitForSeconds(2f);

        if (iteration == 1)
        {
            SecondPart();
        }
        else if (iteration == 2)
        {
            ThirdPart();
        }
        yield return null;
    }
}

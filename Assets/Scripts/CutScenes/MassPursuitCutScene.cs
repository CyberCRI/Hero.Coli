// #define QUICKTEST

using UnityEngine;
#if !QUICKTEST
using System.Collections;
#endif

public class MassPursuitCutScene : CutScene
{
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

#if QUICKTEST
    public override void startCutScene()
    {
    }
    public override void endCutScene()
    {
    }
#else

    public override void startCutScene()
    {
        _CutSceneCam.transform.position = BoundCamera.instance.transform.position;
        _CutSceneCam.transform.rotation = BoundCamera.instance.transform.rotation;
        BoundCamera.instance.gameObject.SetActive(false);
        _CutSceneCam.gameObject.SetActive(true);
        _wayPoint1.transform.position = _CutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_wayPoint2.transform.position.x, _wayPoint1.transform.position.y + _offSet, _wayPoint2.transform.position.z);
        _CutSceneCam.GetComponent<PlatformMvt>().enabled = true;
        StartCoroutine(waitForTravellingCam(1));
    }

    public override void endCutScene()
    {
        StartCoroutine(waitBetweenActivation(2f));
        _CutSceneCam.gameObject.SetActive(false);
        BoundCamera.instance.gameObject.SetActive(true);
        _reinstantiateOnTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag && _collisionIteration == 0)
        {
            start();
            _collisionIteration += 1;
            //StartCoroutine(WaitBetweenActivation(2f));

        }
    }

    private void activateOneByOne()
    {
        if (iteration < _BigBadGuyPLatformMvt.Length)
        {
            _BigBadGuyPLatformMvt[iteration].enabled = true;
            iteration++;
            StartCoroutine(waitBetweenActivation(0.8f));
        }
    }

    private void secondPart()
    {
        _wayPoint1.transform.position = _wayPoint2.transform.position;
        _wayPoint2.transform.position = _camPosition.transform.position;
        _CutSceneCam.GetComponent<PlatformMvt>().restart();
        StartCoroutine(waitForTravellingCam(2));
    }

    private void thirdPart()
    {
        end();
        _wayPoint1.transform.position = _wayPoint2.transform.position;
        _wayPoint2.transform.position = BoundCamera.instance.transform.position;
    }

    IEnumerator waitBetweenActivation(float timeToWait)
    {
        _cellControl.freezePlayer(false);
        yield return new WaitForSeconds(timeToWait);
        activateOneByOne();
        yield return null;
    }

    IEnumerator waitForTravellingCam(int iteration)
    {
        while (Vector3.Distance(_CutSceneCam.transform.position, _wayPoint2.transform.position) <= 0.5f)
        {
            yield return null;
        }
        if (iteration == 2)
        {
            for (int i = 0; i < _BigBadGuyPLatformMvt.Length; i++)
            {
                _BigBadGuyPLatformMvt[i].GetComponent<BigBadGuy>().wakeUp(true);
            }
        }
        yield return new WaitForSeconds(2f);

        if (iteration == 1)
        {
            secondPart();
        }
        else if (iteration == 2)
        {
            thirdPart();
        }
        yield return null;
    }
#endif
}

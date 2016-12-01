// #define QUICKTEST

using UnityEngine;
#if !QUICKTEST
using System.Collections;
#endif

public class PursuitCutScene : CutScene
{

    [SerializeField]
    private GameObject _badGuy;
    [SerializeField]
    private Camera _mainCamCopy;
    [SerializeField]
    private Transform _wayPoint1;
    [SerializeField]
    private Transform _wayPoint2;
    private int _iteration = 0;
    private Vector3 _mainCamOrigin;

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

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag)
        {
            if (_iteration == 0)
            {
                start();
                _mainCamOrigin = _boundCamera.transform.position;
            }
        }
    }

    public override void startCutScene()
    {

        SetMainCamCopy(1);
        _iteration++;
    }

    public override void endCutScene()
    {
        _badGuy.GetComponent<PlatformMvt>().speed = 32f;
        this.enabled = false;
    }

    private void SetMainCamCopy(int value)
    {
        if (value == 1)
        {
            _mainCamCopy.transform.position = _boundCamera.transform.position;
            _mainCamCopy.transform.rotation = _boundCamera.transform.rotation;
            _mainCamCopy.gameObject.SetActive(true);
            _boundCamera.gameObject.SetActive(false);
            _wayPoint1.transform.position = _boundCamera.transform.position;
            _wayPoint2.transform.position = new Vector3(_badGuy.transform.position.x, _mainCamCopy.transform.position.y, _badGuy.transform.position.z);
            StartScrollingCam();
        }
        else if (value == 2)
        {
            _boundCamera.gameObject.SetActive(false);
            _mainCamCopy.gameObject.SetActive(true);
            _wayPoint1.transform.position = _mainCamCopy.transform.position;
            _wayPoint2.transform.position = _mainCamOrigin;
            _mainCamCopy.GetComponent<PlatformMvt>().restart();
            StartCoroutine(waitForEnd());
        }
        else if (value == 3)
        {
            _boundCamera.target = _cellControl.transform;
            _mainCamCopy.gameObject.SetActive(false);
            _boundCamera.gameObject.SetActive(true);
            end();
        }
    }

    private void StartScrollingCam()
    {
        _mainCamCopy.GetComponent<PlatformMvt>().enabled = true;
        StartCoroutine(waitForSecondPart());
    }

    private void CutSceneSecondPart()
    {
        //_badGuy.GetComponent<iTweenEvent>().enabled = true;
        _badGuy.GetComponent<PlatformMvt>().enabled = true;
        StartCoroutine(CutSceneFourthPart(0.5f));
    }

    IEnumerator waitForSecondPart()
    {
        while (Vector3.Distance(_mainCamCopy.transform.position, new Vector3(_wayPoint2.transform.position.x, _wayPoint2.transform.position.y, _wayPoint2.transform.position.z)) >= 0.05f)
        {
            yield return null;
        }
        _boundCamera.target = _badGuy.transform;
        _mainCamCopy.gameObject.SetActive(false);
        _boundCamera.gameObject.SetActive(true);
        CutSceneSecondPart();
        yield return null;
    }

    IEnumerator CutSceneFourthPart(float duration)
    {
        while (duration >= 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        SetMainCamCopy(3);
        //SetMainCamCopy(2);
        yield return null;
    }

    IEnumerator waitForEnd()
    {
        while (Vector3.Distance(_mainCamCopy.transform.position, new Vector3(_wayPoint2.transform.position.x, _wayPoint2.transform.position.y, _wayPoint2.transform.position.z)) >= 0.5f)
        {
            yield return null;
        }
        SetMainCamCopy(3);
        yield return null;
    }
#endif
}

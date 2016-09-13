using UnityEngine;
using System.Collections;
using System;

public class CutScenePursuit : CutScene {

    [SerializeField]
    private GameObject _badGuy;
    [SerializeField]
    private Camera _mainCamCopy;
    [SerializeField]
    private Transform _wayPoint1;
    [SerializeField]
    private Transform _wayPoint2;
    private int _iteration = 0;
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            start();
        }
    }

    public override void startCutScene()
    {
        
        SetMainCamCopy(1);
        _iteration++;
        Debug.Log(_iteration);
    }

    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
        this.enabled = false;
    }

    public override void initialize()
    {
        
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
            _wayPoint1.transform.position = _mainCamCopy.transform.position;
            _wayPoint2.transform.position = _boundCamera.transform.position;
            _mainCamCopy.GetComponent<PlatformMvt>().restart();
            StartCoroutine(waitForEnd());
        }
        else if (value == 3)
        {
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
        _badGuy.GetComponent<iTweenEvent>().enabled = true;
        StartCoroutine(CutSceneFourthPart(1.5f));
    }

    IEnumerator waitForSecondPart()
    {
        while (Vector3.Distance(_mainCamCopy.transform.position, new Vector3(_wayPoint2.transform.position.x, _wayPoint2.transform.position.y, _wayPoint2.transform.position.z)) >= 0.05f)
        {
            yield return null;
        }
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
        SetMainCamCopy(2);
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
}

using UnityEngine;
using System.Collections;

public class BadGuyCutSceneTrigger : CutScene {

    private bool _first = true;
    [SerializeField]
    private iTweenEvent _iTweenEventBigGuy;
    private bool _isSlowingAnimation = false;
    [SerializeField]
    private float slowSpeed = 1;
    [SerializeField]
    private GameObject _deadBigBadGuy;
    private BoundCamera _mainCamBound;
    private Transform _initialTarget;
    [SerializeField]
    private Camera _cutSceneCam;
    [SerializeField]
    private GameObject _wayPoint1;
    [SerializeField]
    private GameObject _wayPoint2;
    [SerializeField]
    private GameObject _dummyFlagellum;

    // Use this for initialization
    void Start () {
        _mainCamBound = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BoundCamera>();
        _initialTarget = _mainCamBound.target;
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "CutSceneElement")
        {
            _isSlowingAnimation = true;
            StartCoroutine(killDummy(_iTweenEventBigGuy.gameObject));
        }

        if (col.tag == "Player")
        {
            if (_first == true)
            {
                _first = false;
                start();
            }
        }
    }

    public override void startCutScene()
    {
        _cutSceneCam.transform.position = _mainCamBound.transform.position;
        _cutSceneCam.transform.rotation = _mainCamBound.transform.rotation;
        _mainCamBound.target = _iTweenEventBigGuy.gameObject.transform;
        _mainCamBound.gameObject.SetActive(false);
        _cutSceneCam.gameObject.SetActive(true);
        _wayPoint1.transform.position = _cutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z);
        StartCoroutine(waitForSecondPart());
    }

    IEnumerator waitForSecondPart()
    {
        while(_cutSceneCam.transform.position != new Vector3(_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z))
        {
            yield return true;
        }
        CutSceneSecondPart();
        yield return null;
    }

    void CutSceneSecondPart()
    {
        _iTweenEventBigGuy.enabled = true;
        _mainCamBound.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
    }

    IEnumerator killDummy(GameObject dummy)
    {
        _iTweenEventBigGuy.GetComponent<DeathDummy>().StartDeath();
        StartCoroutine(waitForDummyDeath());
        yield return null;
    }

    IEnumerator waitForDummyDeath()
    {
        while(_dummyFlagellum.activeSelf == true)
        {
            yield return null;
        }
        StartCoroutine(prepareEnd());
        yield return null;
    }

    IEnumerator prepareEnd()
    {
        yield return new WaitForSeconds(2);
        
        _wayPoint1.transform.position = new Vector3 (_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y,_iTweenEventBigGuy.transform.position.z);
        _wayPoint2.transform.position = new Vector3 (_initialTarget.position.x,_cutSceneCam.transform.position.y,_initialTarget.position.z);
        _cutSceneCam.transform.position = _wayPoint1.transform.position;
        _cutSceneCam.gameObject.SetActive(true);
        _mainCamBound.gameObject.SetActive(false);
        _cutSceneCam.GetComponent<PlatformMvt>().restart();
        StartCoroutine(waitForEnd());
        
        yield return null;
    }

    IEnumerator waitForEnd()
    {
        while (_cutSceneCam.transform.position != new Vector3(_wayPoint2.transform.position.x, _wayPoint2.transform.position.y, _wayPoint2.transform.position.z))
        {
            yield return true;
        }
        
        end ();
        yield return null;
    }
    
    public override void endCutScene ()
    {
        _mainCamBound.target = _initialTarget;
        _cutSceneCam.gameObject.SetActive(false);
        _mainCamBound.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        ModalManager.setModal("T1_AMPICILLIN");
    }
}

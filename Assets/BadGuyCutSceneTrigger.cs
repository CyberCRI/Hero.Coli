using UnityEngine;
using System.Collections;

public class BadGuyCutSceneTrigger : MonoBehaviour {

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
    private CellControl _cellControl;
    [SerializeField]
    private Camera _cutSceneCam;
    [SerializeField]
    private GameObject _wayPoint1;
    [SerializeField]
    private GameObject _wayPoint2;

    // Use this for initialization
    void Start () {
        _mainCamBound = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BoundCamera>();
        _initialTarget = _mainCamBound.target;
        _cellControl = GameObject.FindGameObjectWithTag("Player").GetComponent<CellControl>();
	}
	
	// Update is called once per frame
	void Update () {

        /*if (_cutSceneCam.transform.position == new Vector3 (_wayPoint2.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z))
        {
            CutSceneSecondPart();
        }*/
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "CutSceneElement")
        {
            _isSlowingAnimation = true;
            StartCoroutine(SlowAnimation(col.gameObject));
        }

        if (col.tag == "Player")
        {
            if (_first == true)
            {
                _first = false;
                StartCutScene();
            }
        }
    }

    IEnumerator SlowAnimation(GameObject bigbadGuy)
    {
        var anim = bigbadGuy.GetComponent<Animation>() as Animation;
        while (_isSlowingAnimation == true)
        {
            float animSPeed = anim["medusa_slow"].speed;
            animSPeed -= 1 / (100 / slowSpeed);
            anim["medusa_slow"].speed = animSPeed;
            if (animSPeed <= 0)
            {
                _isSlowingAnimation = false;
                //var instance = Instantiate(_deadBigBadGuy, bigbadGuy.transform.position, bigbadGuy.transform.rotation) as GameObject;
                _deadBigBadGuy.SetActive(true);
                bigbadGuy.gameObject.SetActive(false);
                StartCoroutine(EndCutScene());
            }
            yield return null;
        }

        yield return null;
    }

    IEnumerator WaitForSecondPart()
    {
        while(_cutSceneCam.transform.position != new Vector3(_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z))
        {
            yield return true;
        }
        CutSceneSecondPart();
        yield return null;
    }

    IEnumerator EndCutScene()
    {
        yield return new WaitForSeconds(2);
        _wayPoint1.transform.position = new Vector3 (_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y,_iTweenEventBigGuy.transform.position.z);
        _wayPoint2.transform.position = new Vector3 (_initialTarget.position.x,_cutSceneCam.transform.position.y,_initialTarget.position.z);
        _cutSceneCam.transform.position = _wayPoint1.transform.position;
        _cutSceneCam.gameObject.SetActive(true);
        _mainCamBound.gameObject.SetActive(false);
        _cutSceneCam.GetComponent<PlatformMvt>().restart();
        StartCoroutine(WaitForEnd());
        yield return null;
    }

    IEnumerator WaitForEnd()
    {
        while (_cutSceneCam.transform.position != new Vector3(_wayPoint2.transform.position.x, _wayPoint2.transform.position.y, _wayPoint2.transform.position.z))
        {
            yield return true;
        }
        _mainCamBound.target = _initialTarget;
        _cellControl.freezePlayer(false);
        _cutSceneCam.gameObject.SetActive(false);
        _mainCamBound.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        ModalManager.setModal("T1_AMPICILLIN");
        yield return null;
    }

    void StartCutScene()
    {
        _cutSceneCam.transform.position = _mainCamBound.transform.position;
        _cutSceneCam.transform.rotation = _mainCamBound.transform.rotation;
        _mainCamBound.target = _iTweenEventBigGuy.gameObject.transform;
        _mainCamBound.gameObject.SetActive(false);
        _cutSceneCam.gameObject.SetActive(true);
        _wayPoint1.transform.position = _cutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z);
        StartCoroutine(WaitForSecondPart());
        _cellControl.freezePlayer(true);
    }

    void CutSceneSecondPart()
    {
        _iTweenEventBigGuy.enabled = true;
        _mainCamBound.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
    }
}

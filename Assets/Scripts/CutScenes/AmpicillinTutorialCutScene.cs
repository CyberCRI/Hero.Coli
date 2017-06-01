// #define QUICKTEST

using UnityEngine;
using System.Collections;

public class AmpicillinTutorialCutScene : CutScene
{

#if QUICKTEST
    private const float endWaitDuration = 0.1f;
    private const float iTweenEventSpeed = 350f;
#else
    private const float endWaitDuration = 2f;
    private const float iTweenEventSpeed = 35f;
#endif

    private bool _first = true;
    [SerializeField]
    private iTweenEvent _iTweenEventBigGuy;
    private bool _isSlowingAnimation = false;
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
    public override void initialize()
    {
		_initialTarget = BoundCamera.instance.target;
        _iTweenEventBigGuy.Values.Remove("speed");
        _iTweenEventBigGuy.Values.Add("speed", iTweenEventSpeed);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "CutSceneElement")
        {
            _isSlowingAnimation = true;
            StartCoroutine(killDummy(_iTweenEventBigGuy.gameObject));
        }

        if (col.tag == Character.playerTag)
        {
            if (_first)
            {
                _first = false;
                start();
            }
        }
    }

    public override void startCutScene()
    {
        _cutSceneCam.transform.position = BoundCamera.instance.transform.position;
        _cutSceneCam.transform.rotation = BoundCamera.instance.transform.rotation;
        _cutSceneCam.fieldOfView = BoundCamera.instance.GetComponent<Camera>().fieldOfView;
        BoundCamera.instance.target = _iTweenEventBigGuy.gameObject.transform;
        BoundCamera.instance.gameObject.SetActive(false);
        _cutSceneCam.gameObject.SetActive(true);
        _wayPoint1.transform.position = _cutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z);
        StartCoroutine(waitForSecondPart());
    }

    IEnumerator waitForSecondPart()
    {
        while (_cutSceneCam.transform.position != new Vector3(_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z))
        {
            yield return true;
        }
        launchCutSceneSecondPart();
        yield return null;
    }

    void launchCutSceneSecondPart()
    {
        _iTweenEventBigGuy.enabled = true;
        BoundCamera.instance.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
    }

    IEnumerator killDummy(GameObject dummy)
    {
		Character.get ().GetComponentInChildren<CharacterSoundManager> ().OnCharacterDeath (CustomDataValue.AMPICILLIN);
        _iTweenEventBigGuy.GetComponent<DeathDummy>().startDeath();
        StartCoroutine(waitForDummyDeath());
        yield return null;
    }

    IEnumerator waitForDummyDeath()
    {
        while (_dummyFlagellum.activeSelf)
        {
            yield return null;
        }
        StartCoroutine(prepareEnd());
        yield return null;
    }

    IEnumerator prepareEnd()
    {
        yield return new WaitForSeconds(endWaitDuration);

        _wayPoint1.transform.position = new Vector3(_iTweenEventBigGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigGuy.transform.position.z);
        _wayPoint2.transform.position = new Vector3(_initialTarget.position.x, _cutSceneCam.transform.position.y, _initialTarget.position.z);
        _cutSceneCam.transform.position = _wayPoint1.transform.position;
        _cutSceneCam.gameObject.SetActive(true);
        BoundCamera.instance.gameObject.SetActive(false);
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
        end();
        yield return null;
    }

    public override void endCutScene()
    {
        BoundCamera.instance.target = _initialTarget;
        _cutSceneCam.gameObject.SetActive(false);
        BoundCamera.instance.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        ModalManager.setModal("T1_AMPICILLIN");
    }
}

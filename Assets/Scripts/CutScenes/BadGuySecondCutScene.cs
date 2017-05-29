using UnityEngine;
using System.Collections;

public class BadGuySecondCutScene : CutScene
{
    [SerializeField]
    private iTweenEvent _iTweenEventBigBadGuy;
    [SerializeField]
    private Camera _cutSceneCam;
    [SerializeField]
    private GameObject _dummyPlayer;
    [SerializeField]
    private Transform[] _wayPointCam;
    [SerializeField]
    private Transform[] _wayPointPlayer;
    private GameObject _player;
    private bool _hasSecondPartPlayed = false;
    private Vector3 _originWayPoint1;
    private Vector3 _originWayPoint2;

    [SerializeField]
    private EndGameCollider _endGameCollider;


    // Use this for initialization
    public override void initialize()
    {
        _player = GameObject.FindGameObjectWithTag(Character.playerTag);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hasSecondPartPlayed && Vector3.Distance(_cutSceneCam.transform.position, _wayPointCam[1].transform.position) <= 0.002)
        {
            _hasSecondPartPlayed = true;
            _cutSceneCam.gameObject.SetActive(false);
            BoundCamera.instance.gameObject.SetActive(true);
            //_iTweenEventBigBadGuy.enabled = true;
            _wayPointCam[0].transform.position = _originWayPoint2;
            _wayPointCam[1].transform.position = _originWayPoint1;
            StartCoroutine(waitForSecondPart());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            start();
        }
    }

    IEnumerator waitForSecondPart()
    {

        yield return new WaitForSeconds(1.5f);
        BoundCamera.instance.gameObject.SetActive(false);
        BoundCamera.instance.target = _dummyPlayer.transform;
        _cutSceneCam.gameObject.SetActive(true);
        while (Vector3.Distance(_cutSceneCam.transform.position, _wayPointCam[1].transform.position) > 0.002)
        {
            yield return null;
        }
        _dummyPlayer.GetComponent<PlatformMvt>().enabled = true;
        _iTweenEventBigBadGuy.enabled = true;
        BoundCamera.instance.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        GameStateController.get().FadeScreen(true, 2.5f);
        yield return new WaitForSeconds(2.5f);
        end();
        yield return null;
    }

    public override void startCutScene()
    {
        _dummyPlayer.transform.position = _player.transform.position;
        _dummyPlayer.transform.rotation = _player.transform.rotation;
        _player.SetActive(false);
        _dummyPlayer.SetActive(true);

        _cutSceneCam.transform.position = BoundCamera.instance.transform.position;
        _cutSceneCam.transform.rotation = BoundCamera.instance.transform.rotation;

        _wayPointCam[0].transform.position = _cutSceneCam.transform.position;
        _wayPointCam[0].transform.rotation = _cutSceneCam.transform.rotation;
        _originWayPoint1 = _wayPointCam[0].transform.position;
        _wayPointCam[1].transform.position = new Vector3(_iTweenEventBigBadGuy.transform.position.x, _cutSceneCam.transform.position.y, _iTweenEventBigBadGuy.transform.position.z);
        _wayPointCam[1].transform.rotation = _iTweenEventBigBadGuy.transform.rotation;
        _originWayPoint2 = _wayPointCam[1].transform.position;

        _wayPointPlayer[0].transform.position = _player.transform.position;
        _wayPointPlayer[0].transform.rotation = _player.transform.rotation;

        _cutSceneCam.gameObject.SetActive(true);
        _cutSceneCam.GetComponent<PlatformMvt>().restart();
        BoundCamera.instance.gameObject.SetActive(false);

        BoundCamera.instance.target = _iTweenEventBigBadGuy.transform;
    }

    public override void endCutScene()
    {
    }
}

using UnityEngine;
using System.Collections;

public class BadGuySecondCutScene : CutScene {

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
    private bool _secondPart = false;
    private Vector3 _originWayPoint1;
    private Vector3 _originWayPoint2;

    [SerializeField]
    private EndGameCollider _endGameCollider;


	// Use this for initialization
	public override void initialize() 
    {
        _player = GameObject.FindGameObjectWithTag(Hero.playerTag);
    }
	
	// Update is called once per frame
	void Update () {
	    
        if (_secondPart == false && Vector3.Distance(_cutSceneCam.transform.position, _wayPointCam[1].transform.position) <= 0.002)
        {
            _secondPart = true;
            _cutSceneCam.gameObject.SetActive(false);
            _boundCamera.gameObject.SetActive(true);
            //_iTweenEventBigBadGuy.enabled = true;
            _wayPointCam[0].transform.position = _originWayPoint2;
            _wayPointCam[1].transform.position = _originWayPoint1;
            StartCoroutine(WaitForSecondPart());
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag)
        {
            start ();
        }
    }

    IEnumerator WaitForSecondPart()
    {

        yield return new WaitForSeconds(1.5f);
        _boundCamera.gameObject.SetActive(false);
        _boundCamera.target = _dummyPlayer.transform;
        _cutSceneCam.gameObject.SetActive(true);
        while (Vector3.Distance(_cutSceneCam.transform.position, _wayPointCam[1].transform.position) > 0.002)
        {
            yield return null;
        }
        _dummyPlayer.GetComponent<PlatformMvt>().enabled = true;
        _iTweenEventBigBadGuy.enabled = true;
        _boundCamera.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        GameStateController.get().FadeScreen(true, 2.5f);
        yield return new WaitForSeconds(2.5f);
        end ();
        yield return null;
    }
    
    public override void startCutScene ()
    {        
        _dummyPlayer.transform.position = _player.transform.position;
        _dummyPlayer.transform.rotation = _player.transform.rotation;
        _player.SetActive(false);
        _dummyPlayer.SetActive(true);

        _cutSceneCam.transform.position = _boundCamera.transform.position;
        _cutSceneCam.transform.rotation = _boundCamera.transform.rotation;

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
        _boundCamera.gameObject.SetActive(false);

        _boundCamera.target = _iTweenEventBigBadGuy.transform;
    }    
    
    public override void endCutScene ()
    {
        //ModalManager.setModal("T1_END");
        _endGameCollider.triggerEnd ();
    }
}

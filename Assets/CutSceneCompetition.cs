using UnityEngine;
using System.Collections;
using System;

public class CutSceneCompetition : CutScene {

    [SerializeField]
    private GameObject _npc;
    [SerializeField]
    private Camera _cutSceneCam;
    [SerializeField]
    private GameObject _wayPoint1;
    [SerializeField]
    private GameObject _wayPoint2;
    [SerializeField]
    private GameObject _wayPointNPC1;
    [SerializeField]
    private GameObject _wayPointNPC2;
    [SerializeField]
    private float _offset = 40f;
	[SerializeField]
	private Transform _camPosition;


    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void startCutScene()
    {
        _cutSceneCam.transform.position = _boundCamera.transform.position;
        _cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
        _boundCamera.target = _npc.gameObject.transform;
        _boundCamera.offset = new Vector3(_boundCamera.offset.x, _boundCamera.offset.y + _offset, _boundCamera.offset.z);
        _wayPoint1.transform.position = _cutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_npc.transform.position.x, _cutSceneCam.transform.position.y + _offset, _npc.transform.position.z);
        _boundCamera.gameObject.SetActive(false);
        _cutSceneCam.gameObject.SetActive(true);
        StartCoroutine(waitForSecondPart());
    }

    public override void endCutScene()
    {
		Debug.Log ("end");
		_cellControl.freezePlayer (false);
		//this.enabled = false;
    }

    public override void initialize()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            start();
        }

		if (col.tag == "Door") 
		{
			Debug.Log ("Door");
			_cutSceneCam.transform.position = _boundCamera.transform.position;
			_cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
			_boundCamera.target = _cellControl.gameObject.transform;
			_boundCamera.offset = new Vector3(_boundCamera.offset.x, _boundCamera.offset.y - _offset, _boundCamera.offset.z);
			_wayPoint1.transform.position = _cutSceneCam.transform.position;
			_wayPoint2.transform.position = new Vector3(_cellControl.transform.position.x, _cutSceneCam.transform.position.y - _offset, _cellControl.transform.position.z);
			_boundCamera.gameObject.SetActive(false);
			_cutSceneCam.gameObject.SetActive(true);
			_cutSceneCam.GetComponent<PlatformMvt> ().restart ();
			StartCoroutine(waitForCamReset());
		}
    }    

    IEnumerator waitForSecondPart()
    {
        while (Vector3.Distance(_cutSceneCam.transform.position, new Vector3 (_npc.transform.position.x, _cutSceneCam.transform.position.y, _npc.transform.position.z)) >= 0.05f)
        {
            Debug.Log("wait");
			yield return null;
        }
        launchCutSceneSecondPart();
        yield return null;
    }

    void launchCutSceneSecondPart()
    {
		_npc.GetComponent<PlatformMvt>().enabled = true;
        _boundCamera.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
        StartCoroutine(waitForThirdPart());
    }

    IEnumerator waitForThirdPart()
    {
        while (Vector3.Distance(_npc.transform.position, _wayPointNPC2.transform.position) >= 0.05f)
        {
            Debug.Log("wait");
            yield return true;
        }
        launchCutSceneThirdPart();
        yield return null;
    }

    void launchCutSceneThirdPart()
    {
        Debug.Log("Third");
		_cutSceneCam.transform.position = _boundCamera.transform.position;
		_cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
		_boundCamera.target = _camPosition;
		_boundCamera.offset = new Vector3(_boundCamera.offset.x, _boundCamera.offset.y, _boundCamera.offset.z);
		_wayPoint1.transform.position = _cutSceneCam.transform.position;
		_wayPoint2.transform.position = new Vector3(_camPosition.position.x, _cutSceneCam.transform.position.y, _camPosition.transform.position.z);
		_boundCamera.gameObject.SetActive(false);
		_cutSceneCam.gameObject.SetActive(true);
		StartCoroutine(waitForEnd());
    }

	IEnumerator waitForEnd()
	{
		yield return new WaitForSeconds (3f);
		_boundCamera.gameObject.SetActive(true);
		_cutSceneCam.gameObject.SetActive(false);
		_camPosition.GetComponent<PlatformMvt> ().enabled = true;
		yield return new WaitForSeconds(2f);
		end ();
		yield return null;
	}

	IEnumerator waitForCamReset()
	{
		yield return new WaitForSeconds (2f);
		_boundCamera.gameObject.SetActive(true);
		_cutSceneCam.gameObject.SetActive(false);
		yield return null;
	}
}

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
    private float _iTweenSpeed;
    [SerializeField]
    private iTweenEvent _iTweenNPC;


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

        if (col.tag == "NPC")
        {
            _iTweenNPC.GetComponent<iTween>().isRunning = false;
            StartCoroutine(npcWait());
        }
    }    

    IEnumerator waitForSecondPart()
    {
        while (Vector3.Distance(_cutSceneCam.transform.position, new Vector3 (_npc.transform.position.x, _cutSceneCam.transform.position.y, _npc.transform.position.z)) >= 0.05f)
        {
            Debug.Log("wait");
            yield return true;
        }
        launchCutSceneSecondPart();
        yield return null;
    }

    void launchCutSceneSecondPart()
    {
        _iTweenNPC.enabled = true;
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
    }

    IEnumerator npcWait()
    {
        yield return new WaitForSeconds(2f);
        _iTweenNPC.GetComponent<iTween>().isRunning = true;
        yield return null;
    }

    void ResetSpeed()
    {
        _iTweenNPC.Values.Remove("speed");
        _iTweenNPC.Values.Add("speed", _iTweenSpeed);
    }
}

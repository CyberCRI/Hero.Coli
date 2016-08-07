using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmpicillinCutScene : CutScene {

    [SerializeField]
    private PlatformMvt _pltMvt2Flagellum;
    [SerializeField]
    private PlatformMvt _pltMvt3Flagellum;
    private BoundCamera _mainCam;
    private GameObject _player;
    [SerializeField]
    private Camera _cutSceneCam;
    private bool _moveCam = false;
    private bool _moveCam2 = false;
    [SerializeField]
    private List<GameObject> _points = new List<GameObject>();
    
    bool _played = false;
    
    /*
    // Use this for initialization
    void Start () {
        _mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BoundCamera>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
	    if (_moveCam == true)
        {
            _cutSceneCam.transform.position = Vector3.MoveTowards(_cutSceneCam.transform.position, new Vector3 (_iTween2Flagellum.transform.position.x,_cutSceneCam.transform.position.y,_iTween2Flagellum.transform.position.z ), 1f);
        }
        if (_moveCam2 == true)
        {
            Debug.Log("Hello World");
            _cutSceneCam.transform.position = Vector3.MoveTowards(_cutSceneCam.transform.position, new Vector3(_player.transform.position.x, _cutSceneCam.transform.position.y, _player.transform.position.z), 1f);
            Debug.Log(Vector3.Distance(_cutSceneCam.transform.position, new Vector3(_iTween2Flagellum.transform.position.x, _cutSceneCam.transform.position.y, _iTween2Flagellum.transform.position.z)));
            if (Vector3.Distance(_cutSceneCam.transform.position ,new Vector3(_iTween2Flagellum.transform.position.x, _cutSceneCam.transform.position.y, _iTween2Flagellum.transform.position.z)) >= 30)
            {
                end();
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            _player = col.gameObject;
            start();
        }
    }

    public override void startCutScene()
    {
        for (int i = 0 ; i < _iTween2Flagellum.transform.childCount; i++)
        {
            _iTween2Flagellum.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < _iTween3Flagellum.transform.childCount; i++)
        {
            _iTween3Flagellum.transform.GetChild(i).gameObject.SetActive(true);
        }
        _iTween2Flagellum.enabled = true;
        StartCoroutine(WaitForTarget());
    }
    
    public override void endCutScene ()
    {
        _mainCam.target = _player.transform;
        Destroy(this.transform.parent.gameObject);        
    }

    IEnumerator WaitForTarget()
    {
        _cutSceneCam.transform.position = _mainCam.transform.position;
        _cutSceneCam.transform.rotation = _mainCam.transform.rotation;
        _mainCam.target = _iTween2Flagellum.transform;
        _cutSceneCam.gameObject.SetActive(true);
        _mainCam.gameObject.SetActive(true);
        _moveCam = true;
        yield return new WaitForSeconds(2);
        _iTween3Flagellum.enabled = true;
        yield return null;
    }
    */
    public void ResetCamTarget()
    {
        _moveCam = false;
        _moveCam2 = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Door")
        {
            if(!_played)
            {
                start();
                _played = true;
            }
        }
        if (col.tag == "CutSceneElement")
        {
            Destroy(col.gameObject);
        }
    }

    void StartEscape(PlatformMvt _pltMvt, float speed)
    {
        _pltMvt.points.Remove(_pltMvt.points[3]);
        for (int i = 0; i < 3; i++)
        {
            _pltMvt.points[i] = _points[i];
            _pltMvt.SetCurrentDestination(0);
            _pltMvt.speed = speed;
            _pltMvt.loop = false;
        }
    }

    public override void startCutScene()
    {
        StartEscape(_pltMvt2Flagellum, 10);
        StartCoroutine(WaitForSecondBacteria());
    }

    public override void endCutScene() {
        Destroy(this.transform.parent.gameObject);
    }

    IEnumerator WaitForSecondBacteria()
    {
        yield return new WaitForSeconds(4.5f);
        StartEscape(_pltMvt3Flagellum, 25);
        yield return new WaitForSeconds(3f);
        end();
        yield return null;
    }
}

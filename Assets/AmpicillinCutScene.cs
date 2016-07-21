using UnityEngine;
using System.Collections;

public class AmpicillinCutScene : MonoBehaviour {

    [SerializeField]
    private iTweenEvent _iTween2Flagellum;
    [SerializeField]
    private iTweenEvent _iTween3Flagellum;
    private CellControl _cellControl;
    private BoundCamera _mainCam;
    private GameObject _player;
    [SerializeField]
    private Camera _cutSceneCam;
    private bool _moveCam = false;
    private bool _moveCam2 = false;

    // Use this for initialization
    void Start () {
        _mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BoundCamera>();
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
                _mainCam.target = _player.transform;
                _cellControl.FreezePLayer(false);
                Destroy(this.gameObject.transform.parent.gameObject);
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            _player = col.gameObject;
            _cellControl = col.GetComponent<CellControl>();
            StartCutScene();
        }
    }

    void StartCutScene()
    {
        _cellControl.FreezePLayer(true);
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

    public void ResetCamTarget()
    {
        _moveCam = false;
        _moveCam2 = true;
    }
}

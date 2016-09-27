using UnityEngine;

public class InstantiateObject : CutSceneElements
{
    [SerializeField]
    private GameObject _toInstantiate;
    [SerializeField]
    private Transform _originTransform;
    [SerializeField]
    private bool _reInstantiate;
    [SerializeField]
    private bool _onTrigger;
    [SerializeField]
    private string _triggerTag;
    private bool _first = true;
    private Transform _resetCamera;
    private GameObject _newInstance;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (_onTrigger == true)
        {
            if (col.tag == _triggerTag)
            {
                if (_reInstantiate == true)
                {
                    if (_newInstance != null)
                    {
                        Destroy(_newInstance);
                    }
                    _newInstance = (GameObject)Instantiate(_toInstantiate, _originTransform.position, _originTransform.rotation);
                }
                else
                {
                    Instantiate(_toInstantiate, _originTransform.position, _originTransform.rotation);
                }

                if (_first)
                {
                    _resetCamera = _boundCamera.transform;
                    _first = false;
                }
                else
                {
                    _boundCamera.transform.position = _resetCamera.position;
                    _boundCamera.transform.rotation = _resetCamera.rotation;
                    _boundCamera.target = _cellControl.gameObject.transform;
                    _boundCamera.gameObject.SetActive(true);
                    _cutSceneCamera.enabled = false;
                    _blackBar.closeBar(false);
                    _cellControl.freezePlayer(false);
                }
            }
        }
    }
}

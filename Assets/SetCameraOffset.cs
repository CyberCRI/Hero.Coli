using UnityEngine;
using System.Collections;

public class SetCameraOffset : MonoBehaviour {

    [SerializeField]
    private float _offSetValue;
    [SerializeField]
    private float _timeToSetCam;
    [SerializeField]
    private bool _resetCam = false;
    private BoundCamera _boundCam;
    private float _originOffsetY;

	// Use this for initialization
	void Start () {
        _boundCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BoundCamera>();
        _originOffsetY = _boundCam.offset.y;
        Debug.Log(_boundCam);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if (_resetCam == false)
            {
                _boundCam.ZoomInOut(_offSetValue, _timeToSetCam);
            }
            else if (_resetCam == true)
            {
                _boundCam.ZoomInOut(_originOffsetY, _timeToSetCam);
            }
        }
    }
}

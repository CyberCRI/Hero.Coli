using UnityEngine;

public class SetCameraOffset : MonoBehaviour {

    [SerializeField]
    private float _offSetValue;
    [SerializeField]
    private float _timeToSetCam;
    [SerializeField]
    private bool _resetCam = false;
    private float _originOffsetY;

	// Use this for initialization
	void Start () {
        _originOffsetY = GUITransitioner.get().mainBoundCamera.offset.y;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag)
        {
            if (_resetCam == false)
            {
                GUITransitioner.get().mainBoundCamera.ZoomInOut(_offSetValue, _timeToSetCam);
            }
            else if (_resetCam == true)
            {
                GUITransitioner.get().mainBoundCamera.ZoomInOut(_originOffsetY, _timeToSetCam);
            }
        }
    }
}

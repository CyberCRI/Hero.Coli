using UnityEngine;
using System.Collections;

public class BoundCamera : MonoBehaviour {
	public Transform target;
	public bool useScenePosition = true;
	public Vector3 offset;
	
	public bool _transition = true;
	public bool _zoomed = false;
	public float zoomedCameraDistanceMin = 5;
	public float cameraDistanceMin = 20;
  	public float cameraDistanceMax = 75;
      
      [SerializeField]
  	private float scrollSpeed = 5;
      
	public float zoomSmooth = 3;
	public float fovZoomed = 10.0f;
	
	private float fovUnzoomed;
	private float fov;
	
    private float _timeAtLastFrame = 0f;
    private float _timeAtCurrentFrame = 0f;
    private float deltaTime = 0f;
	
	public void SetZoom(bool zoomIn) {
		_zoomed = zoomIn;
		if(zoomIn) {
			fov = fovZoomed;
		} else {
			fov = fovUnzoomed;
		}
	}

    private float _zoomingTime;
    private float _originZoomingTime;
    private float _originalOffset;
    private bool _isZooming;
    private Coroutine _currentCoroutine;
	
	// Use this for initialization
	void Start () {
		if(useScenePosition){
          offset = new Vector3(0, transform.position.y, 0);
		}
		fovUnzoomed = GetComponent<Camera>().fieldOfView;
		fov = fovUnzoomed;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		_timeAtCurrentFrame = Time.realtimeSinceStartup;
        deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame;
		
		transform.position = target.position + offset;
		
		if(_transition){
			/*
			if(!_zoomed) {
				fov = Mathf.Clamp(fov - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, cameraDistanceMin, cameraDistanceMax);
			}
			*/
			GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fov, deltaTime * zoomSmooth);
		}
    }

    public void ZoomInOut(float YOffset, float zoomingTime)
    {
        if (_isZooming == true)
        {
            StopCoroutine(_currentCoroutine);
        }
        _zoomingTime = zoomingTime;
        _originZoomingTime = zoomingTime;
        var offSetDif = -(offset.y - YOffset);
        _currentCoroutine = StartCoroutine(Zoom(offSetDif,offset.y));
    }

    IEnumerator Zoom(float YOffset, float originalOffset)
    {
        _isZooming = true;
        while(_zoomingTime > 0)
        {
            _zoomingTime -= Time.deltaTime;
            offset.y = originalOffset + (YOffset * (1 - (_zoomingTime / _originZoomingTime)));
            yield return null;
        }
        _isZooming = false;
        yield return null;
    }
}

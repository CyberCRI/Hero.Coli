using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour {
	public Transform target;
	public bool useScenePosition = true;
	public Vector3 offset;
	
	public bool _transition = true;
	public bool _zoomed = false;
	public float zoomedCameraDistanceMin = 5;
	public float cameraDistanceMin = 20;
  	public float cameraDistanceMax = 75;
  	public float scrollSpeed = 5;
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
	
	// Use this for initialization
	void Start () {
		if(useScenePosition){
		  offset = transform.position - target.position;
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
			if(!_zoomed) {
				fov = Mathf.Clamp(fov + Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, cameraDistanceMin, cameraDistanceMax);
			}
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, deltaTime * zoomSmooth);
		}
	}
}

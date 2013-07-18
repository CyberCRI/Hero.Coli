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
	
	private float fov;
	
	public void SetZoom(bool zoomIn) {
		_zoomed = zoomIn;
		if(zoomIn) {
			fov = 10.0f;
		} else {
			fov = 65.0f;
		}
	}
	
	// Use this for initialization
	void Start () {
		if(useScenePosition)
			offset = transform.position - target.position;
		fov = GetComponent<Camera>().fieldOfView;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = target.position + offset;
		
		if(_transition){
			if(!_zoomed) {
				fov = Mathf.Clamp(fov + Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, cameraDistanceMin, cameraDistanceMax);
			}
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, Time.deltaTime * zoomSmooth);
		}
	}
}

using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour {
	public Transform target;
	public bool useScenePosition = true;
	public Vector3 offset;
	
	public bool zoom = true;
	public float cameraDistanceMin = 20;
  	public float cameraDistanceMax = 75;
  	public float scrollSpeed = 5;
	public float zoomSmooth = 3;
	
	private float fov;
	
	// Use this for initialization
	void Start () {
		if(useScenePosition)
			offset = transform.position - target.position;
		fov = GetComponent<Camera>().fieldOfView;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = target.position + offset;
		
		if(zoom){
			fov = Mathf.Clamp(fov + Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, cameraDistanceMin, cameraDistanceMax);
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, Time.deltaTime * zoomSmooth);
		}
	}
}

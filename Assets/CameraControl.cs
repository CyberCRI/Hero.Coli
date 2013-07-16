using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour{

	public float moveSpeed = 9f;
	public float rotationSpeed = 6f;

	public bool zoom = true;
	public float cameraDistanceMin = 20;
  	public float cameraDistanceMax = 75;
  	public float scrollSpeed = 5;
	public float zoomSmooth = 3;

	private float fov;
	
	void Start (){
		fov = camera.fov;
	}
  
	void Update(){
		//Keyboard controls
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			//Rotation
			//float rotation = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;
			//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * rotationSpeed);
			//Translate
			Vector3 inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));;
			transform.Translate(inputMovement * moveSpeed * Time.deltaTime, Space.World);
		}
		
		if(zoom){
			fov = Mathf.Clamp(fov + Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, cameraDistanceMin, cameraDistanceMax);
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, Time.deltaTime * zoomSmooth);
		}
	}

}
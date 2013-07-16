using UnityEngine;
using System.Collections;

public class CellControl : MonoBehaviour{

	public float moveSpeed = 9f;
	public float rotationSpeed = 6f;
	
	//Getter & setter for the move speed.
	
	//References for the click to move controler.
	//	private float _moveSmooth = .5f;
	//	private Vector3 _destination = Vector3.zero;

	//Getter & setter for the inventory
	void Start (){
	}
  
	void Update(){
		//Keyboard controls
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			//Rotation
			float rotation = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * rotationSpeed);
			//Translate
			Vector3 inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));;
			transform.Translate(inputMovement * moveSpeed * Time.deltaTime, Space.World);
		}
	}

}
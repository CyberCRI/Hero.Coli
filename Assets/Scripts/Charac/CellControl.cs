using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellControl : MonoBehaviour{

	public float moveSpeed = 9f;
	public float rotationSpeed = 6f;
	public List<Animation> anims;
	
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
			Vector3 inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			this.collider.attachedRigidbody.AddForce(inputMovement * moveSpeed);
			
			//SetSpeed
			float speed = Mathf.Abs(Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")))) + 0.3f;
			Animation[] anims = GetComponentsInChildren<Animation>();
			foreach(Animation anim in anims)
				foreach (AnimationState state in anim) {
        			state.speed = speed;
        }
		}
	}

}
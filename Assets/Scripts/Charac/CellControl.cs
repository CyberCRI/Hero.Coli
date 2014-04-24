using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellControl : MonoBehaviour{

	public float baseMoveSpeed;
	public float rotationSpeed = 6f;
	public List<Animation> anims;
  public Hero hero;
  public float moveEnergyCost;

  public float currentMoveSpeed;

  private bool _pause;

  public void Pause(bool pause)
  {
    _pause = pause;
  }

  public bool isPaused()
  {
    return _pause;
  }
	
	//Getter & setter for the move speed.
	
	//References for the click to move controler.
	//	private float _moveSmooth = .5f;
	//	private Vector3 _destination = Vector3.zero;

	//Getter & setter for the inventory
	void Start (){
    gameObject.GetComponent<PhenoSpeed>().setBaseSpeed(baseMoveSpeed);
		GameObject.Find ("GUITransitioner").GetComponent<GUITransitioner>().control = this;
	}
  
	void Update(){
		//Keyboard controls
		if(!_pause && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)){
			//Rotation
			float rotation = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * rotationSpeed);
			
			//Translate
			Vector3 inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
      if(inputMovement.sqrMagnitude > 1) inputMovement /= Mathf.Sqrt(2);
      Vector3 moveAmount = inputMovement * currentMoveSpeed;

			this.collider.attachedRigidbody.AddForce(moveAmount);

      float cost = moveAmount.sqrMagnitude*moveEnergyCost;
      //Logger.Log ("sqrInputMovementMagnitude="+inputMovement.sqrMagnitude, Logger.Level.ONSCREEN);
      hero.subEnergy(cost);

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
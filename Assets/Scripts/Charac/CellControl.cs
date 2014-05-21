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

  //Click to move variables    
  private int _smooth; // Determines how quickly object moves towards position
  private float _hitdist = 0.0f;
  private Vector3 _targetPosition;
    
  private enum ControlType {
      RightClickToMove,
      LeftClickToMove,
      AbsoluteWASD,
      RelativeWASD
  };
  private ControlType _currentControlType = ControlType.LeftClickToMove;


  public void Pause(bool pause)
  {
    _pause = pause;
  }

  public bool isPaused()
  {
    return _pause;
    }

  private void ClickToMoveUpdate(KeyCode mouseButtonCode) {
    if(Input.GetKeyDown(mouseButtonCode))            
    {
        _smooth = 1;
        
        Plane playerPlane = new Plane(Vector3.up, transform.position);            
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);            
        
        if (playerPlane.Raycast (ray, out _hitdist)) {                
            _targetPosition = ray.GetPoint(_hitdist);     
            transform.rotation = Quaternion.LookRotation(_targetPosition - transform.position);
        }
    }
    transform.position = Vector3.Slerp (transform.position, _targetPosition, Time.deltaTime * _smooth);    
  }
    
  private void AbsoluteWASDUpdate() {
    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
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
      foreach(Animation anim in anims) {
        foreach (AnimationState state in anim) {
          state.speed = speed;
        }
      }  
    }
  }
	
	void Start (){
    gameObject.GetComponent<PhenoSpeed>().setBaseSpeed(baseMoveSpeed);
	}
  
	void Update(){
		//Keyboard controls
		if(!_pause) {
      switch(_currentControlType) {
        case ControlType.LeftClickToMove:
          ClickToMoveUpdate(KeyCode.Mouse0);
          break;
        case ControlType.RightClickToMove:
          ClickToMoveUpdate(KeyCode.Mouse1);
          break;
        case ControlType.AbsoluteWASD:
          AbsoluteWASDUpdate();
          break;
        case ControlType.RelativeWASD:
          AbsoluteWASDUpdate();
          break;
        default:
          AbsoluteWASDUpdate();
          break;
      }
    }
  }
}
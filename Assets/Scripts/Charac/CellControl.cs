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
  private Vector3 _inputMovement;
  /* 
   * Click to move variables
   */
  private int _smooth; // Determines how quickly object moves towards position
  private float _hitdist = 0.0f;
  private Vector3 _targetPosition;
    
  private enum ControlType {
      RightClickToMove,
      LeftClickToMove,
      AbsoluteWASD,
      RelativeWASD
  };
  private ControlType _currentControlType = ControlType.RightClickToMove;


  public void Pause(bool pause)
  {
    _pause = pause;
  }

  public bool isPaused()
  {
    return _pause;
  }

  private void ClickToMoveUpdate(KeyCode mouseButtonCode) {
    Vector3 lastTickPosition = transform.position;
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
    _inputMovement = (_targetPosition - transform.position);    
    _inputMovement = new Vector3(_inputMovement.x, 0, _inputMovement.z);
        Logger.Log("ClickToMoveUpdate: _inputMovement="+_inputMovement+"; "+_inputMovement+".sqrMagnitude="+_inputMovement.sqrMagnitude, Logger.Level.ONSCREEN);
    if(_inputMovement.sqrMagnitude <= 5f) {
      stopMovement();
    } else {
      _inputMovement = _inputMovement.normalized;
    }
    Logger.Log("ClickToMoveUpdate: _inputMovement="+_inputMovement, Logger.Level.ONSCREEN);

  }
    
  private void AbsoluteWASDUpdate() {
    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
      Logger.Log("key input=["+Input.GetAxis("Horizontal")+";"+Input.GetAxis("Vertical")+"]", Logger.Level.ONSCREEN);
      //Rotation
      float rotation = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * rotationSpeed);
            
      //Translate
      _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
      if(_inputMovement.sqrMagnitude > 1) _inputMovement /= Mathf.Sqrt(2);
    } else if (Vector3.zero != _inputMovement) {
      stopMovement();
    }
  }

  private void stopMovement() {        
    _inputMovement = Vector3.zero;
    _targetPosition = transform.position;
    setSpeed();
  }

  private void commonUpdate() {
    if(Vector3.zero != _inputMovement) {
      Vector3 moveAmount = _inputMovement * currentMoveSpeed;
      
      this.collider.attachedRigidbody.AddForce(moveAmount);
      
      updateEnergy(moveAmount);

      setSpeed();
    }
  }

  private void setSpeed() {   
    //SetSpeed
    float speed = _inputMovement.sqrMagnitude + 0.3f;
    Logger.Log("commonUpdate: speed="+speed, Logger.Level.ONSCREEN);
    Animation[] anims = GetComponentsInChildren<Animation>();
    foreach(Animation anim in anims) {
      foreach (AnimationState state in anim) {
        state.speed = speed;
      }
    }
  }

  private void updateEnergy(Vector3 moveAmount) {
    float cost = moveAmount.sqrMagnitude*moveEnergyCost;
    hero.subEnergy(cost);
    Logger.Log("control="+_currentControlType
    +"\nupdateEnergy("+moveAmount+")"
    +"\n=> -"+cost, Logger.Level.ONSCREEN);
  }
	
	void Start (){
    gameObject.GetComponent<PhenoSpeed>().setBaseSpeed(baseMoveSpeed);

    _targetPosition = transform.position;
	}
  
	void Update(){
		//Keyboard controls
		if(!_pause) {

      _inputMovement = Vector3.zero;

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
      commonUpdate();
    }
    if(Input.GetKeyDown(KeyCode.Space)) {
      if (_currentControlType == ControlType.RightClickToMove) {
          _currentControlType = ControlType.AbsoluteWASD;
      } else {
          _currentControlType = ControlType.RightClickToMove;
      }
    }
  }
}
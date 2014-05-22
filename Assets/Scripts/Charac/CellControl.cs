using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellControl : MonoBehaviour{

	public float baseMoveSpeed;
	public float rotationSpeed;
  public float relativeRotationSpeed;
	public List<Animation> anims;
  public Hero hero;
  public float moveEnergyCost;
  public float currentMoveSpeed;
  public string wallname;

  private bool _pause;
  private Vector3 _inputMovement;
  /* 
   * Click to move variables
   */
  private int _smooth; // Determines how quickly object moves towards position
  private float _hitdist = 0.0f;
  private Vector3 _targetPosition;
    
  private enum ControlType {
      RightClickToMove = 0,
      LeftClickToMove = 1,
      AbsoluteWASD = 2,
      RelativeWASD = 3
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
      }
    }

    Vector3 aim = _targetPosition - transform.position;    
    _inputMovement = new Vector3(aim.x, 0, aim.z);

    if(_inputMovement.sqrMagnitude <= 5f) {
      stopMovement();
    } else {
      _inputMovement = _inputMovement.normalized;
    }

    rotationUpdate();
  }
    
  private void AbsoluteWASDUpdate() {
    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {

      Logger.Log("key input=["+Input.GetAxis("Horizontal")+";"+Input.GetAxis("Vertical")+"]", Logger.Level.ONSCREEN);
            
      //Translate
      _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
      if(_inputMovement.sqrMagnitude > 1) _inputMovement /= Mathf.Sqrt(2);

      rotationUpdate();

    } else if (Vector3.zero != _inputMovement) {
      stopMovement();
    }
  }

  private void RelativeWASDUpdate() {

    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {

      float norm = Input.GetAxis("Vertical");
      if(norm < 0) norm = 0;
      
      float deltaAngle = Input.GetAxis("Horizontal");

      transform.RotateAround(transform.position, Vector3.up, deltaAngle * relativeRotationSpeed);

      float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
      _inputMovement = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * norm;
            
      Logger.Log("key input=["+Input.GetAxis("Horizontal")+";"+Input.GetAxis("Vertical")+"]"
        +"\nnorm="+norm
        +"\ndeltaAngle="+deltaAngle
        +"\nangle="+angle
        +"\n_inputMovement="+_inputMovement
        , Logger.Level.ONSCREEN);
    }
  }

  private void stopMovement() {        
    _inputMovement = Vector3.zero;
    _targetPosition = transform.position;
    setSpeed();
  }

  private void rotationUpdate() {
    if(Vector3.zero != _inputMovement) {
      //Rotation
      float rotation = Mathf.Atan2(_inputMovement.x, _inputMovement.z) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * rotationSpeed);
    }
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
  }
	
	void Start (){
    gameObject.GetComponent<PhenoSpeed>().setBaseSpeed(baseMoveSpeed);

    _targetPosition = transform.position;
	}
  
	void Update(){
		//Keyboard controls
		if(!_pause) {

      _inputMovement = Vector3.zero;

      Logger.Log(_currentControlType.ToString(), Logger.Level.ONSCREEN);

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
          RelativeWASDUpdate();
          break;
        default:
          AbsoluteWASDUpdate();
          break;
      }
      commonUpdate();

      float angle = transform.rotation.eulerAngles.y;
      Logger.Log("transform.rotation.eulerAngles="+transform.rotation.eulerAngles
             +"\ntransform.rotation="+transform.rotation
             +"\n_inputMovement="+_inputMovement
             +"\nvec="+new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle))
             , Logger.Level.ONSCREEN);
    }
    if(Input.GetKeyDown(KeyCode.Space)) {
      _currentControlType = (ControlType)(((int)_currentControlType + 1) % 4);
      _targetPosition = transform.position;
    }
  }
    
  void OnCollisionStay(Collision col) {
    if ((Vector3.zero != _inputMovement) && col.collider && (wallname == col.collider.name)){
      stopMovement();
    }
  }
}
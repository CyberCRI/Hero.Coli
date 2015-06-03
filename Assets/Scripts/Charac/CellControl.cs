using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellControl : MonoBehaviour{

	public float baseMoveSpeed;
	public float rotationSpeed;
  public float relativeRotationSpeed;
  public Hero hero;
  public float moveEnergyCost;
  public float currentMoveSpeed;
  public string wallname;

  public CellControlButton absoluteWASDButton;
  public CellControlButton leftClickToMoveButton;
  public CellControlButton relativeWASDButton;
  public CellControlButton rightClickToMoveButton;
  public UISprite selectedMouseControlTypeSprite;
  public UISprite selectedKeyboardControlTypeSprite;

  private bool _pause;
  private Vector3 _inputMovement;
  private SwimAnimator _swimAnimator;

  /* 
   * Click to move variables
   */
  private float _hitdist = 0.0f;
  private Vector3 _targetPosition;
  private bool _isFirstUpdate = false;

  //*
  public enum ControlType {
      RightClickToMove = 0,
      LeftClickToMove = 1,
      AbsoluteWASD = 2,
      RelativeWASD = 3
  };
  //*/
  //private ControlType _currentControlType = ControlType.AbsoluteWASDAndLeftClickToMove;
    private bool isLeftClickToMove = true;
    private bool isAbsoluteWASD = true;


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
    
      Plane playerPlane = new Plane(Vector3.up, transform.position);            
      Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);            
      
			if (playerPlane.Raycast (ray, out _hitdist) && !UICamera.hoveredObject) {                
        _targetPosition = ray.GetPoint(_hitdist);
      }
    }

    if(Vector3.zero == _inputMovement)
    {
      Vector3 aim = _targetPosition - transform.position;    
      _inputMovement = new Vector3(aim.x, 0, aim.z);

      if(_inputMovement.sqrMagnitude <= 5f) {
        stopMovement();
      } else {
        _inputMovement = _inputMovement.normalized;
      }

      rotationUpdate();
    }
  }
    
  private void AbsoluteWASDUpdate() {
    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {

      cancelMouseMove();

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

      cancelMouseMove();

      float norm = Input.GetAxis("Vertical");
      if(norm < 0) norm = 0;
      
      float deltaAngle = Input.GetAxis("Horizontal");

      transform.RotateAround(transform.position, Vector3.up, deltaAngle * relativeRotationSpeed);

      float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
      _inputMovement = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * norm;
    }
  }

  public void reset() {
    stopMovement();
  }

  private void cancelMouseMove() {
    _targetPosition = transform.position;
  }

  public void stopMovement() {        
    _inputMovement = Vector3.zero;
    cancelMouseMove();
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
      
      this.GetComponent<Collider>().attachedRigidbody.AddForce(moveAmount);
      
      updateEnergy(moveAmount);

      setSpeed();
    }
  }

  private void setSpeed()
  {
    //SetSpeed
    float speed = _inputMovement.sqrMagnitude + 0.3f;
    //TODO put variable for this
    if(null == _swimAnimator)
    {
      _swimAnimator = (SwimAnimator)gameObject.GetComponent<SwimAnimator>();
    }
    _swimAnimator.setSpeed(speed);
   }

  private void updateEnergy(Vector3 moveAmount) {
    float cost = moveAmount.sqrMagnitude*moveEnergyCost;
    hero.subEnergy(cost);
  }
	
	void Start (){
    Debug.LogError("CellControl::Start isLeftClickToMove="+isLeftClickToMove+" & isAbsoluteWASD="+isAbsoluteWASD);
    
    gameObject.GetComponent<PhenoSpeed>().setBaseSpeed(baseMoveSpeed);

    _targetPosition = transform.position;

        switchControlTypeToAbsoluteWASD();
        switchControlTypeToLeftClickToMove();
	}
  
    void Update ()
    {
        if(!_pause) {

            _inputMovement = Vector3.zero;
            
            //Keyboard controls
            if(isAbsoluteWASD)
            {
                AbsoluteWASDUpdate();
            } else {
                RelativeWASDUpdate();
            }

            //Mouse controls
            if(!_isFirstUpdate) {
                if(isLeftClickToMove) {
                    ClickToMoveUpdate(KeyCode.Mouse0);
                } else {
                    ClickToMoveUpdate(KeyCode.Mouse1);
                }
            } else { 
                    _isFirstUpdate = false;
            }
            commonUpdate();
        }
    }

  private void switchControlTypeTo(ControlType newControlType) {
    switch(newControlType) {
      case ControlType.AbsoluteWASD:
        switchControlTypeToAbsoluteWASD();
        break;
      case ControlType.LeftClickToMove:
        switchControlTypeToLeftClickToMove();
        break;
      case ControlType.RelativeWASD:
        switchControlTypeToRelativeWASD();
        break;
      case ControlType.RightClickToMove:
        switchControlTypeToRightClickToMove();
        break;
    }
  }

  public void switchControlTypeToRightClickToMove() {
    switchControlTypeTo(ControlType.RightClickToMove, rightClickToMoveButton.transform.position);
  }

  public void switchControlTypeToLeftClickToMove() {
    switchControlTypeTo(ControlType.LeftClickToMove, leftClickToMoveButton.transform.position);
  }

  public void switchControlTypeToAbsoluteWASD() {
    switchControlTypeTo(ControlType.AbsoluteWASD, absoluteWASDButton.transform.position);
  }

  public void switchControlTypeToRelativeWASD() {
    switchControlTypeTo(ControlType.RelativeWASD, relativeWASDButton.transform.position);
  }
  
  private void switchControlTypeTo(ControlType newControlType, Vector3 position) {
    //Logger.Log("CellControl::switchControlTypeTo("+newControlType+") with old="+_currentControlType, Logger.Level.DEBUG);
        Debug.LogError("before CellControl::switchControlTypeTo("+newControlType+") isLeftClickToMove="+isLeftClickToMove+" & isAbsoluteWASD="+isAbsoluteWASD);
        if(ControlType.LeftClickToMove == newControlType) {
            _isFirstUpdate = true;
        }

        switch(newControlType) {
            case ControlType.AbsoluteWASD:
                isAbsoluteWASD = true;
                selectedKeyboardControlTypeSprite.transform.position = absoluteWASDButton.transform.position;
                break;
            case ControlType.RelativeWASD:
                isAbsoluteWASD = false;
                selectedKeyboardControlTypeSprite.transform.position = relativeWASDButton.transform.position;
                break;
            case ControlType.LeftClickToMove:
                isLeftClickToMove = true;
                selectedMouseControlTypeSprite.transform.position = leftClickToMoveButton.transform.position;
                break;
            case ControlType.RightClickToMove:
                isLeftClickToMove = false;
                selectedMouseControlTypeSprite.transform.position = rightClickToMoveButton.transform.position;
                break;
            default:
                break;

        }

        _targetPosition = transform.position;

        Debug.LogError("after CellControl::switchControlTypeTo("+newControlType+") isLeftClickToMove="+isLeftClickToMove+" & isAbsoluteWASD="+isAbsoluteWASD);
  }

    public void refreshControlType ()
    {
        Debug.LogError("before refreshControlType isLeftClickToMove="+isLeftClickToMove+" & isAbsoluteWASD="+isAbsoluteWASD);
        if(isLeftClickToMove){
            switchControlTypeToLeftClickToMove ();
        } else {
            switchControlTypeToRightClickToMove ();
        }
        if(isAbsoluteWASD){
            switchControlTypeToAbsoluteWASD ();
        } else {
            switchControlTypeToRelativeWASD ();
        }
        Debug.LogError("after refreshControlType isLeftClickToMove="+isLeftClickToMove+" & isAbsoluteWASD="+isAbsoluteWASD);
    }

  void OnCollisionStay(Collision col) {
    if ((Vector3.zero != _inputMovement) && col.collider && (wallname == col.collider.name)){
      stopMovement();
    }
  }
}
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

  public CellControlButton absoluteWASDButton;
  public CellControlButton leftClickToMoveButton;
  public CellControlButton relativeWASDButton;
  public CellControlButton rightClickToMoveButton;
  public UISprite selectedControlTypeSprite;

  private bool _pause;
  private Vector3 _inputMovement;
  public Vector3 GetInputMovement() { return _inputMovement;}

  /* 
   * Click to move variables
   */
  private int _smooth; // Determines how quickly object moves towards position
  private float _hitdist = 0.0f;
  private Vector3 _targetPosition;
  private bool _isFirstUpdate = false;
    
  private enum ControlType {
      RightClickToMove = 0,
      LeftClickToMove = 1,
      AbsoluteWASDAndLeftClickToMove = 2,
      AbsoluteWASD = 3,
      RelativeWASD = 4
  };
  private ControlType _currentControlType = ControlType.AbsoluteWASDAndLeftClickToMove;

	public enum RockCollisionType {
		Slide = 2,
		Grab = 1,
		Normal = 0
	}
	//Drag PushableBox
	private RockCollisionType _currentCollisionType = 0;
	public RockCollisionType getCurrentCollisionType() {return _currentCollisionType;}

	private float _angle = 0f;		// the angle for the rotation around the pushable box
	private float _angleProgress = 0f;	// the current progression of the rotation
	private float _rotationSpeed = 2f;
	private float _angleStep = 0f;		// depends on the rotationSpeed and the position of the click

	private float _minAngle = 17f;
	private float _maxAngle = 50f;

	private bool _isDragging = false;
	public bool getIsDragging() {return _isDragging;}
	public void setIsDragging(bool b) {_isDragging = b;}

	GameObject box;
	public void setBox(GameObject o) {box = o;}
	public GameObject getBox(){return box;}




  public void Pause(bool pause)
  {
    _pause = pause;
  }

  public bool isPaused()
  {
    return _pause;
  }

  private void clickToMoveUpdate(KeyCode mouseButtonCode) {
    Vector3 lastTickPosition = transform.position;
    if(Input.GetKeyDown(mouseButtonCode))            
    {
		    _smooth = 1;
			    
			Plane playerPlane = new Plane(Vector3.up, transform.position);            
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);            
			      
			if (playerPlane.Raycast (ray, out _hitdist) && !UICamera.hoveredObject) {                
				 _targetPosition = ray.GetPoint(_hitdist);
		}

			if(_isDragging)

			{	//Cancel the drag
				box.GetComponent<PushableBox>().setUsedClicked(true);
				box.GetComponent<PushableBox>().clicked();

				if(_isDragging)
				{//Determine the angle for the move
					box.GetComponent<PushableBox>().setDestination(_targetPosition);
					_angle = Vector3.Angle(transform.forward,_targetPosition-box.transform.position);

					Vector3 relativepoint = transform.InverseTransformPoint(_targetPosition);

					//if clicking behind cell : leave the grab
					if(relativepoint.z < 0.0f)
						box.GetComponent<PushableBox>().updateDragStatusOnClick();

					if(_angle < _minAngle || _angle >= _maxAngle)
						_angle = 0f;
					else if(_angle >=_minAngle && _angle < _maxAngle)
					{ 
						// if the point is on the left or the right of the player

						_angleProgress = 0f;

						if (relativepoint.x < 0.0f)
						{
							_angle = -_angle;
							_angleStep = -_rotationSpeed;
						}
						else 
							_angleStep = _rotationSpeed;

						int i = 0 ;
						bool isFound = false;



						//inverse of _targetPosition in world coord
						/*Vector3 inverseTargetPosition = box.transform.TransformPoint(box.transform.InverseTransformPoint(_targetPosition)*(-1f));
						inverseTargetPosition = new Vector3(inverseTargetPosition.x,_targetPosition.y,inverseTargetPosition.z);*/


						//Change the position of the anchor 

						//Vector3 closestPoint = box.rigidbody.ClosestPointOnBounds(inverseTargetPosition);
						//transform.FindChild("springStart").GetComponent<SpringJoint>().anchor = box.transform.InverseTransformPoint(closestPoint);

					}
				}
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
	  if(_isDragging)
	    draggingUpdate();
      rotationUpdate();
    }
  }
    

  private void absoluteWASDUpdate() {
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

  private void relativeWASDUpdate() {

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

  private void stopMovement() {        
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

	private void draggingMove(float angle) {
		if(_isDragging)
		{
			transform.RotateAround(box.transform.position,new Vector3(0,1,0),angle);
		}
	}



	//the update during the Drag mode with a PushableBox
	private void draggingUpdate() {

		if (Mathf.Abs(_angle) > _minAngle && Mathf.Abs(_angle)< _maxAngle)

		{
			if(Mathf.Abs(_angleProgress) < Mathf.Abs (0.8f*_angle))
					_angleProgress += _angleStep;

			else _angleProgress += 0.5f*_angleStep;

			if(Mathf.Abs(_angleProgress) >= Mathf.Abs (_angle))
			{

				_angle = 0.0f;
				_angleProgress =0.0f;

				_angleStep = 0.0f;
			}
				
			else 
			{

				draggingMove(_angleStep);

			}
		}
		else
		{

			_angle = 0f;
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

   /* absoluteWASDButton = GameObject.Find("AbsoluteWASDButton").GetComponent<AbsoluteWASDButton>();
    leftClickToMoveButton = GameObject.Find("LeftClickToMoveButton").GetComponent<LeftClickToMoveButton>();
    relativeWASDButton = GameObject.Find("RelativeWASDButton").GetComponent<RelativeWASDButton>();
    rightClickToMoveButton = GameObject.Find("RightClickToMoveButton").GetComponent<RightClickToMoveButton>();
    selectedControlTypeSprite = GameObject.Find ("SelectedControlTypeSprite").GetComponent<UISprite>();*/

//    absoluteWASDButton.cellControl = this;
//    leftClickToMoveButton.cellControl = this;
//    relativeWASDButton.cellControl = this;
//    rightClickToMoveButton.cellControl = this;

    switchControlTypeToAbsoluteWASDAndLeftClickToMove();
	}
  
	void Update(){
		//Keyboard controls
		if(!_pause) {

      _inputMovement = Vector3.zero;

      switch(_currentControlType) {
        case ControlType.AbsoluteWASDAndLeftClickToMove:  
            absoluteWASDUpdate();
            if(!_isFirstUpdate) {
                clickToMoveUpdate(KeyCode.Mouse0);
            } else { _isFirstUpdate = false; }
            break;
        case ControlType.LeftClickToMove:      
          if(!_isFirstUpdate) {
              clickToMoveUpdate(KeyCode.Mouse0);
          } else { _isFirstUpdate = false; }
          break;
        case ControlType.RightClickToMove:
          clickToMoveUpdate(KeyCode.Mouse1);
          break;
        case ControlType.AbsoluteWASD:
          absoluteWASDUpdate();
          break;
        case ControlType.RelativeWASD:
          relativeWASDUpdate();
          break;
        default:
          absoluteWASDUpdate();
          break;
      }
	  if( !_isDragging)
        commonUpdate();
	  else if(_isDragging && _angle ==0f)
			{
				commonUpdate();
			}
    }
    
		switchRockCollision();
    //if(Input.GetKeyDown(KeyCode.Space)) {
    //  switchControlTypeTo((ControlType)(((int)_currentControlType + 1) % 5));
    //}


    
  }

	//Allow the user to choose the way to interact with the pushableBox
	private void switchRockCollision () {

		if(Input.GetKeyDown(KeyCode.Space))
		   {
			if (_currentCollisionType == RockCollisionType.Slide)
				_currentCollisionType -=2;
			else
				_currentCollisionType +=1;
			Logger.Log ("collisionType::"+_currentCollisionType,Logger.Level.WARN);

			}
		}

  private void switchControlTypeTo(ControlType newControlType) {
    switch(newControlType) {
      case ControlType.AbsoluteWASDAndLeftClickToMove:
        switchControlTypeToAbsoluteWASDAndLeftClickToMove();
        break;
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
    
  public void switchControlTypeToAbsoluteWASDAndLeftClickToMove() {
    switchControlTypeTo(ControlType.AbsoluteWASDAndLeftClickToMove, absoluteWASDButton.transform.position);
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
    Logger.Log("CellControl::switchControlTypeTo("+newControlType+") with old="+_currentControlType, Logger.Level.DEBUG);

    if(ControlType.LeftClickToMove == newControlType) {
      _isFirstUpdate = true;
    }

    selectedControlTypeSprite.transform.position = position;
    _targetPosition = transform.position;
    _currentControlType = newControlType;
  }

  void OnCollisionStay(Collision col) {
    if ((Vector3.zero != _inputMovement) && col.collider && (wallname == col.collider.name)){
      stopMovement();
    }
  }
}
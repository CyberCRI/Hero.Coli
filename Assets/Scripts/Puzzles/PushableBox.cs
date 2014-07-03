using UnityEngine;
using System.Collections;

[System.Serializable]
public class PushableBox : MonoBehaviour {
	
	public float minSpeed;
	private Vector3 _initPos;
  private CellControl _control = null;
  private RigidbodyConstraints noPush = RigidbodyConstraints.FreezeAll;
  private RigidbodyConstraints canPush = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

	private bool _dragged = false;
	private bool _willBeDragged = false;

	private Color32 _normalColor;
	private Color32 _nearColor;

	private bool _playerIsNear = false;
	private bool _usedClicked = false;			// if Clicked method has already been used from outside the GameObject during the frame 
	public void setUsedClicked(bool b) {_usedClicked = b;}
	
	void Start(){
		_initPos = transform.position;
		_nearColor = new Color32 (51,233,72,255);
		_normalColor = new Color32(140,180,187,190);
	}

  private CellControl lazySafeGetCellControl(GameObject col)
  {
    if(null == _control)
    {
      _control = col.GetComponent<CellControl>();
    }
    return _control;
  }

  void OnCollisionEnter(Collision col) {
		lazySafeGetCellControl(col.gameObject);

		if(col.gameObject == _control.gameObject 
           && _willBeDragged 
           && _control.getCurrentCollisionType() == CellControl.RockCollisionType.Grab)
		{
		//	CreateHingeJoint(col);
			//CreateFixedJoint(col);
			createSpringJoint(col);
			updateDragStatusOnContact();
		}
    if (col.collider){
      lazySafeGetCellControl(col.gameObject);
      if(_control && _control.currentMoveSpeed >= minSpeed){
        rigidbody.constraints = canPush;
      }
    }
  }
  
  void OnCollisionExit(Collision col) {
    if (col.collider){
      lazySafeGetCellControl(col.gameObject);
      if(_control)
			{
				if(_control.getCurrentCollisionType() == CellControl.RockCollisionType.Slide)
				  rigidbody.AddForce(_control.GetInputMovement()*(0.25f+(_control.currentMoveSpeed/minSpeed)),ForceMode.Acceleration);
				else
       			  rigidbody.constraints = noPush;
			}
    }
  }

  void OnCollisionStay(Collision col)
  {
	
    if(rigidbody.constraints != canPush)
    {
      if (col.collider){
        lazySafeGetCellControl(col.gameObject);
        if(_control && _control.currentMoveSpeed >= minSpeed){
          rigidbody.constraints = canPush;
        }
      }
    }
  }

	void createHingeJoint(Collision col)
	{
		if(_willBeDragged)
		{
			gameObject.AddComponent<HingeJoint>();
			hingeJoint.connectedBody = _control.rigidbody;
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.autoConfigureConnectedAnchor = false;
			hingeJoint.connectedAnchor = new Vector3(0,0,1);
			hingeJoint.breakForce = Mathf.Infinity;
			//hingeJoint.axis= new Vector3(0,1,0);
			JointLimits limit = hingeJoint.limits;
			limit.min = -30;
			limit.max = 30;

			hingeJoint.limits = limit;
			hingeJoint.useLimits = true;
		}
	}

	void createSpringJoint(Collision col)
	{
		if(_willBeDragged)
		{
			SpringJoint buffjoint = gameObject.AddComponent<SpringJoint>() as SpringJoint;
			buffjoint.connectedBody = _control.rigidbody;
			buffjoint.minDistance = transform.lossyScale.magnitude;

      //TODO create variables for these numbers
			buffjoint.maxDistance = buffjoint.minDistance*3f;
			buffjoint.breakForce = _control.currentMoveSpeed*1.1f;

			buffjoint.anchor = transform.InverseTransformPoint(col.contacts[0].point);
			buffjoint.autoConfigureConnectedAnchor=false;
			buffjoint.connectedAnchor = _control.transform.InverseTransformPoint(col.contacts[0].point);

			Logger.Log ("anchor ::"+buffjoint.anchor,Logger.Level.WARN);
			//buffjoint.anchor = Vector3.zero;
			_control.setIsDragging(true);
			_control.setBox(gameObject);
		}
	}

	void createFixedJoint(Collision col)
	{
		if(_willBeDragged)
		{
			FixedJoint buffjoint = gameObject.AddComponent<FixedJoint>() as FixedJoint;
			//FixedJoint buffjoint = new FixedJoint();
			buffjoint.connectedBody = _control.rigidbody;
			buffjoint.breakForce = Mathf.Infinity;

		}
	}
	
	public void resetPos(){
		transform.position = _initPos;
	}

	private void updateDragStatusOnContact() {
  	if(_willBeDragged){
      _willBeDragged = !_willBeDragged;
      _dragged = true;
    }
	}

	private void updateDragStatusOnClick() {
		if(_willBeDragged) _willBeDragged = false;
		else if (!_willBeDragged && !_dragged) _willBeDragged = true;
		else if (!_willBeDragged && _dragged) {
			_dragged = false;
			Destroy(GetComponent<SpringJoint>() as SpringJoint);
			_control.setIsDragging(false);
			_control.setBox(null);
		}
	}
	

	public void clicked() {
		// if left-clicked 
		if(Input.GetMouseButtonDown(0) && renderer.isVisible && _playerIsNear)
		{
			RaycastHit[] hits;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			hits = Physics.RaycastAll(ray);
								
			foreach(RaycastHit hit in hits)
			{
				//if the pushableBox have been clicked
				if(hit.transform.GetComponent<PushableBox>())
				{
          updateDragStatusOnClick();
          break;
				}
			}
		}
	}

	void Update()
	{

		if(renderer.isVisible && (null == _control))
			lazySafeGetCellControl(GameObject.Find("Perso"));
		if(_control && _control.getCurrentCollisionType() == CellControl.RockCollisionType.Grab)
		{
			detectProximity();
			if(!_usedClicked)
		    	clicked();
			else 
				_usedClicked = false;
			  switchParticleColor();
		}
	}

	void switchParticleColor()
	{
		if (_playerIsNear && particleSystem.startColor != _nearColor)
		{
			particleSystem.startColor = _nearColor;
			particleSystem.startSize = 4.5f;
		}
		else if (!_playerIsNear && particleSystem.startColor != _normalColor)
		{

			particleSystem.startColor = _normalColor;
			particleSystem.startSize = 3f;
		}
	}

	void detectProximity ()
	{
		if(renderer.isVisible)
		{
			Vector3 radius = transform.localScale;
			Collider[] hitsColliders = Physics.OverlapSphere(transform.position, radius.sqrMagnitude*40);
			
			// If Player is in a small area
			if (System.Array.Find(hitsColliders, col => col == _control.collider))
			{
				if(!_playerIsNear)
				{
				_playerIsNear = true;
				Logger.Log ("Near", Logger.Level.WARN);
				}
			}
			else if (_playerIsNear)
			{
				_playerIsNear = false;
				Logger.Log ("NotNear", Logger.Level.WARN);
			}
		}

	}

}

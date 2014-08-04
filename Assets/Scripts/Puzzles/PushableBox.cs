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
	private Vector3 _destination;
	public void setDestination(Vector3 v) {_destination = new Vector3 (v.x,v.y,v.z);}

	private Color32 _normalColor;
	private Color32 _nearColor;
	private Color32 _draggingColor;

	private bool _playerIsNear = false;
	private bool _usedClicked = false;			// if Clicked method has already been used from outside the GameObject during the frame 
	public void setUsedClicked(bool b) {_usedClicked = b;}
	
	void Start(){
		_initPos = transform.position;
		_nearColor = new Color32 (51,233,72,255);
		_normalColor = new Color32(140,180,187,190);
		_draggingColor = new Color32(255,178,41,147);
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
			Logger.Log ("col.gamObject::"+col.gameObject,Logger.Level.WARN);
		//	CreateHingeJoint(col);
			//CreateFixedJoint(col);

			//CreateSpringJoint(col);
			if(!_control.getBox()) {

			CreateDragLink(col);
			}

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

	// NOT USED
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
			JointLimits limit = hingeJoint.limits;
			limit.min = -30;
			limit.max = 30;

			hingeJoint.limits = limit;
			hingeJoint.useLimits = true;
		}
	
	}
	


	//USED
	void createSpringJoint (Collision col, GameObject springStart, GameObject springEnd)
	{

			Vector3 bAnchor = springStart.transform.InverseTransformPoint(col.contacts[0].point);
			Vector3 bConnectedAnchor = springEnd.transform.InverseTransformPoint(col.contacts[0].point);

			
			SpringJoint buffjoint = springStart.AddComponent<SpringJoint>() as SpringJoint;
			buffjoint.connectedBody = springEnd.rigidbody;
			buffjoint.minDistance = 0f;
			buffjoint.maxDistance =  transform.lossyScale.magnitude;
			
			buffjoint.breakForce = _control.currentMoveSpeed*1.1f;
			buffjoint.anchor = new Vector3(bAnchor.x,0f,bAnchor.z);
			buffjoint.autoConfigureConnectedAnchor=false;
			buffjoint.connectedAnchor = new Vector3(bConnectedAnchor.x,0f,bConnectedAnchor.z);
			_control.setIsDragging(true);
			_control.setBox(gameObject);


	}




	void createFixedJoint(Collision col)

	{
		if(_willBeDragged)
		{
			FixedJoint buffjoint = gameObject.AddComponent<FixedJoint>() as FixedJoint;
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

	public void updateDragStatusOnClick() {
		if(_willBeDragged) _willBeDragged = false;
		else if (!_willBeDragged && !_dragged) _willBeDragged = true;
		else if (!_willBeDragged && _dragged) {
			_dragged = false;


			DestroyDragLink(_control.gameObject);
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

	/*	Debug Line showing the anchor and the connected anchor
	 * 
	 	if (_control &&_control.GetIsDragging())
		{
			Debug.DrawLine(transform.TransformPoint(_control.transform.FindChild("springStart").GetComponent<SpringJoint>().anchor),transform.TransformPoint(_control.transform.FindChild("springStart").GetComponent<SpringJoint>().connectedAnchor),Color.red);
			Debug.DrawLine (transform.TransformPoint(_control.transform.FindChild("springStart").GetComponent<SpringJoint>().anchor),_control.transform.position,Color.blue);
			Debug.DrawLine (transform.TransformPoint(_control.transform.FindChild("springStart").GetComponent<SpringJoint>().connectedAnchor),_control.transform.position,Color.green);
		}*/
	}

	void switchParticleColor()
	{
		if(_control.getIsDragging() && _control.getBox() == gameObject 
		   && particleSystem.startColor != _draggingColor)
		{
			particleSystem.startColor = _draggingColor;
		}
		else if (!_control.getIsDragging() && _playerIsNear && particleSystem.startColor != _nearColor)
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

				}
			}
			else if (_playerIsNear)
			{
				_playerIsNear = false;

			}
		}

	}

	public void pushRock(Vector3 ammount)
	{

		rigidbody.AddForceAtPosition(ammount,gameObject.GetComponent<SpringJoint>().anchor,ForceMode.Acceleration);
		rigidbody.useGravity = true;

	}

	public void CreateDragLink(Collision col)
	{
		GameObject springStart = new GameObject();
		GameObject springEnd = new GameObject();

		springStart.name = "springStart";
		springStart.AddComponent<Rigidbody>();

		springEnd.name = "springEnd";
		springEnd.AddComponent<Rigidbody>();




		//add new objet to parent with hingejoint
		springStart.transform.parent = col.contacts[0].otherCollider.transform;
		springStart.transform.localPosition = Vector3.zero;
		springStart.transform.parent.gameObject.AddComponent<HingeJoint>();
		springStart.transform.parent.hingeJoint.connectedBody = springStart.rigidbody;

		springEnd.transform.parent = transform;
		springEnd.transform.localPosition = Vector3.zero;
		springEnd.transform.parent.gameObject.AddComponent<HingeJoint>();
		springEnd.transform.parent.hingeJoint.connectedBody = springEnd.rigidbody;

		//create springjoint
		createSpringJoint(col, springStart, springEnd);

	}

	public void DestroyDragLink(GameObject g)
	{
		GameObject springStart = g.transform.FindChild("springStart").gameObject;

		GameObject springEnd = springStart.GetComponent<SpringJoint>().connectedBody.gameObject;

		//destroy hingejoint
		Destroy (springStart.transform.parent.hingeJoint);
		Destroy (springEnd.transform.parent.hingeJoint);

		//destroy springjoint
		Destroy (springStart.GetComponent<SpringJoint>());

		//destroy object
		Destroy (springStart);
		Destroy (springEnd);

	}

}

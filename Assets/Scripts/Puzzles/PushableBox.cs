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

		if(col.gameObject == _control.gameObject && _willBeDragged && _control.GetCurrentCollisionType() == CellControl.RockCollisionType.Grab)
		{
			CreateHingeJoint(col);
			UpdateDragStatusOnContact();
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
				if(_control.GetCurrentCollisionType() == CellControl.RockCollisionType.Slide)
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

	void CreateHingeJoint(Collision col)
	{
		if(_willBeDragged)
		{
			gameObject.AddComponent<HingeJoint>();
			hingeJoint.connectedBody = _control.rigidbody;
		//	hingeJoint.anchor = col.contacts[0].point;
			hingeJoint.breakForce = Mathf.Infinity;
			//hingeJoint.axis= new Vector3(0,1,0);
		}
	}
	
	public void resetPos(){
		transform.position = _initPos;
	}

	private void UpdateDragStatusOnContact() {

		if(_willBeDragged){_willBeDragged = !_willBeDragged; _dragged = true;}




	}

	private void UpdateDragStatusOnClick() {
		if(_willBeDragged) _willBeDragged = false;
		else if (!_willBeDragged && !_dragged) _willBeDragged = true;
		else if (!_willBeDragged && _dragged) {
			_dragged = false;
			Destroy(hingeJoint);
		}
	}
	

	void Clicked() {

		// if left-clicked 
		if(Input.GetMouseButtonDown(0) && renderer.isVisible)
		{
			Vector3 radius = transform.localScale;
			Collider[] hitsColliders = Physics.OverlapSphere(transform.position,radius.sqrMagnitude*40);

			// If Player is in a small area
			if (System.Array.Find(hitsColliders,col => col == _control.collider))
			    {
					int i = 0 ;
					bool isFound = false;
					_playerIsNear = true;

					RaycastHit[] hits;
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					hits = Physics.RaycastAll(ray);
					
					
					while (i < hits.Length && !isFound)
					{
						//if the pushableBox have been clicked
						if(hits[i].transform.GetComponent<PushableBox>())
						{
						UpdateDragStatusOnClick();
							isFound = true;
						}
						i++;
					}
				}
			else
			{
				_playerIsNear = false;
			}



		}
	}

	void Update()
	{

		if(renderer.isVisible && !_control)
			lazySafeGetCellControl(GameObject.Find("Perso"));
		if(_control && _control.GetCurrentCollisionType() == CellControl.RockCollisionType.Grab)
		{

		  Clicked();
			SwitchParticlesColor();
		}
	}

	void SwitchParticlesColor()
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

}

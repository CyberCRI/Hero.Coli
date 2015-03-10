using UnityEngine;
using System.Collections;

[System.Serializable]
public class PushableBox : MonoBehaviour {
	
	public float minSpeed;
	private Vector3 _initPos;
  private CellControl _control = null;
  private RigidbodyConstraints noPush = RigidbodyConstraints.FreezeAll;
  private RigidbodyConstraints canPush = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
	
	void Start(){
		_initPos = transform.position;
	}

  private CellControl lazySafeGetCellControl(Collision col)
  {
    if(null == _control)
    {
      _control = col.gameObject.GetComponent<CellControl>();
    }
    return _control;
  }

  void OnCollisionEnter(Collision col) {
    if (col.collider){
      lazySafeGetCellControl(col);
      if(_control && _control.currentMoveSpeed >= minSpeed){
        GetComponent<Rigidbody>().constraints = canPush;
      }
    }
  }
  
  void OnCollisionExit(Collision col) {
    if (col.collider){
      lazySafeGetCellControl(col);
      if(_control)
        GetComponent<Rigidbody>().constraints = noPush;
    }
  }

  void OnCollisionStay(Collision col)
  {
    if(GetComponent<Rigidbody>().constraints != canPush)
    {
      if (col.collider){
        lazySafeGetCellControl(col);
        if(_control && _control.currentMoveSpeed >= minSpeed){
          GetComponent<Rigidbody>().constraints = canPush;
        }
      }
    }
  }
	
	public void resetPos(){
		transform.position = _initPos;
	}
	
}

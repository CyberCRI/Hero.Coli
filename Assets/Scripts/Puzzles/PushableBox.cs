using UnityEngine;
using System.Collections;

[System.Serializable]
public class PushableBox : MonoBehaviour {
	
	public float minSpeed;
	private Vector3 _initPos;
	
	void Start(){
		_initPos = transform.position;
	}
	
    void OnCollisionEnter(Collision col) {
		if (col.collider){
			CellControl cc = col.gameObject.GetComponent<CellControl>();
			if(cc && cc.currentMoveSpeed >= minSpeed){
				rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
			}
		}
	}
	
	void OnCollisionExit(Collision col) {
		if (col.collider){
			CellControl cc = col.gameObject.GetComponent<CellControl>();
			if(cc)
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		
	}
	
	public void resetPos(){
		Debug.Log("WTF");
		transform.position = _initPos;
	}
	
}

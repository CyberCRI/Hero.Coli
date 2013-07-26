using UnityEngine;
using System.Collections;

[System.Serializable]
public class PushableBox : MonoBehaviour {
	
	public float minSpeed = 20;
	
    void OnCollisionEnter(Collision col) {
		if (col.collider){
			CellControl cc = col.gameObject.GetComponent<CellControl>();
			if(cc && cc.moveSpeed >= minSpeed){
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
	
}

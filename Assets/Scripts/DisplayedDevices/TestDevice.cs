using UnityEngine;
using System.Collections;

public class TestDevice : MonoBehaviour {
	
	public int _deviceID;
	
	void OnPress() {
		Debug.Log("on press "+_deviceID);
	}
	void OnMouseUpAsButton() {		
		Debug.Log("mouse up as button "+_deviceID);
	}
	
	void OnMouseUp() {		
		Debug.Log("mouse up "+_deviceID);
	}
	
	void OnMouseDown() {		
		Debug.Log("mouse down "+_deviceID);
	}
	
	void OnMouseOver() {		
		Debug.Log("mouse over "+_deviceID);
	}
	
	void OnMouseDrag() {		
		Debug.Log("mouse drag "+_deviceID);
	}
	
	void OnMouseEnter() {		
		Debug.Log("mouse enter "+_deviceID);
	}
	
	void OnMouseExit() {		
		Debug.Log("mouse exit "+_deviceID);
	}
}

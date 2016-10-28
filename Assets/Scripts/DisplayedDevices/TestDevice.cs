using UnityEngine;

public class TestDevice : MonoBehaviour {
	
	public int _deviceID;
	
	void OnPress() {
		Debug.Log(this.GetType() + " on press "+_deviceID);
	}
	void OnMouseUpAsButton() {		
		Debug.Log(this.GetType() + " mouse up as button "+_deviceID);
	}
	
	void OnMouseUp() {		
		Debug.Log(this.GetType() + " mouse up "+_deviceID);
	}
	
	void OnMouseDown() {		
		Debug.Log(this.GetType() + " mouse down "+_deviceID);
	}
	
	void OnMouseOver() {		
		Debug.Log(this.GetType() + " mouse over "+_deviceID);
	}
	
	void OnMouseDrag() {		
		Debug.Log(this.GetType() + " mouse drag "+_deviceID);
	}
	
	void OnMouseEnter() {		
		Debug.Log(this.GetType() + " mouse enter "+_deviceID);
	}
	
	void OnMouseExit() {		
		Debug.Log(this.GetType() + " mouse exit "+_deviceID);
	}
}

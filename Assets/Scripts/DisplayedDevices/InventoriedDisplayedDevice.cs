using UnityEngine;

public class InventoriedDisplayedDevice : DisplayedDevice {
	
	void OnEnable() {
		Debug.Log(this.GetType() + " OnEnable "+_device);
	}
	
	public override void OnPress(bool isPressed) {
		if(isPressed) {
			Debug.Log(this.GetType() + " OnPress() "+getDebugInfos());
            toggleEquiped();
		}
	}

}
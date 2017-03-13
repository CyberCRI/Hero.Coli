using UnityEngine;

public class SettingsMainMenuItemArray : MainMenuItemArray
{
	void setAndroidDisplay()
	{
		var controlsButton = GetItemOfType<ControlsMainMenuItem>();
		float relativeOffsetY = controlsButton.GetComponent<UIAnchor>().relativeOffset.y;
		controlsButton.gameObject.SetActive (false);
		var backButton = GetItemOfType<BackMainMenuItem> ();
		backButton.GetComponent<UIAnchor> ().relativeOffset.y = relativeOffsetY;
	}

	void Start()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			setAndroidDisplay();
		#endif
	}
}

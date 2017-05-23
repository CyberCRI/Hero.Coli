public class SettingsMainMenuItemArray : MainMenuItemArray
{
	// graphics: all but webgl & android
	// controls: all but android

	void setAndroidDisplay()
	{
		GraphicsOptionsMainMenuItem graphicsButton = getItemOfType<GraphicsOptionsMainMenuItem>();
		SoundOptionMainMenuItem soundButton = getItemOfType<SoundOptionMainMenuItem>();
		ControlsMainMenuItem controlsButton = getItemOfType<ControlsMainMenuItem>();
		BackMainMenuItem backButton = getItemOfType<BackMainMenuItem> ();

		float graphicsRelativeOffsetY = graphicsButton.GetComponent<UIAnchor>().relativeOffset.y;
		float soundRelativeOffsetY = graphicsButton.GetComponent<UIAnchor>().relativeOffset.y;
		
		controlsButton.gameObject.SetActive (false);
		graphicsButton.gameObject.SetActive (false);

		soundButton.GetComponent<UIAnchor>().relativeOffset.y = graphicsRelativeOffsetY;
		backButton.GetComponent<UIAnchor> ().relativeOffset.y = soundRelativeOffsetY;

		this._items = new MainMenuItem[2];
		this._items[0] = soundButton;
		this._items[1] = backButton;
	}

	void setWebPlayerDisplay()
	{
		// save relativeOffset and hide
		GraphicsOptionsMainMenuItem graphicsButton = getItemOfType<GraphicsOptionsMainMenuItem>();
		SoundOptionMainMenuItem soundButton = getItemOfType<SoundOptionMainMenuItem>();
		ControlsMainMenuItem controlsButton = getItemOfType<ControlsMainMenuItem>();
		BackMainMenuItem backButton = getItemOfType<BackMainMenuItem> ();

		float graphicsRelativeOffsetY = graphicsButton.GetComponent<UIAnchor>().relativeOffset.y;
		float soundRelativeOffsetY = graphicsButton.GetComponent<UIAnchor>().relativeOffset.y;

		float relativeOffsetDelta = soundRelativeOffsetY - graphicsRelativeOffsetY;
		
		graphicsButton.gameObject.SetActive (false);

		soundButton.GetComponent<UIAnchor>().relativeOffset.y = graphicsRelativeOffsetY;
		controlsButton.GetComponent<UIAnchor>().relativeOffset.y = soundRelativeOffsetY;
		backButton.GetComponent<UIAnchor> ().relativeOffset.y = graphicsRelativeOffsetY + relativeOffsetDelta;

		this._items = new MainMenuItem[3];
		this._items[0] = soundButton;
		this._items[1] = controlsButton;
		this._items[2] = backButton;
	}

	void Start()
	{
#if UNITY_EDITOR

#elif UNITY_WEBPLAYER
		setWebPlayerDisplay(); 
#elif UNITY_ANDROID
		setAndroidDisplay();
#endif
	}
}

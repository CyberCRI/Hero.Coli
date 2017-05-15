public class SettingsMainMenuItemArray : MainMenuItemArray
{
	// graphics: all but webgl
	// controls: all but android

	void setAndroidDisplay()
	{
		GraphicsOptionsMainMenuItem graphicsButton = getItemOfType<GraphicsOptionsMainMenuItem>();
		SoundOptionMainMenuItem soundButton = getItemOfType<SoundOptionMainMenuItem>();
		ControlsMainMenuItem controlsButton = getItemOfType<ControlsMainMenuItem>();
		float relativeOffsetY = controlsButton.GetComponent<UIAnchor>().relativeOffset.y;
		controlsButton.gameObject.SetActive (false);

		BackMainMenuItem backButton = getItemOfType<BackMainMenuItem> ();
		backButton.GetComponent<UIAnchor> ().relativeOffset.y = relativeOffsetY;

		this._items = new MainMenuItem[3];
		this._items[0] = graphicsButton;
		this._items[1] = soundButton;
		this._items[2] = backButton;
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
#if UNITY_WEBPLAYER && !UNITY_EDITOR
		setWebPlayerDisplay(); 
#elif UNITY_ANDROID && !UNITY_EDITOR
		setAndroidDisplay();
#endif
	}
}

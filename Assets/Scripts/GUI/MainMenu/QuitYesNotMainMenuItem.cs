using UnityEngine;
using System.Collections;

public class QuitYesNotMainMenuItem : MainMenuItem {
	public enum Type {
		Yes,
		No,
	}

	[SerializeField]
	private Type _type;
	[SerializeField]
	private string _urlLink;

	public override void click ()
	{
		if (_type == Type.Yes)
			OnClickYes ();
		else
			OnClickNo ();
	}

	void OnClickYes()
	{
		base.click();
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.QUITYES.ToString()));
#if UNITY_WEBGL
		URLOpener.open(_urlLink,false);
#else
		Application.Quit();
#endif
	}

	void OnClickNo()
	{
		base.click();
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.QUITNO.ToString()));
		MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.DEFAULT);
	}
}

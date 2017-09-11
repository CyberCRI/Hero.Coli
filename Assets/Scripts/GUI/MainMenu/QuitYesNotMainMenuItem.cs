using UnityEngine;
using System.Collections;

public class QuitYesNotMainMenuItem : MainMenuItem {
	public enum Type {
		Yes,
		No,
	}

	[SerializeField]
	private Type _type;

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
		Application.Quit();
	}

	void OnClickNo()
	{
		base.click();
		MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.DEFAULT);
	}
}

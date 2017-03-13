using UnityEngine;
using System.Collections;

public class BackMainMenuItem : MainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.DEFAULT);
    }

	public override void initialize ()
	{
		base.initialize ();
	}
}

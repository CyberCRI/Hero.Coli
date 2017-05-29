using UnityEngine;
using System.Collections;

/// <summary>
/// Switches the main menu screen to the chapter selection screen
/// </summary>
public class ChapterSelectionMainMenuItem : MainMenuItem {
	/// <summary>
	/// Called the user clicks on this instance
	/// </summary>
	public override void click() {
		base.click ();
		MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.CHAPTERSELECTION);
	}

	void OnEnable()
	{
		#if HIDDEN_CHAPTERS
		gameObject.SetActive(false);
		#endif 
	}
}

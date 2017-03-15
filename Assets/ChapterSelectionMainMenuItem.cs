using UnityEngine;
using System.Collections;

public class ChapterSelectionMainMenuItem : MainMenuItem {
	public override void click() { 
		MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.CHAPTERSELECTION);
	}
}

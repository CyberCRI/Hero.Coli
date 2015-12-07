using UnityEngine;
using System.Collections;

public class MainMenuItemArray : MonoBehaviour {
    
    public MainMenuItem[] _items;

	public void triggerEnd() {
		foreach (MainMenuItem item in _items)
		{
			GoToOtherGameModeMainMenuItem castItem = item as GoToOtherGameModeMainMenuItem;
			if(castItem != null) {
				//castItem._isLocked = false;
				castItem.displayed = true;
			}
		}
	}
}

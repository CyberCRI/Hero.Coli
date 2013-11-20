using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoggerLabel : MonoBehaviour {
	
	public UILabel   label;
	
	// Update is called once per frame
	void Update () {
		List<string> messages = Logger.popAllMessages();
		messages.Sort();
		string toDisplay = "";
		foreach(string msg in messages) {
			toDisplay+=msg+"\n";
		}
		if(!string.IsNullOrEmpty(toDisplay)) {
			toDisplay.Remove(toDisplay.Length-1, 1);
		}	
		label.text = toDisplay;	
	}
}

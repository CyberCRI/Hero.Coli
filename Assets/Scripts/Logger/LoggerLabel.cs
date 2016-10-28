using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoggerLabel : MonoBehaviour {
	
	public UILabel   label;
    public GameObject loggerGUIComponentRoot;

    public void setActive (bool activate)
    {
        Debug.LogWarning(this.GetType() + " setActive("+activate+")");
        if(null != loggerGUIComponentRoot) {
            loggerGUIComponentRoot.SetActive(activate);
        }
        if(activate && !this.gameObject.activeSelf) {
            this.gameObject.SetActive(activate);
        }
    }

    public void treatMessages ()
    {
        List<string> messages = Logger.popAllMessages();
        //messages.Sort();
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

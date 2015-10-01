﻿using UnityEngine;
using System.Collections;

public class LearnMoreOptionsMainMenuItemArray : MainMenuItemArray {
    
	public GameObject learnMoreOptionsPanel;
	
	public static bool isHelp = false;
	private string urlKey = "LEARNMORE.LINK.MOOCFULL";

	private string getLocalizedUrl() {
		return Localization.Localize(urlKey);
	}
	public void goToMOOCSameTab() {
		goToMOOC (false);
	}
	public void goToMOOCNewTab() {
		goToMOOC (true);
	}

	private void goToMOOC (bool newTab) {	
		if (!newTab) {
			RedMetricsManager.get ().sendEvent (TrackingEvent.GOTOMOOC, new CustomData (CustomDataTag.SAMETAB, getLocalizedUrl()));
			Application.OpenURL (getLocalizedUrl());
		} else {
			RedMetricsManager.get ().sendEvent (TrackingEvent.GOTOMOOC, new CustomData (CustomDataTag.NEWTAB, getLocalizedUrl()));
			switch (Application.platform) {
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.OSXWebPlayer:
				Logger.Log ("LearnMoreOptionsMainMenuItemArray::goToMOOC Webplayer/WebGL attempting to open " + getLocalizedUrl(), Logger.Level.WEBPLAYER);
				Application.ExternalEval ("window.open('" + getLocalizedUrl() + "','_blank')");
				break;
			case RuntimePlatform.WindowsPlayer: 
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
				Debug.LogError ("LearnMoreOptionsMainMenuItemArray::goToMOOC Editor/Standalone attempting to open " + getLocalizedUrl());
				Application.OpenURL (getLocalizedUrl());
				break;
			default:
				Debug.LogError ("LearnMoreOptionsMainMenuItemArray::goToMOOC default attempting to open " + getLocalizedUrl());
				Application.OpenURL (getLocalizedUrl());
				break;
			}
		}
	}
    
    void OnEnable ()
    {
        learnMoreOptionsPanel.SetActive(true);
    }
    
    void OnDisable ()
    {
        learnMoreOptionsPanel.SetActive(false);
    }
}

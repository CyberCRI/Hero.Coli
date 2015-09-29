using UnityEngine;
using System.Collections;

public class LearnMoreOptionsMainMenuItemArray : MainMenuItemArray {
    
	public GameObject learnMoreOptionsPanel;
	
	public static bool isHelp = false;
	public string url = "http://genius.com/Igem-paris-bettencourt-team-what-are-biobricks-annotated";
	
	public void goToMOOCSameTab() {
		goToMOOC (false);
	}
	public void goToMOOCNewTab() {
		goToMOOC (true);
	}

	private void goToMOOC (bool newTab) {	
		if (!newTab) {
			RedMetricsManager.get ().sendEvent (TrackingEvent.GOTOMOOC, new CustomData (CustomDataTag.SAMETAB, url));
			Application.OpenURL (url);
		} else {
			RedMetricsManager.get ().sendEvent (TrackingEvent.GOTOMOOC, new CustomData (CustomDataTag.NEWTAB, url));
			switch (Application.platform) {
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.OSXWebPlayer:
					Logger.Log ("LearnMoreOptionsMainMenuItemArray::goToMOOC Webplayer/WebGL attempting to open " + url, Logger.Level.WEBPLAYER);
					Application.ExternalEval ("window.open('" + url + "','_blank')");
					break;
			case RuntimePlatform.WindowsPlayer: 
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
					Debug.LogError ("LearnMoreOptionsMainMenuItemArray::goToMOOC Editor/Standalone attempting to open " + url);
					Application.OpenURL (url);
					break;
			default:
					Debug.LogError ("LearnMoreOptionsMainMenuItemArray::goToMOOC default attempting to open " + url);
					Application.OpenURL (url);
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

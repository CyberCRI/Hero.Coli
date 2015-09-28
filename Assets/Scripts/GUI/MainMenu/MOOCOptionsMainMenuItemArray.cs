using UnityEngine;
using System.Collections;

public class MOOCOptionsMainMenuItemArray : MainMenuItemArray {
    
	public GameObject moocOptionsPanel;
	
	public static bool isHelp = false;
	public string url = "http://genius.com/Igem-paris-bettencourt-team-what-are-biobricks-annotated";
	
	public void goToMOOC () {	
		switch (Application.platform) {
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsWebPlayer:
		case RuntimePlatform.OSXWebPlayer:
				Debug.LogError ("MOOCOptionsMainMenuItemArray::goToMOOC Webplayer/WebGL attempting to open " + url);
				Application.ExternalEval("window.open('"+url+"','_blank')");
				break;
			case RuntimePlatform.WindowsPlayer: 
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
				Debug.LogError ("MOOCOptionsMainMenuItemArray::goToMOOC Editor/Standalone attempting to open " + url);
				Application.OpenURL (url);
				break;
			default:
				Debug.LogError ("MOOCOptionsMainMenuItemArray::goToMOOC default attempting to open " + url);
				Application.OpenURL (url);
				break;
		}
	}
    
    void OnEnable ()
    {
        moocOptionsPanel.SetActive(true);
    }
    
    void OnDisable ()
    {
        moocOptionsPanel.SetActive(false);
    }
}

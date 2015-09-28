using UnityEngine;
using System.Collections;

public class MOOCOptionsMainMenuItemArray : MainMenuItemArray {
    
    public GameObject moocOptionsPanel;
	
	public static bool isHelp = false;
	public string url = "http://genius.com/Igem-paris-bettencourt-team-what-are-biobricks-annotated";
	
	public void goToMOOC () {			
		Debug.LogError("MOOCOptionsMainMenuItemArray::goToMOOC attempting to open "+url);
		Application.OpenURL(url);
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

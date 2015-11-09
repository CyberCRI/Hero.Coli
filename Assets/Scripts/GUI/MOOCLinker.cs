using UnityEngine;
using System.Collections;

public class MOOCLinker : MonoBehaviour {

	public static bool isHelp = false;
	public string url = "http://genius.com/Igem-paris-bettencourt-team-what-are-biobricks-annotated";

	void OnClick() {
		if (isHelp) {			
			Debug.LogError("attempting to open "+url);
			Application.OpenURL(url);
		}
	}
}

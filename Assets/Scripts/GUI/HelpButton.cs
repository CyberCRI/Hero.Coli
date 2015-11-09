using UnityEngine;
using System.Collections;

public class HelpButton : MonoBehaviour {
	void OnPress (bool isPressed){
		MOOCLinker.isHelp = !MOOCLinker.isHelp;
		//TODO change cursor
		//TODO use Pause?
	}
}

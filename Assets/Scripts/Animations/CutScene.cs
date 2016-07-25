using UnityEngine;
using System.Collections;

public class CutScene : MonoBehaviour {

	// Use this for initialization
	void startCutScene () {
		blockClicks(true);
	}
	
	void endCutScene () {
		blockClicks(false);
	}
}

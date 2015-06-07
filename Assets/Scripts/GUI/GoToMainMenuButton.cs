using UnityEngine;
using System.Collections;

public class GoToMainMenuButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnPress(bool isPressed) {
        if(isPressed) {
            GameStateController.get ().goToMainMenu();
        }
    }
}

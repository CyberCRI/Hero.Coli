using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnPress (bool isPressed) {
    Debug.Log ("CraftFinalizationButton::OnPress("+isPressed+")");
    
  }
}

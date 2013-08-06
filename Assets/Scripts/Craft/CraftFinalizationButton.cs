using UnityEngine;
using System.Collections;

public class CraftFinalizationButton : MonoBehaviour {
  public CraftFinalizer _craftFinalizer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnPress (bool isPressed) {
    Debug.Log ("CraftFinalizationButton::OnPress("+isPressed+")");
    if(isPressed) _craftFinalizer.finalizeCraft();
  }
}

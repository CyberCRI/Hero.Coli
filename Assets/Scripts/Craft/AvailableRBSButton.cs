using UnityEngine;
using System.Collections;

public class AvailableRBSButton : MonoBehaviour {
  public AvailableBioBricksManager _availableBioBricksManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableRBSButton::OnPress()", Logger.Level.INFO);
      _availableBioBricksManager.displayRBS();
    }
  }
}

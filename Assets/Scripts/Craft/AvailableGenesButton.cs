using UnityEngine;
using System.Collections;

public class AvailableGenesButton : MonoBehaviour {
  public AvailableBioBricksManager _availableBioBricksManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableGenesButton::OnPress()", Logger.Level.INFO);
      _availableBioBricksManager.displayGenes();
    }
  }
}

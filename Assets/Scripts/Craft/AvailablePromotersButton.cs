using UnityEngine;
using System.Collections;

public class AvailablePromotersButton : MonoBehaviour {
  public AvailableBioBricksManager _availableBioBricksManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailablePromotersButton::OnPress()", Logger.Level.INFO);
      _availableBioBricksManager.displayPromoters();
    }
  }
}

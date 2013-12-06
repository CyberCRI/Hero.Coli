using UnityEngine;
using System.Collections;

public class BioBrickPanel : MonoBehaviour {
	void OnEnable()
  {
    AvailableBioBricksManager.get().OnPanelEnabled();
  }
}

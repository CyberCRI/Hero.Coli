using UnityEngine;
using System.Collections;

public class CraftScreenPanel : MonoBehaviour {
	void OnEnable () {
    //CraftZoneManager.get().setDevice(null);
    CraftZoneManager.get().OnBioBricksChanged();
	}
}

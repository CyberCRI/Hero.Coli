using UnityEngine;
using System.Collections;

public class ActiveRFPInfoPanel : MonoBehaviour {

	public GameObject infoPanel, hero;
	public GameStateController gameStateController;
	private bool alreadyDisplayed;
	// Use this for initialization
	void Start () {
		alreadyDisplayed = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		Logger.Log("ActiveRFPInfoPanel::OnTriggerEnter("+other.ToString()+") alreadyDisplayed="+alreadyDisplayed.ToString(), Logger.Level.TEMP);
		if(alreadyDisplayed == false) {
			if(other == hero.GetComponent<Collider>()) {
        Logger.Log("call to InfoWindowManager", Logger.Level.TEMP);
        InfoWindowManager.displayInfoWindow();
				alreadyDisplayed = true;
			}
		}
    }
		

}

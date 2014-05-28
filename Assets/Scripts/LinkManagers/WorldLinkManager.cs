using UnityEngine;
using System.Collections;


/*DESCRIPTION
 * This class create the links between the World's Scene, classes and GameObject and the others
 * */

public class WorldLinkManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

    GameObject perso = GameObject.Find ("Perso");

		//tuto End
		EndGameCollider endGameCollider = GameObject.Find("TutoEnd").GetComponent<EndGameCollider>();
    endGameCollider.hero = perso;
		endGameCollider.gameStateController = GameStateController.get ();
    endGameCollider.infoPanel = GameStateController.get().endPanel.gameObject;

    //tuto End
    InfoWindowCollisionTrigger tutoRFP = GameObject.Find("TutoRFP").GetComponent<InfoWindowCollisionTrigger>();
    tutoRFP.heroCollider = perso.GetComponent<CapsuleCollider>();

		Logger.Log ("EndGameCollider.infoPanel"+endGameCollider.infoPanel,Logger.Level.INFO);
	
	}

}

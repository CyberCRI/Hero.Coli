using UnityEngine;
using System.Collections;


/*DESCRIPTION
 * This class create the links between the World's Scene, classes and GameObject and the others
 * */

public class WorldLinkManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//tuto End
		EndGameCollider endGameCollider = GameObject.Find("TutoEnd").GetComponent<EndGameCollider>();
		endGameCollider.hero = GameObject.Find ("Perso");
		endGameCollider.gameStateController = GameStateController.get ();
		endGameCollider.infoPanel = GameStateController.get().endPanel.gameObject;

		Logger.Log ("EndGameCollider.infoPanel"+endGameCollider.infoPanel,Logger.Level.WARN);
	
	}

}

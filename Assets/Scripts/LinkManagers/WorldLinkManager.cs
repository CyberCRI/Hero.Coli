using UnityEngine;
using System.Collections;


/*DESCRIPTION
 * This class create the links between the World's Scene, classes and GameObject and the others
 * */

public class WorldLinkManager : MonoBehaviour
{

    public MineManager mineManager;
    public Transform startPosition;

    // Use this for initialization
    void Start ()
    {

        GameObject perso = GameObject.Find ("Perso");
        mineManager.hero = perso.GetComponent<Hero> ();

        if (null != startPosition) {
            perso.transform.position = startPosition.transform.position;
            perso.transform.rotation = startPosition.transform.rotation;
            startPosition.gameObject.SetActive(false);
        }

        //tuto End
        EndGameCollider endGameCollider = GameObject.Find ("TutoEnd").GetComponent<EndGameCollider> ();
        endGameCollider.hero = perso;
        endGameCollider.endInfoPanel = GameStateController.get ().endWindow;
        endGameCollider.endMainMenuButton = GameStateController.get ().endMainMenuButton;
            
        //tuto End
        InfoWindowCollisionTrigger tutoRFP = GameObject.Find ("TutoRFP").GetComponent<InfoWindowCollisionTrigger> ();
        tutoRFP.heroCollider = perso.GetComponent<CapsuleCollider> ();

        Logger.Log ("EndGameCollider.infoPanel" + endGameCollider.endInfoPanel, Logger.Level.INFO);
	
    }

}

using UnityEngine;


/*DESCRIPTION
 * This class create the links between the World's Scene, classes and GameObject and the others
 * */

public class WorldLinkManager : LinkManager
{

    public MineManager mineManager;
    public Transform startPosition;

    public override void initialize ()
    {

        GameObject perso = Hero.get().gameObject;
        if(null == perso) {
            Logger.Log("WorldLinkManager: error: Hero not found!", Logger.Level.ERROR);
        }

        if (null != startPosition) {
            CellControl.get().teleport(startPosition.transform.position, startPosition.transform.rotation);
            startPosition.gameObject.SetActive(false);
        }

        //specific code for adventure1
        GameObject endGameColliderGameObject = GameObject.Find ("TutoEnd");
        if(null != endGameColliderGameObject) { 
            EndGameCollider endGameCollider = endGameColliderGameObject.GetComponent<EndGameCollider> ();
            endGameCollider.hero = perso;
            endGameCollider.endInfoPanel = GameStateController.get ().endWindow;
            endGameCollider.endMainMenuButton = GameStateController.get ().endMainMenuButton;
            Logger.Log ("EndGameCollider.infoPanel" + endGameCollider.endInfoPanel, Logger.Level.INFO);
        }
            
        //specific code for adventure1
        GameObject tutoRFPGameObject = GameObject.Find ("TutoRFP");
        if(null != tutoRFPGameObject && null != perso) {
            InfoWindowCollisionTrigger tutoRFP = tutoRFPGameObject.GetComponent<InfoWindowCollisionTrigger> ();
            tutoRFP.heroCollider = perso.GetComponent<CapsuleCollider> ();
        }
	
        //specific code for tutorial1
        endGameColliderGameObject = GameObject.Find ("Tutorial1End");
        if(null != endGameColliderGameObject) { 
            EndGameCollider endGameCollider = endGameColliderGameObject.GetComponent<EndGameCollider> ();
            endGameCollider.hero = perso;
            endGameCollider.endInfoPanel = GameStateController.get ().endWindow;
            endGameCollider.endMainMenuButton = GameStateController.get ().endMainMenuButton;
            Logger.Log ("EndGameCollider.infoPanel" + endGameCollider.endInfoPanel, Logger.Level.INFO);
        }
    }

    public override void finishInitialize ()
    {    
    }
}

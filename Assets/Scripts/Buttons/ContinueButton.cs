using UnityEngine;
using System.Collections;

public class ContinueButton : ModalButton
{
  
    public GameObject parentPanel;
    public GameObject nextInfoPanel;
    public StartGameButton nextInfoPanelContinue;
    public GameStateController gameStateController;
  
    public override void press ()
    {
        Logger.Log ("ContinueButton::press()", Logger.Level.INFO);

        parentPanel = gameObject.transform.parent.gameObject;

        //TODO manage stack of modal elements in ModalManager
        //ModalManager.unsetModal(parentPanel);
        ModalManager.unsetModal ();
        gameStateController.tryUnlockPause ();

        ModalManager.setModal (nextInfoPanel, true, nextInfoPanelContinue.gameObject, nextInfoPanelContinue.GetType ().AssemblyQualifiedName);
    }
}

using UnityEngine;
using System.Collections;

public class ContinueButton : ModalButton
{
  
    public GameObject parentPanel;
    public GameObject nextInfoPanel;
    public StartGameButton nextInfoPanelContinue;
    public GameStateController gameStateController;
  
    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("ContinueButton::OnPress()", Logger.Level.INFO);

            parentPanel = gameObject.transform.parent.gameObject;

            //TODO manage stack of modal elements in ModalManager
            //ModalManager.unsetModal(parentPanel);
            ModalManager.unsetModal ();
            gameStateController.tryUnlockPause ();

            ModalManager.setModal (nextInfoPanel, true, nextInfoPanelContinue.gameObject, nextInfoPanelContinue.GetType ().Name);
        }
    }
}

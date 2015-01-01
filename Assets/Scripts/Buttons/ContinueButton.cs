using UnityEngine;
using System.Collections;

public class ContinueButton : ModalButton
{
  
    public GameObject nextInfoPanel;
    public GameObject nextInfoPanelContinue;
    public string nextInfoPanelContinueClass;

    public GameStateController gameStateController;
  
    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("ContinueButton::OnPress()", Logger.Level.INFO);
            ModalManager.setModal(nextInfoPanel, true, nextInfoPanelContinue, nextInfoPanelContinueClass);
            gameObject.transform.parent.gameObject.SetActive (false);
        }
    }
}

using UnityEngine;
using System.Collections;

public class ContinueButton : ModalButton
{
  
    public GameObject parentPanel;
    public GameObject nextInfoPanel;
    public StartGameButton nextInfoPanelContinue;
  
    public override void press ()
    {
        // Debug.Log(this.GetType() + " press()");

        parentPanel = gameObject.transform.parent.gameObject;

        //TODO manage stack of modal elements in ModalManager
        //ModalManager.unsetModal(parentPanel);
        ModalManager.unsetModal ();
        GameStateController.get ().tryUnlockPause ();

        ModalManager.setModal (nextInfoPanel, true, nextInfoPanelContinue.gameObject, nextInfoPanelContinue.GetType ().AssemblyQualifiedName);
    }
}

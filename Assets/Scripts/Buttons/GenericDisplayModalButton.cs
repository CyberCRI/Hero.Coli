using UnityEngine;
using System.Collections;

public class GenericDisplayModalButton : ModalButton
{
    public bool unsetModal;
    public string modalCode;

    protected override void OnPress (bool isPressed)
    {
        if (isPressed) {
            Logger.Log ("GenericDisplayModalButton::OnPress() with modalCode=" + modalCode, Logger.Level.INFO);
            
            //TODO manage stack of modal elements in ModalManager
            if (unsetModal) {
                ModalManager.unsetModal ();
            }
            ModalManager.setModal (modalCode);
        }
    }
}

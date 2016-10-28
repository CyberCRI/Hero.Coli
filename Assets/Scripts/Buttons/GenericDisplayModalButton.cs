using UnityEngine;
using System.Collections;

public class GenericDisplayModalButton : ModalButton
{
    public bool unsetModal;
    public string modalCode;

    public override void press ()
    {
        Logger.Log ("GenericDisplayModalButton::press() with modalCode=" + modalCode);
            
        //TODO manage stack of modal elements in ModalManager
        if (unsetModal) {
            ModalManager.unsetModal ();
        }
        ModalManager.setModal (modalCode);
    }
}

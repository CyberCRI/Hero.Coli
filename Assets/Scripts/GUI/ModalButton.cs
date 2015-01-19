using UnityEngine;
using System.Collections;

public abstract class ModalButton : MonoBehaviour
{
    protected virtual void OnPress (bool isPressed)
    {
        Logger.Log ("ModalButton::OnPress()", Logger.Level.INFO);
        if (isPressed) {
            press ();
        }
    }
    
    public abstract void press ();
}

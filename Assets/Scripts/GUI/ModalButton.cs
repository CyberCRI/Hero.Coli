using UnityEngine;
using System.Collections;

public abstract class ModalButton : MonoBehaviour
{
    protected virtual void OnPress (bool isPressed)
    {
        Debug.Log(this.GetType() + " ModalButton::OnPress()");
        if (isPressed) {
            press ();
        }
    }
    
    public abstract void press ();
}

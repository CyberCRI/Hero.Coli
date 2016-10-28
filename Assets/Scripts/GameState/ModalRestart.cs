using UnityEngine;
using System.Collections;

public class ModalRestart : ModalButton
{
    public override void press ()
    {
        Debug.Log(this.GetType() + " ModalRestart::press()");
        GameStateController.restart ();
    }
}

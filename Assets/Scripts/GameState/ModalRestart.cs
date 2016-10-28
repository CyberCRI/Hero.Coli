using UnityEngine;
using System.Collections;

public class ModalRestart : ModalButton
{
    public override void press ()
    {
        Debug.Log(this.GetType() + " press()");
        GameStateController.restart ();
    }
}

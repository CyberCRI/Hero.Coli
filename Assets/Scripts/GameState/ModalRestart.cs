using UnityEngine;
using System.Collections;

public class ModalRestart : ModalButton
{
    public override void press ()
    {
        Logger.Log ("ModalRestart::press()", Logger.Level.INFO);
        MemoryManager.get ().sendEvent(TrackingEvent.RESTART);
        GameStateController.restart ();
    }
}

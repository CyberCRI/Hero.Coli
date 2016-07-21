using UnityEngine;

//ModalButton inheritor that quits modal window when isPressed is true,
//contrary to CancelModal
public class GFPCraftHintModal : ModalButton
{
    public override void press ()
    {
        Logger.Log ("GFPCraftHintModal::press()", Logger.Level.INFO);
        GameStateController.get ().tryUnlockPause ();
        ModalManager.unsetModal ();
        GameObject.Find("Perso").AddComponent<GFPCraftHint>();
    }
}

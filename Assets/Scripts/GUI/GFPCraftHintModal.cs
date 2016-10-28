using UnityEngine;

//ModalButton inheritor that quits modal window when isPressed is true,
//contrary to CancelModal
public class GFPCraftHintModal : ModalButton
{
    public override void press ()
    {
        // Debug.Log(this.GetType() + " press()");
        GameStateController.get ().tryUnlockPause ();
        ModalManager.unsetModal ();
        Hero.get().gameObject.AddComponent<GFPCraftHint>();
    }
}

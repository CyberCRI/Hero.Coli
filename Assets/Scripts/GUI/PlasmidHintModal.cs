using UnityEngine;

//ModalButton inheritor that quits modal window when isPressed is true,
//contrary to CancelModal
public class PlasmidHintModal : ModalButton
{
    public override void press ()
    {
        // Debug.Log(this.GetType() + " press()");
        GameStateController.get ().tryUnlockPause ();
        ModalManager.unsetModal ();
        Character.get().gameObject.AddComponent<PlasmidHint>();
    }
}

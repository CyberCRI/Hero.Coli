using UnityEngine;

//ModalButton inheritor that quits modal window when isPressed is true,
//contrary to CancelModal
public class RBS1CraftHintModal : ModalButton
{
    public override void press ()
    {
        Debug.Log(this.GetType() + " RBS1CraftHintModal::press()");
        GameStateController.get ().tryUnlockPause ();
        ModalManager.unsetModal ();
        Hero.get().gameObject.AddComponent<RBS1CraftHint>();
    }
}

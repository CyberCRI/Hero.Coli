using UnityEngine;

//ModalButton inheritor that quits modal window when isPressed is true,
//contrary to CancelModal
public class RBS1CraftHintModal : ModalButton
{
    public override void press ()
    {
        Logger.Log ("RBS1CraftHintModal::press()", Logger.Level.INFO);
        GameStateController.get ().tryUnlockPause ();
        ModalManager.unsetModal ();
        Hero.get().gameObject.AddComponent<RBS1CraftHint>();
    }
}

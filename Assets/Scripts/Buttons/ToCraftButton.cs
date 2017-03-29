using UnityEngine;

public class ToCraftButton : ExternalOnPressButton
{
    public override void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            // TODO isOpenable should return why it's not openable
            // and the reason should be switch-case'd 
            if (CraftZoneManager.isOpenable())
            {
                // Debug.Log(this.GetType() + " OnPress()");
                GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
            }
            else
            {
                if(Character.isInjured)
                {
                    ModalManager.setModal("NoCraftWhileDamage");
                }
                else
                {
                    ModalManager.setModal("NoBricksYet");
                }
            }
        }
    }
}
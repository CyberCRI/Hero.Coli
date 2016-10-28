using UnityEngine;

public class ToCraftButton : ExternalOnPressButton
{
    public override void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            if (CraftZoneManager.isOpenable())
            {
                Debug.Log(this.GetType() + " ToCraftButton::OnPress()");
                GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
            }
            else
            {
                ModalManager.setModal("NoCraftWhileDamage");
            }
        }
    }
}
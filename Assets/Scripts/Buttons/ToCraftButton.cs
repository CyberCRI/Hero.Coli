public class ToCraftButton : ExternalOnPressButton
{
    public override void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            if (CraftZoneManager.isOpenable())
            {
                Logger.Log("ToCraftButton::OnPress()", Logger.Level.INFO);
                GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
            }
            else
            {
                ModalManager.setModal("NoCraftWhileDamage");
            }
        }
    }
}
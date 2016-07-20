//TODO make better name (cf ToEquipButton & ToWorldButton)
public class ToCraftButton : ExternalOnPressButton {

  public override void OnPress(bool isPressed)
  {
    if(isPressed && CraftZoneManager.isOpenable()) {
      Logger.Log("ToCraftButton::OnPress()", Logger.Level.INFO);
      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
    }
  }
}
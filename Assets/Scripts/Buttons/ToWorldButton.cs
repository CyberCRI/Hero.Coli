//TODO make better name (cf ToCraftButton & ToEquipButton)
public class ToWorldButton : ExternalOnPressButton {

	 public override void OnPress(bool isPressed) {
	    if(isPressed) {

	      Logger.Log("ToWorldButton::OnPress() actual screen: "+GUITransitioner.get()._currentScreen, Logger.Level.INFO);
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen1);
	    }
	  }
}

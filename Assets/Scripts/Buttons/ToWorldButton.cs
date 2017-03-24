using UnityEngine;

//TODO make better name (cf ToCraftButton & ToEquipButton)
public class ToWorldButton : ExternalOnPressButton
{
    public override void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            // Debug.Log(this.GetType() + " OnPress() current screen: " + GUITransitioner.get()._currentScreen);
            GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen1);
        }
    }
}

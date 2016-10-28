using UnityEngine;

public class FocusMaskClickInterceptor : MonoBehaviour
{
    private void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            // Debug.Log(this.GetType() + " OnPress()");
            FocusMaskManager.get().click();
        }
    }
}

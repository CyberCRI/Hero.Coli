using UnityEngine;

public class FocusMaskClickInterceptor : MonoBehaviour
{
    private void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            //Debug.LogError("FocusMaskClickInterceptor::OnPress()");
            FocusMaskManager.get().click();
        }
    }
}

using UnityEngine;

public class CraftDeviceSlotBackground : MonoBehaviour {
    public CraftDeviceSlot slot;
    void OnPress(bool isPressed) {
        //Debug.LogError("CraftDeviceSlotBackground OnPress");
        if(isPressed)
        {
            slot.setSelected(true);
        }
    }
}

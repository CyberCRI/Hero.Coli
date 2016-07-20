public class CraftDeviceSlotBackground : ExternalOnPressButton {
    public CraftDeviceSlot slot;
    public override void OnPress(bool isPressed) {
        //Debug.LogError("CraftDeviceSlotBackground OnPress");
        if(isPressed)
        {
            slot.setSelected(true);
        }
    }
}

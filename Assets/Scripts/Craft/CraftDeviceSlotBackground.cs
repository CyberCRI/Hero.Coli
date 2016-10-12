public class CraftDeviceSlotBackground : ExternalOnPressButton {
    public CraftDeviceSlot slot;
    public override void OnPress(bool isPressed) {
        //Debug.LogError("CraftDeviceSlotBackground OnPress");
        if(isPressed)
        {
            RedMetricsManager.get ().sendEvent(TrackingEvent.SELECT, new CustomData(CustomDataTag.SLOT, slot.name.ToString()));
            slot.setSelected(true);
        }
    }
}

using UnityEngine;

public class CraftResultDevice : DisplayedDevice {
    public CraftDeviceSlot slot;
    
  public override void OnPress(bool isPressed)
  {
      // Debug.Log("CraftResultDevice OnPress("+isPressed+")");
    base.OnPress(isPressed);
    if(null != slot)
    {
        // Debug.Log("CraftResultDevice OnPress("+isPressed+") slot != null");
        RedMetricsManager.get ().sendEvent(TrackingEvent.REMOVE, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
        slot.removeAllBricks();
    }
    // else
    // {
    //     Debug.Log("CraftResultDevice OnPress("+isPressed+") slot == null");
    // }
    // Logger.Log("CraftResultDevice::OnPress("+isPressed+")");
  }

  protected override void OnHover(bool isOver)
  {
    base.OnHover(isOver);
  }
}
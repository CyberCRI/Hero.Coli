using UnityEngine;

public class CraftResultDevice : DisplayedDevice {
    public CraftDeviceSlot slot;
    
  public override void OnPress(bool isPressed)
  {
      Debug.LogError("CraftResultDevice OnPress("+isPressed+")");
    base.OnPress(isPressed);
    if(null != slot)
    {
        Debug.LogError("CraftResultDevice OnPress("+isPressed+") slot != null");
        slot.removeAllBricks();
    }
    else
    {
        Debug.LogError("CraftResultDevice OnPress("+isPressed+") slot == null");
    }
    Logger.Log("CraftResultDevice::OnPress("+isPressed+")", Logger.Level.INFO);
  }

  protected override void OnHover(bool isOver)
  {
    base.OnHover(isOver);
  }
}
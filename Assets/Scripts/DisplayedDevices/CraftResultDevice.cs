public class CraftResultDevice : DisplayedDevice {
    public CraftDeviceSlot slot;
  protected override void OnPress(bool isPressed)
  {
    base.OnPress(isPressed);
    if(null != slot)
    {
        slot.removeAllBricks();
    }
    Logger.Log("CraftResultDevice::OnPress("+isPressed+")", Logger.Level.INFO);
  }

  protected override void OnHover(bool isOver)
  {
    base.OnHover(isOver);
  }
}
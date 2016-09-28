class ListedDevice : DisplayedDevice
{

    public bool displayFeedback = true;

    void Start()
    {
        if (displayFeedback)
        {
            playFeedback();
            displayFeedback = false;
        }
    }

    public override void OnPress(bool isPressed)
    {
        // Debug.Log("ListedDevice::OnPress("+isPressed+")");

        if (CraftZoneManager.isDeviceEditionOn())
        {
            if (isPressed)
            {
                //ask craft zone to display biobricks associated to this device
                CraftZoneManager.get().setDevice(_device);
            }
        }
    }

    protected override void OnHover(bool isOver)
    {
        base.OnHover(isOver);
    }
}
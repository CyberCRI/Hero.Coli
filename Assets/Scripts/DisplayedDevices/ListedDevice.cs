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
        if (CraftZoneManager.isDeviceEditionOn())
        {
            if (isPressed)
            {
                // Debug.Log(this.GetType() + " OnPress()");
                RedMetricsManager.get().sendEvent(TrackingEvent.ADD, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
                CraftZoneManager.get().setDevice(_device);
            }
        }
    }

    protected override void OnHover(bool isOver)
    {
        base.OnHover(isOver);
    }

    // protected override void OnDestroy()
    // {
    //     base.OnDestroy();
    //     Debug.Log(this.gameObject.name + " gets destroyed");
    // }
}
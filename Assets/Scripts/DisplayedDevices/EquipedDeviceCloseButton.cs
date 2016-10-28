using UnityEngine;
using System.Collections;

public class EquipedDeviceCloseButton : MonoBehaviour {

  public EquipedDisplayedDevice device;
  private static DevicesDisplayer _devicesDisplayer;

  void Start()
  {
    if(null == _devicesDisplayer)
    {
      _devicesDisplayer = DevicesDisplayer.get();
    }
  }

	void OnPress()
  {
      //quick fix to remove closeButton
        //TODO fixme
      
    // if(null != device)
    // {
    //   if(device.askRemoveDevice())
    //   {
    //             RedMetricsManager.get ().sendEvent(TrackingEvent.UNEQUIP, new CustomData(CustomDataTag.DEVICE, device._device.getInternalName()));
    //   }
    // }
    // else
    // {
    //         Debug.LogWarning(this.GetType() + " OnPress null==device");
    // }
  }

  // void Update()
  // {
  //   if(null == _devicesDisplayer)
  //   {
  //     _devicesDisplayer = DevicesDisplayer.get();
  //   }
  //   else
  //   {
  //     gameObject.SetActive(_devicesDisplayer.IsEquipScreen());
  //   }
  // }
}

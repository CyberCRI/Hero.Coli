using UnityEngine;
using System.Collections;

public class InventoryDevice : MonoBehaviour {
  public InventoriedDisplayedDevice inventoriedDisplayedDevice;
  public GameObject equipedMask;
  private Device _device;

  void Update()
  {
    if(null == _device)
    {
      _device = inventoriedDisplayedDevice._device;
    }
    bool exists = Equipment.get().exists (d => d.getName() == _device.getName());

    equipedMask.SetActive(exists);
  }
}

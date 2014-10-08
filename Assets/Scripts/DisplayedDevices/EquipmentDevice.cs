using UnityEngine;
using System.Collections;

public class EquipmentDevice : MonoBehaviour {
  public GameObject equipedDisplayedDeviceDummy;
  public EquipedDisplayedDevice equipedDisplayedDevice;
  public EquipedDeviceCloseButton closeButton;

  void Start()
  {
    equipedDisplayedDeviceDummy.SetActive(false);
  }
}

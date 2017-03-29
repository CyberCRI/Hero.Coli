using UnityEngine;
using System.Collections;

public class InventoryDevice : MonoBehaviour {
    public InventoriedDisplayedDevice inventoriedDisplayedDevice;
    public GameObject equippedMask;
    private Device _device;

    void Update()
    {
        if(null == _device)
        {
            _device = inventoriedDisplayedDevice._device;
        }
        //TODO test BioBricks equality (cf next line)
        bool exists = Equipment.get().exists (d => d.Equals(_device));
        //bool exists = Equipment.get().exists (d => d.getInternalName() == _device.getInternalName());

        equippedMask.SetActive(exists);
    }
}

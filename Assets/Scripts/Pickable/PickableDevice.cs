using UnityEngine;

public abstract class PickableDevice : PickableItem
{
    protected override void addTo ()
    {
        Device device = _dnaBit as Device;
        if (null == device) {
            device = produceDNABit () as Device;
        }
        if (null == device) {
            Debug.LogWarning(this.GetType() + " addTo() - failed to produce non-null dna bit");
        } else {
            Logger.Log ("PickableDevice::addTo " + _dnaBit, Logger.Level.INFO);
            foreach (BioBrick brick in device.getExpressionModules().First.Value.getBioBricks()) {
                AvailableBioBricksManager.get ().addAvailableBioBrick (brick, false);
            }
            Inventory.get ().askAddDevice (device);
        }
    }
}

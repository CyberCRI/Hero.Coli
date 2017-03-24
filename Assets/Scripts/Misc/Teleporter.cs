using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    private GameObject[] _checkpoints;
    [SerializeField]
    private GameObject[] _zones;

    private GameObject getCheckPoint(int checkpointIndex)
    {
        foreach (var checkpoint in _checkpoints)
        {
            if (checkpoint.GetComponent<Checkpoint>() != null
                && checkpoint.GetComponent<Checkpoint>().index == checkpointIndex)
                return checkpoint;
        }
        // if no checkpoint with this index was found we check the origin checkpoint as a fallback
        foreach (var checkpoint in _checkpoints)
        {
            if (checkpoint.GetComponent<Checkpoint>() != null &&
                checkpoint.GetComponent<Checkpoint>().isOriginSpawnPoint)
                return checkpoint;
        }
        return null;
    }

    public void activateAll(bool activate)
    {
        foreach (var zone in _zones)
        {
            zone.SetActive(activate);
        }
    }

    public void initialize()
    {
        _checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
    }

    void addDevices(CheckPointData checkPoint)
    {
        if (checkPoint.deviceDataList != null)
        {
            foreach (var deviceEquipped in checkPoint.deviceDataList)
            {
                var device = DeviceBuilder.createDeviceFromData(deviceEquipped.deviceData);
                if (deviceEquipped.equipped)
                    CraftZoneManager.get().addAndEquipDevice(device);
                else
                    Inventory.get().askAddDevice(device);
            }
        }
        if (checkPoint.previous)
            addDevices(checkPoint.previous);
    }

    /// <summary>
    /// Teleport the player to the specified teleportIndex.
    /// </summary>
    /// <param name="teleportIndex">Teleport index.</param>
    public void teleport(int teleportIndex)
    {
        var checkpoint = getCheckPoint(teleportIndex);
        if (checkpoint != null)
        {
            if (checkpoint.GetComponent<SwitchZoneOnOff>())
            {
                checkpoint.GetComponent<SwitchZoneOnOff>().triggerSwitchZone();
            }
            CellControl.get(this.GetType().ToString()).teleport(checkpoint.transform.position);
            AvailableBioBricksManager.get().availableBioBrickData = checkpoint.GetComponent<Checkpoint>().checkPointData;
            foreach (Transform child in checkpoint.transform)
            {
                if (child.gameObject.tag == "Dummy")
                {
                    child.gameObject.SetActive(false);
                }
            }
            if (checkpoint.GetComponent<Checkpoint>().checkPointData != null)
            {
                addDevices(checkpoint.GetComponent<Checkpoint>().checkPointData);
                while (CraftZoneManager.get().getSlotCount() < checkpoint.GetComponent<Checkpoint>().requiredSlots)
                    CraftZoneManager.get().addSlot();
            }
        }
    }
}
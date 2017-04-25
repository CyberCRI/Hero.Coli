using UnityEngine;

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

    // dev method
    public void activateAll(bool activate)
    {
        foreach (var zone in _zones)
        {
            zone.SetActive(activate);
        }
    }

    public void initialize()
    {
        _checkpoints = GameObject.FindGameObjectsWithTag(Checkpoint.checkpointTag);
    }

    void addDevices(CheckPointData checkPoint)
    {
        // Debug.Log(this.GetType() + " addDevices(" + checkPoint.name + ")");
        if (checkPoint.deviceDataList != null)
        {
            foreach (DeviceEquippedData knownDevice in checkPoint.deviceDataList)
            {
                Device device = DeviceBuilder.createDeviceFromData(knownDevice.deviceData);
                if (knownDevice.equipped)
                {
                    CraftZoneManager.get().setDevice(device);
                }
                else
                {
                    Inventory.get().askAddDevice(device);
                }
            }
        }
        // Debug.Log(this.GetType() + " addDevices(" + checkPoint.name + ") done, considering rec call");
        if (checkPoint.previous)
        {
            // Debug.Log(this.GetType() + " addDevices rec call on " + checkPoint.previous.name);
            addDevices(checkPoint.previous);
        }
    }

    /// <summary>
    /// Teleport the player to the specified teleportIndex.
    /// </summary>
    /// <param name="teleportIndex">Teleport index.</param>
    public void teleport(int teleportIndex)
    {
        // Debug.Log(this.GetType() + " teleport(" + teleportIndex + ")");
        var checkpoint = getCheckPoint(teleportIndex);
        if (checkpoint != null)
        {
            if (checkpoint.GetComponent<SwitchZoneOnOff>())
            {
                // Debug.Log(this.GetType() + " teleport(" + teleportIndex + ") triggerSwitchZone");
                checkpoint.GetComponent<SwitchZoneOnOff>().triggerSwitchZone();
            }
            CellControl.get(this.GetType().ToString()).teleport(checkpoint.transform.position);

            Checkpoint checkpointComponent = checkpoint.GetComponent<Checkpoint>();
            AvailableBioBricksManager.get().availableBioBrickData = checkpointComponent.checkPointData;
            // Debug.Log(this.GetType() + " teleport(" + teleportIndex + ") availableBioBrickData set");
            // AvailableBioBricksManager.get().logAvailableBioBrickData();

            // to hide StartPosition's dummy player
            foreach (Transform child in checkpoint.transform)
            {
                if (child.gameObject.tag == "Dummy")
                {
                    child.gameObject.SetActive(false);
                }
            }

            if (checkpointComponent.checkPointData != null)
            {
                addDevices(checkpointComponent.checkPointData);
                while (CraftZoneManager.get().getSlotCount() < checkpointComponent.requiredSlots)
                    CraftZoneManager.get().addSlot();
            }
            // Debug.Log(this.GetType() + " calls setChapter");
            GameStateController.setChapter(checkpointComponent.index, Time.unscaledTime);
        }
    }
}
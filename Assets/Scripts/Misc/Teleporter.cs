using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    private GameObject[] _checkpoints;
    [SerializeField]
    private GameObject[] _zones;

	void Start()
	{
		init ();
	}

	private GameObject getCheckPoint(int checkpointIndex)
	{
		foreach (var checkpoint in _checkpoints) {
			if (checkpoint.GetComponent<Checkpoint>() != null
				&& checkpoint.GetComponent<Checkpoint> ().index == checkpointIndex)
				return checkpoint;
		}
		// if no checkpoint with this index was found we check the origin checkpoint as a fallback
		foreach (var checkpoint in _checkpoints) {
			if (checkpoint.GetComponent<Checkpoint>() != null &&
				checkpoint.GetComponent<Checkpoint> ().isOriginSpawnPoint)
				return checkpoint;
		}
		return null;
	}

	public void init()
	{
		_checkpoints = GameObject.FindGameObjectsWithTag ("Checkpoint");
	}

	/// <summary>
	/// Teleport the player to the specified teleportIndex.
	/// </summary>
	/// <param name="teleportIndex">Teleport index.</param>
    public void teleport(int teleportIndex)
    {
		var checkpoint = getCheckPoint (teleportIndex);
        if(checkpoint != null)
        {
			if (checkpoint.GetComponent<SwitchZoneOnOff> ())
				checkpoint.GetComponent<SwitchZoneOnOff> ().triggerSwitchZone ();
			CellControl.get(this.GetType().ToString()).teleport(checkpoint.transform.position);
			AvailableBioBricksManager.get ().availableBioBrickData = checkpoint.GetComponent<Checkpoint> ().availableBioBricks;
			foreach (Transform child in checkpoint.transform) {
				if (child.gameObject.tag == "Dummy")
					child.gameObject.SetActive (false);
			}
			while (CraftZoneManager.get ().getSlotCount() < checkpoint.GetComponent<Checkpoint> ().requiredSlots)
				CraftZoneManager.get ().addSlot ();
        }
    }
}
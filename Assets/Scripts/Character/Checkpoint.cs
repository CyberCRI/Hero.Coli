using UnityEngine;

public class Checkpoint : MonoBehaviour {
	public const string checkpointTag = "Checkpoint";
	/// <summary>
	/// Respawn index. Useful for the chapter select screen
	/// </summary>
	[Tooltip("Respawn index. Useful for the chapter select screen")]
	public int index;
	/// <summary>
	/// Is this instance the origin spawn point? (Only one spawn point should be the origin spawn point).
	/// </summary>
	[Tooltip("Is this instance the origin spawn point? (Only one spawn point should be the origin spawn point).")]
	public bool isOriginSpawnPoint = false;
	/// <summary>
	/// All the information about this checkpoint
	/// </summary>
	public CheckPointData checkPointData;
	/// <summary>
	/// The available bio bricks.
	/// </summary>
	public BiobrickDataCount[] requiredBioBricks { get { return checkPointData.biobrickDataList; } }
	/// <summary>
	/// The required slots.
	/// </summary>
	public DeviceEquippedData[] requiredDevices { get { return checkPointData.deviceDataList; } }
	/// <summary>
	/// The required slots.
	/// </summary>
	public int requiredSlots { get { return checkPointData.requiredSlots; }}
}

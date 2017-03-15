using UnityEngine;

public class Checkpoint : MonoBehaviour {
	/// <summary>
	/// Respawn index. Useful for the chapter select screen
	/// </summary>
	[Tooltip("Respawn index. Useful for the chapter select screen")]
	public int index;
	/// <summary>
	/// Is this instance the origin spawn point ? (Only one spawn point should be the origin spawn point).
	/// </summary>
	[Tooltip("Is this instance the origin spawn point ? (Only one spawn point should be the origin spawn point).")]
	public bool isOriginSpawnPoint = false;
}

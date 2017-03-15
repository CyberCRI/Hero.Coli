using UnityEngine;
using System.Collections;
/// <summary>
/// When clicked, starts the game at a specific checkpoint
/// </summary>
public class CheckpointMainMenuItem : MainMenuItem {
	[Tooltip("The index of the checkpoint. This index determines where the player is spawned")]
	[SerializeField] private int _checkPointIndex;
	/// <summary>
	/// Gets the index of the checkpoint. This index determines where the player is spawned
	/// </summary>
	/// <value>The index of the check point.</value>
	public int checkPointIndex {
		get {
			return _checkPointIndex;
		}
	}
	/// <summary>
	/// When the user clicks on this instance
	/// </summary>
	public override void click ()
	{
		base.click ();
	}
}

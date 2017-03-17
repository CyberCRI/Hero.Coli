using UnityEngine;
using System.Collections;
/// <summary>
/// When clicked, starts the game at a specific checkpoint
/// </summary>
public class CheckpointMainMenuItem : MainMenuItem {
	[Tooltip("The index of the checkpoint. This index determines where the player is spawned")]
	[SerializeField] private int _checkpointIndex;
	/// <summary>
	/// Gets the index of the checkpoint. This index determines where the player is spawned
	/// </summary>
	/// <value>The index of the check point.</value>
	public int checkpointIndex {
		get {
			return _checkpointIndex;
		}
	}

	[Tooltip("The gamemap the checkpoint is linked to.")]
	[SerializeField] private GameConfiguration.GameMap _gameMap = GameConfiguration.GameMap.TUTORIAL1;
	/// <summary>
	/// The gamemao the checkpoint is linked to.
	/// </summary>
	/// <value>The game map.</value>
	public GameConfiguration.GameMap gameMap {
		get {
			return _gameMap;
		}
	}
	/// <summary>
	/// Called when the user clicks on this instant. Leaves the main menu and teleports the player to the indexed checkPoint
	/// </summary>
	public override void click ()
	{
		base.click ();
		GameStateController.get ().loadWithCheckpoint (_checkpointIndex, _gameMap);
	}
}

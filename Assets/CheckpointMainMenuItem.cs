using UnityEngine;
using System.Collections;
/// <summary>
/// When clicked, starts the game at a specific checkpoint
/// </summary>
public class CheckpointMainMenuItem : MainMenuItem {
	[SerializeField] private int _checkPointIndex;
	public int checkPointIndex {
		get {
			return _checkPointIndex;
		}
	}

	public override void click ()
	{
		base.click ();
	}
}

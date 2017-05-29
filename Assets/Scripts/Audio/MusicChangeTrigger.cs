using UnityEngine;
using System.Collections;

public class MusicChangeTrigger : MusicPlayer {

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == Character.playerTag && GameStateController.get().gameState == GameState.Game) {
			PlayMusic ();
		}
	}
}

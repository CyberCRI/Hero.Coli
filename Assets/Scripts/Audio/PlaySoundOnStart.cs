using UnityEngine;
using System.Collections;

public class PlaySoundOnStart : MonoBehaviour {
	/// <summary>
	/// The sound that will be played onStart
	/// </summary>
	[Tooltip("The sound that will be played if the player enters the trigger")]
	public PlayableSound sound;

	void Start()
	{
		sound.Play ();
	}
}

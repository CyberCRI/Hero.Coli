using UnityEngine;
using System.Collections;

public abstract class MusicPlayer : MonoBehaviour {
	/// <summary>
	/// The background music.
	/// </summary>
	[Tooltip("The background music")]
	public PlayableMusic backgroundMusic;

	/// <summary>
	/// If greater than 0, the music will be played with a delay (in seconds)
	/// </summary>
	[Tooltip("If greater than 0, the music will be played with a delay (in seconds)")]
	public float delay = -1.0f;

	/// <summary>
	/// If true, no music will be played and the current music will stop playing
	/// </summary>
	[Tooltip("If true, no music will be played and the current music will stop playing")]
	public bool silence = false;

	/// <summary>
	/// Plays the music
	/// </summary>
	/// <param name="transform">Transform.</param>
	public void PlayMusic(Transform transform)
	{
		if (silence)
			SoundManager.instance.StopAllMusic ();
		// Since the delay stops the current music we should check first if current music is not already the one we intend to play
		else if (delay > 0.0f && SoundManager.instance.GetMusicAudio (backgroundMusic.clip) == null)
			StartCoroutine (DelayedMusic (backgroundMusic, transform, delay));
		else
			backgroundMusic.Play (transform);
	}

	/// <summary>
	/// Plays the music
	/// </summary>
	public void PlayMusic()
	{
		PlayMusic (backgroundMusic.sourceTransform);
	}

	protected IEnumerator DelayedMusic (PlayableMusic backgroundMusic, Transform transform, float delay)
	{
		SoundManager.instance.StopAllMusic ();
		yield return new WaitForSecondsRealtime (delay);
		backgroundMusic.Play ();
		
	}
}

using UnityEngine;
using System.Collections;

public abstract class MusicPlayer : MonoBehaviour {
	/// <summary>
	/// The background music it will change to if the player enters the trigger
	/// </summary>
	[Tooltip("The background music it will change to if the player enters the trigger")]
	public AudioClip backgroundMusicClip;

	/// <summary>
	/// How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)
	/// </summary>
	[Tooltip("How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)")]
	public float fadeInSeconds = 3.0f;

	/// <summary>
	/// How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)
	/// </summary>
	[Tooltip("How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)")]
	public float fadeOutSeconds = 3.0f;

	/// <summary>
	/// If true, no music will be played and the current music will stop playing
	/// </summary>
	[Tooltip("If true, no music will be played and the current music will stop playing")]
	public bool silence = false;

	/// <summary>
	/// The volume of the audio
	/// </summary>
	[Tooltip("The volume of the audio")]
	public float volume = 1.0f;

	/// <summary>
	/// Whether the audio will be lopped
	/// </summary>
	[Tooltip("Whether the audio will be looped")]
	public bool loop = true;

	/// <summary>
	/// Whether the audio persists in between scene changes
	/// </summary>
	[Tooltip("Whether the audio persists between scene changes")]
	public bool persist = true;

	/// <summary>
	/// If greater than 0, the music will be played with a delay (in seconds)
	/// </summary>
	[Tooltip("If greater than 0, the music will be played with a delay (in seconds)")]
	public float delay = -1.0f;

	/// <summary>
	/// Plays the music
	/// </summary>
	protected virtual void PlayMusic()
	{
		PlayMusic(this.backgroundMusicClip, this.volume, this.loop, this.persist, this.fadeInSeconds, this.fadeOutSeconds, this.delay);
	}

	protected virtual void PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float delay)
	{
		if (silence)
			SoundManager.instance.StopAllMusic ();
		// Since the delay stops the current music we should check first if current music is not already the one we intend to play
		else if (delay > 0.0f && SoundManager.instance.GetMusicAudio(clip) == null)
			StartCoroutine (DelayedMusic (clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, delay));
		else
			SoundManager.instance.PlayMusic (clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds);
	}

	protected IEnumerator DelayedMusic (AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float delay)
	{
		//SoundManager.instance.StopAllMusic ();
		yield return new WaitForSecondsRealtime (delay);
		SoundManager.instance.PlayMusic (clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds);
	}
}

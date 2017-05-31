using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class PlayableAudio {
	/// <summary>
	/// The audio clip
	/// </summary>
	[Tooltip("The audio clip")]
	public AudioClip clip;

	/// <summary>
	/// The volume of the audio
	/// </summary>
	[Tooltip("The volume of the audio")]
	[Range(0.0f, 1.0f)]
	public float volume = 1.0f;

	/// <summary>
	/// If true, the audio will start at a random point.
	/// </summary>
	public bool randomStart = false;

	/// <summary>
	/// The type of the mixer group
	/// </summary>
	public SoundManager.AudioMixerGroupType mixerGroupType {
		get {
			return GetAudioMixerGroupType ();
		}
	}

	protected int _audioId = -1;

	public int audioId {
		get { return _audioId; }
		set { _audioId = value; }
	}

	public Audio.AudioType audioType {
		get {
			return GetAudioType ();
		}
	}

	protected abstract Audio.AudioType GetAudioType();

	protected abstract SoundManager.AudioMixerGroupType GetAudioMixerGroupType();

	public abstract int Play();

	public abstract void Stop();
}

public abstract class PlayableAbstractMusic : PlayableAudio 
{
	/// <summary>
	///  How many seconds it needs for current music audio to fade out. It will override its own fade out seconds. If -1 is passed, current music will keep its own fade out seconds
	/// </summary>
	[Tooltip(" How many seconds it needs for current music audio to fade out. It will override its own fade out seconds. If -1 is passed, current music will keep its own fade out seconds")]
	public float currentMusicFadeOut = -1.0f;

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
	/// How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)
	/// </summary>
	[Tooltip("How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)")]
	public float fadeInSeconds = 1.0f;

	/// <summary>
	/// How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)
	/// </summary>
	[Tooltip("How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)")]
	public float fadeOutSeconds = 1.0f;

	/// <summary>
	/// The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null.
	/// </summary>
	[Tooltip("The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null")]
	public Transform sourceTransform;

	protected override Audio.AudioType GetAudioType ()
	{
		return Audio.AudioType.Music;
	}
}

[System.Serializable]
public class PlayableMusic : PlayableAbstractMusic {
	public List<PlayableSubMusic> subMusics;

	public override int Play ()
	{
		return Play (sourceTransform);
	}

	public int Play (Transform transform)
	{
		if (clip != null) {
			_audioId = SoundManager.instance.PlayMusic (this, subMusics);
		}
		return _audioId;
	}

	public override void Stop ()
	{
		var audio = SoundManager.instance.GetAudio (_audioId);
		if (audio != null)
			audio.Stop ();
	}

	protected override SoundManager.AudioMixerGroupType GetAudioMixerGroupType ()
	{
		return SoundManager.AudioMixerGroupType.Music;
	}
}

[System.Serializable]
public class PlayableSubMusic : PlayableAbstractMusic
{
	public SoundManager.AudioMixerGroupType audioMixerGroupType;
	public override int Play ()
	{
		return Play (sourceTransform);
	}

	public int Play (Transform transform)
	{
		if (clip != null) {
			_audioId = SoundManager.instance.PlayMusic (this);
		}
		return _audioId;
	}


	public override void Stop ()
	{
	}

	protected override SoundManager.AudioMixerGroupType GetAudioMixerGroupType()
	{
		return audioMixerGroupType;
	}
}

[System.Serializable]
public abstract class PlayableAbstractSound : PlayableAudio
{
	/// <summary>
	/// The minimum value of the pitch range. Make it equal to the max value if you don't a want random pitch
	/// </summary>
	[Tooltip("The minimum value of the pitch range. Make it equal to the max value if you don't a want random pitch")]
	public float minPitch = 1.0f;

	/// <summary>
	/// The maximum value of the pitch range. Make it equal to the min value if you don't a want random pitch
	/// </summary>
	[Tooltip("The maximum value of the pitch range. Make it equal to the min value if you don't a want random pitch")]
	public float maxPitch = 1.0f;
}
	
[System.Serializable]
public class PlayableSound : PlayableAbstractSound
{
	/// <summary>
	/// The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null.
	/// </summary>
	[Tooltip("The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null")]
	public Transform sourceTransform;


	/// <summary>
	/// How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)
	/// </summary>
	[Tooltip("How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)")]
	public float fadeInSeconds = 0.0f;

	/// <summary>
	/// How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)
	/// </summary>
	[Tooltip("How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)")]
	public float fadeOutSeconds = 0.0f;

	/// <summary>
	/// Whether the audio will be lopped
	/// </summary>
	[Tooltip("Whether the audio will be looped")]
	public bool loop = false;

	protected override Audio.AudioType GetAudioType ()
	{
		return Audio.AudioType.Sound;
	}

	public override int Play ()
	{
		return Play(sourceTransform);
	}

	public int PlayIfNotPlayed()
	{
		return PlayIfNotPlayed (sourceTransform);
	}

	public int PlayIfNotPlayed(Transform sourceTransform)
	{
		if (clip != null && SoundManager.instance.GetSoundAudio (_audioId) == null) {
			_audioId = SoundManager.instance.PlaySound (this);
		}
		return _audioId;
	}

	public int Play(Transform sourceTransform)
	{
		if (clip != null) {
			_audioId = SoundManager.instance.PlaySound (this);
		}
		return _audioId;
	}
		
	public override void Stop ()
	{
		var audio = SoundManager.instance.GetAudio (_audioId);
		if (audio != null) {
			audio.Stop ();
		}
	}

	protected override SoundManager.AudioMixerGroupType GetAudioMixerGroupType ()
	{
		return SoundManager.AudioMixerGroupType.Sound;
	}
}

[System.Serializable]
public class PlayableUISound : PlayableAbstractSound
{
	protected override Audio.AudioType GetAudioType ()
	{
		return Audio.AudioType.UISound;
	}

	public override int Play ()
	{
		if (clip != null) {
			_audioId = SoundManager.instance.PlayUISound (this);
		}
		return _audioId;
	}

	public override void Stop ()
	{
		var audio = SoundManager.instance.GetAudio (_audioId);
		if (audio != null)
			audio.Stop ();
	}

	protected override SoundManager.AudioMixerGroupType GetAudioMixerGroupType ()
	{
		return SoundManager.AudioMixerGroupType.UI;
	}
}

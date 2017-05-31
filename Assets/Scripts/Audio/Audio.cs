using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class Audio
{
	protected static int _audioCounter = 0;
	protected float _volume;
	protected float _targetVolume;
	protected float _initTargetVolume;
	protected float _tempFadeSeconds;
	protected float _fadeInterpolater;
	protected float _onFadeStartVolume;
	protected float _minPitch;
	protected float _maxPitch;
	protected AudioType _audioType;
	protected AudioClip _initClip;
	protected AudioMixerGroup _audioMixerGroup;
	protected Transform _sourceTransform;
	protected List<Audio> _subAudios = new List<Audio>();


	/// <summary>
	/// The ID of the Audio
	/// </summary>
	public int audioID { get; private set; }

	/// <summary>
	/// The audio source that is responsible for this audio
	/// </summary>
	public AudioSource audioSource { get; private set; }

	/// <summary>
	/// Audio clip to play/is playing
	/// </summary>
	public AudioClip clip {
		get {
			return audioSource == null ? _initClip : audioSource.clip;
		}
	}

	/// <summary>
	/// Whether the audio will be lopped
	/// </summary>
	public bool loop { get; set; }

	/// <summary>
	/// Whether the audio persists in between scene changes
	/// </summary>
	public bool persist { get; set; }

	/// <summary>
	/// How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)
	/// </summary>
	public float fadeInSeconds { get; set; }

	/// <summary>
	/// How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)
	/// </summary>
	public float fadeOutSeconds { get; set; }

	/// <summary>
	/// Whether the audio is currently playing
	/// </summary>
	public bool playing { get; set; }

	/// <summary>
	/// Whether the audio is paused
	/// </summary>
	public bool paused { get; private set; }

	/// <summary>
	/// Whether the audio is stopping
	/// </summary>
	public bool stopping { get; private set; }

	/// <summary>
	/// Whether the audio is created and updated at least once. 
	/// </summary>
	public bool activated { get; private set; }

	/// <summary>
	/// A list of audios that are synchronized with this Audio
	/// </summary>
	/// <value>The sub audios.</value>
	public List<Audio> subAudios { get; private set; }

	/// <summary>
	/// The corresponding playableAudio
	/// </summary>
	public PlayableAudio playableAudio { get; set; }

	public enum AudioType
	{
		Music,
		Sound,
		UISound
	}
	public Audio (AudioType audioType, AudioClip clip, AudioMixerGroup audioMixerGroup,
		bool loop, bool persist, float volume, float fadeInValue, float fadeOutValue,
		float minPitch, float maxPitch, bool randomStart,
		Transform sourceTransform, List<Audio> subAudios = null)
	{
		if (sourceTransform == null) {
			this._sourceTransform = SoundManager.instance.gameObject.transform;
		} else {
			this._sourceTransform = sourceTransform;
		}

		this.audioID = _audioCounter;
		_audioCounter++;

		this._audioType = audioType;
		this._initClip = clip;
		this.loop = loop;
		this.persist = persist;
		this._targetVolume = volume;
		this._initTargetVolume = volume;
		this._tempFadeSeconds = -1;
		this._volume = 0.0f;
		this._minPitch = minPitch;
		this._maxPitch = maxPitch;
		this.fadeInSeconds = fadeInValue;
		this.fadeOutSeconds = fadeOutValue;
		this.subAudios = subAudios != null ? subAudios : new List<Audio> ();

		this.playing = false;
		this.paused = false;
		this.stopping = false;
		this.activated = false;

		CreateAudioSource (clip, audioMixerGroup, loop, minPitch, maxPitch);
		Play (randomStart);
	}

	void CreateAudioSource (AudioClip clip, AudioMixerGroup audioMixerGroup, bool loop, float minPitch, float maxPitch)
	{
		this.audioSource = _sourceTransform.gameObject.AddComponent<AudioSource> () as AudioSource;

		this.audioSource.clip = clip;
		this.audioSource.loop = loop;
		this.audioSource.pitch = minPitch < maxPitch ? Random.Range (minPitch, maxPitch) : minPitch;
		this.audioSource.volume = 0.0f;
		this.audioSource.maxDistance = 100.0f;
		this.audioSource.rolloffMode = AudioRolloffMode.Custom;
		this.audioSource.outputAudioMixerGroup = audioMixerGroup;
		if (_sourceTransform != SoundManager.instance.gameObject.transform) {
			this.audioSource.spatialBlend = 1;
		}
	}

	/// <summary>
	/// Start playing audio clip from the beggining
	/// </summary>
	public void Play (bool randomStart)
	{
		Play (this._initTargetVolume, randomStart);
	}

	/// <summary>
	/// Start playing audio clip from the beggining
	/// </summary>
	/// <param name="volume">The target volume</param>
	public void Play (float volume, bool randomStart)
	{
		if (this.audioSource == null) {
			CreateAudioSource (_initClip, _audioMixerGroup, loop, _minPitch, _maxPitch);
		}

		this.audioSource.Play ();
		if (randomStart)
			this.audioSource.time = Random.Range (0.0f, this.audioSource.clip.length);
		this.playing = true;

		this._fadeInterpolater = 0.0f;
		this._onFadeStartVolume = this._volume;
		this._targetVolume = volume;

		foreach (var audio in subAudios) {
			audio.Play (false);
		}
	}

	/// <summary>
	/// Stop playing audio clip
	/// </summary>
	public void Stop ()
	{
		this._fadeInterpolater = 0.0f;
		this._onFadeStartVolume = this._volume;
		this._targetVolume = 0.0f;

		this.stopping = true;

		foreach (var audio in subAudios) {
			audio.Stop ();
		}
	}

	/// <summary>
	/// Pause playing audio clip
	/// </summary>
	public void Pause ()
	{
		this.audioSource.Pause ();
		this.paused = true;

		foreach (var audio in subAudios) {
			audio.Pause ();
		}
	}

	/// <summary>
	/// Resume playing audio clip
	/// </summary>
	public void Resume ()
	{
		audioSource.UnPause ();
		paused = false;

		foreach (var audio in subAudios) {
			audio.Resume ();
		}
	}

	/// <summary>
	/// Sets the audio volume
	/// </summary>
	/// <param name="volume">The target volume</param>
	public void SetVolume (float volume)
	{
		if (volume > this._targetVolume)
			SetVolume (volume, this.fadeOutSeconds);
		else
			SetVolume (volume, this.fadeInSeconds);
	}

	/// <summary>
	/// Sets the volume.
	/// </summary>
	/// <param name="volume">The target volume</param>
	/// <param name="fadeSeconds">How many seconds it needs for the audio to fade in/out to reach target volume. If passed, it will override the Audio's fade in/out seconds, but only for this transition</param>
	public void SetVolume (float volume, float fadeSeconds)
	{
		SetVolume (volume, fadeSeconds, this._volume);
	}

	/// <summary>
	/// Sets the audio volume
	/// </summary>
	/// <param name="volume">The target volume</param>
	/// <param name="fadeSeconds">How many seconds it needs for the audio to fade in/out to reach target volume. If passed, it will override the Audio's fade in/out seconds, but only for this transition</param>
	/// <param name="startVolume">Immediately set the volume to this value before beginning the fade. If not passed, the Audio will start fading from the current volume towards the target volume</param>
	public void SetVolume (float volume, float fadeSeconds, float startVolume)
	{
		this._targetVolume = Mathf.Clamp01 (volume);
		this._fadeInterpolater = 0.0f;
		this._onFadeStartVolume = startVolume;
		this._tempFadeSeconds = fadeSeconds;

		foreach (var audio in subAudios) {
			audio.SetVolume (Mathf.Min(volume, audio._volume));
		}
	}

	/// <summary>
	/// Sets the Audio 3D max distance.
	/// </summary>
	/// <param name="max">the max distance</param>
	public void Set3DMaxDistance (float max)
	{
		this.audioSource.maxDistance = max;
	}

	/// <summary>
	/// Sets the Audio 3D min distance.
	/// </summary>
	/// <param name="min">the min distance</param>
	public void Set3DMinDistance (float min)
	{
		this.audioSource.minDistance = min;
	}

	/// <summary>
	/// Sets the Audio 3D distances
	/// </summary>
	/// <param name="min">the min distance.</param>
	/// <param name="max">the max distance</param>
	public void Set3DDistance (float min, float max)
	{
		Set3DMinDistance (min);
		Set3DMaxDistance (max);
	}

	void FadeInFadeOutVolume ()
	{
		float fadeValue;
		this._fadeInterpolater += Time.unscaledDeltaTime;
		if (this._volume > this._targetVolume)
			fadeValue = this._tempFadeSeconds != -1 ? this._tempFadeSeconds : this.fadeOutSeconds;
		else
			fadeValue = this._tempFadeSeconds != -1 ? this._tempFadeSeconds : this.fadeInSeconds;

		if (fadeValue != 0)
			this._volume = Mathf.Lerp (this._onFadeStartVolume, this._targetVolume, this._fadeInterpolater / fadeValue);
		else {
			this._volume = this._targetVolume;
		}

		foreach (var audio in subAudios) {
			audio._volume = Mathf.Min (this._volume, audio._volume);
		}
	}

	void ApplyGlobalVolumeSettings ()
	{
		switch (this._audioType) {
		case AudioType.Music:
			audioSource.volume = this._volume * SoundManager.instance.globalMusicVolume * SoundManager.instance.globalVolume;
			break;
		case AudioType.Sound:
			audioSource.volume = this._volume * SoundManager.instance.globalSoundsVolume * SoundManager.instance.globalVolume;
			break;
		case AudioType.UISound:
			audioSource.volume = this._volume * SoundManager.instance.globalUISoundsVolume * SoundManager.instance.globalVolume;
			break;
		}
	}

	public virtual void Update ()
	{
		if (this.audioSource == null)
			return;

		this.activated = true;

		if (_volume != _targetVolume)
			FadeInFadeOutVolume ();
		else if (this._tempFadeSeconds != -1) {
			this._tempFadeSeconds = -1;
		}

		ApplyGlobalVolumeSettings ();

		if (_volume == 0.0f && stopping) {
			audioSource.Stop ();
			stopping = false;
			playing = false;
			paused = false;
		}

		if (audioSource.isPlaying != playing) {
			playing = audioSource.isPlaying;
		}

		foreach (var audio in subAudios) {
			if (audio.audioSource != null)
				audio.audioSource.timeSamples = this.audioSource.timeSamples;
		}
	}
}
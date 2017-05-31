using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Placed directly on the player, it manages the music and the sounds of the game
/// </summary>
public class SoundManager : MonoBehaviour
{
	private static SoundManager _instance = null;

	Dictionary<int, Audio> _musicAudio;
	Dictionary<int, Audio> _soundsAudio;
	Dictionary<int, Audio> _UISoundsAudio;

	bool _initialized = false;

	public static SoundManager instance {
		get {
			if (_instance == null) {
				_instance = (SoundManager)FindObjectOfType<SoundManager> ();
				if (_instance == null) {
					// Create gameObject and add component
					_instance = (new GameObject ("SoundManager")).AddComponent<SoundManager> ();
				}
			}
			return _instance;
		}
	}

	/// <summary>
	/// The current main background music
	/// </summary>
	/// <value>The current main background </value>
	public Audio currentMainBackgroundMusicAudio { get; private set; }

	/// <summary>
	/// When set to true, new Music Audios that have the same audio clip as any other Audio, will be ignored
	/// </summary>
	[Tooltip ("When set to true, new Music Audios that have the same audio clip as any other Audio, will be ignored")]
	public bool ignoreDuplicateMusic = true;

	/// <summary>
	/// When set to true, new Sound Audios that have the same audio clip as any other Audio, will be ignored
	/// </summary>
	[Tooltip ("When set to true, new Sound Audios that have the same audio clip as any other Audio, will be ignored")]
	public bool ignoreDuplicateSounds;

	/// <summary>
	/// When set to true, new UI Sound Audios that have the same audio clip as any other Audio, will be ignored
	/// </summary>
	[Tooltip ("When set to true, new UI Sound Audios that have the same audio clip as any other Audio, will be ignored")]
	public bool ignoreDuplicateUISounds;

	/// <summary>
	/// Global volume
	/// </summary>
	[Range (0.0f, 1.0f)]
	public float globalVolume = 1.0f;

	/// <summary>
	/// Global music volume
	/// </summary>
	[Range (0.0f, 1.0f)]
	public float globalMusicVolume = 1.0f;

	/// <summary>
	/// Global sounds volume
	/// </summary>
	[Range (0.0f, 1.0f)]
	public float globalSoundsVolume = 1.0f;

	/// <summary>
	/// Global UI sounds volume
	/// </summary>
	[Range (0.0f, 1.0f)]
	public float globalUISoundsVolume = 1.0f;

	/// <summary>
	/// The audio mixer group associated with music
	/// </summary>
	[Tooltip ("The audio mixer group associated with music")]
	public AudioMixerGroup musicMixerGroup;
	/// <summary>
	/// The audio mixer group associated with the movement
	/// </summary>
	[Tooltip ("The audio mixer group associated with the movement")]
	public AudioMixerGroup movementMusicMixerGroup;
	/// <summary>
	/// The audio mixer group associated with sounds
	/// </summary>
	[Tooltip ("The audio mixer group associated with sound")]
	public AudioMixerGroup soundsMixerGroup;

	/// <summary>
	/// Tue audio mixer group associated with UI sounds
	/// </summary>
	[Tooltip ("The audio mixer group associated with UI sounds")]
	public AudioMixerGroup UIsoundsMixerGroup;

	/// <summary>
	/// The default audio mixer snapshot
	/// </summary>
	[Tooltip ("The default audio mixer snapshot")]
	public AudioMixerSnapshot defaultSnapshot;
	/// <summary>
	/// The low-light audio mixer snapshot
	/// </summary>
	[Tooltip ("the low-light audio mixer snapshot")]
	public AudioMixerSnapshot lowlightSnapshot;
	/// <summary>
	/// The idle audio mixer snapshot
	/// </summary>
	[Tooltip ("the idle audio mixer snapshot")]
	public AudioMixerSnapshot idleSnapshot;
	/// <summary>
	/// The movement audio mixer snapshot
	/// </summary>
	[Tooltip ("the movement audio mixer snapshot")]
	public AudioMixerSnapshot movementSnapshot;

	public enum AudioMixerGroupType
	{
		Music,
		Movement,
		Sound,
		UI
	}

	void OnEnable ()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable ()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
			instance.Init ();
		} else if (_instance != this)
			Destroy (this.gameObject);
	}

	public void ActivateLowLightAudioMix (float timeToReach)
	{
		lowlightSnapshot.TransitionTo (timeToReach);
	}

	public void ActivateDefaultAudioMix (float timeToReach)
	{
		defaultSnapshot.TransitionTo (timeToReach);
	}

	public void ActivateIdleAudioMix (float timeToReach)
	{
		idleSnapshot.TransitionTo (timeToReach);
	}

	public void ActivateMovementAudioMix (float timeToReach)
	{
		movementSnapshot.TransitionTo (timeToReach);
	}

	/// <summary>
	/// Gets the audio mixer group corresponding to its type
	/// </summary>
	/// <returns>The audio mixer group.</returns>
	/// <param name="audioMixerGroupType">Audio mixer group type.</param>
	public AudioMixerGroup GetAudioMixerGroup (AudioMixerGroupType audioMixerGroupType)
	{
		AudioMixerGroup res = null;
		switch (audioMixerGroupType) {
		case AudioMixerGroupType.Music:
			res = musicMixerGroup;
			break;
		case AudioMixerGroupType.Movement:
			res = movementMusicMixerGroup;
			break;
		case AudioMixerGroupType.Sound:
			res = soundsMixerGroup;
			break;
		case AudioMixerGroupType.UI:
			res = UIsoundsMixerGroup;
			break;
		}
		return res;
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
	{
		List<int> keys;

		// Stop and remove all non-persistent music audio
		keys = new List<int> (_musicAudio.Keys);
		foreach (int key in keys) {
			var audio = _musicAudio [key];
			if (!audio.persist && audio.activated) {
				Destroy (audio.audioSource);
				_musicAudio.Remove (key);
			}
		}

		// Stop and remove all sound fx
		keys = new List<int> (_soundsAudio.Keys);
		foreach (int key in keys) {
			var audio = _soundsAudio [key];
			Destroy (audio.audioSource);
			_soundsAudio.Remove (key);
		}
	}

	void Update ()
	{
		// Update music
		var keys = new List<int> (_musicAudio.Keys);
		foreach (int key in keys) {
			var audio = _musicAudio [key];
			audio.Update ();

			// Remove all music clips that are not playing
			if ((!audio.playing && !audio.paused) || audio.clip == null) {
				Destroy (audio.audioSource);
				_musicAudio.Remove (key);
			}
		}

		// Update sound fx
		keys = new List<int> (_soundsAudio.Keys);
		foreach (int key in keys) {
			var audio = _soundsAudio [key];
			audio.Update ();

			// Remove all sound fx clips that are not playing
			if ((!audio.playing && !audio.paused) || audio.clip == null) {
				Destroy (audio.audioSource);
				_soundsAudio.Remove (key);
			}
		}

		// Update UI sound fx

		keys = new List<int> (_UISoundsAudio.Keys);
		foreach (int key in keys) {
			var audio = _UISoundsAudio [key];
			audio.Update ();

			// Remove all UI sound fx clips that are not playing
			if ((!audio.playing && !audio.paused) || audio.clip == null) {
				Destroy (audio.audioSource);
				_UISoundsAudio.Remove (key);
			}
		}
	}

	void Init ()
	{
		if (!_initialized) {
			_musicAudio = new Dictionary<int, Audio> ();
			_soundsAudio = new Dictionary<int, Audio> ();
			_UISoundsAudio = new Dictionary<int, Audio> ();

			_initialized = true;
			DontDestroyOnLoad (gameObject);
		}
	}

	void OnApplicationFocus (bool hasFocus)
	{
		if (hasFocus)
			ResumeAll ();
		else
			PauseAll ();
	}

	void OnApplicationPause (bool pauseStatus)
	{
		if (pauseStatus)
			PauseAll ();
		else
			ResumeAll ();
	}


	#region GetAudio Functions

	/// <summary>
	/// Returns the Audio that has as its id the audioID if one is found, returns null if no such Audio is found
	/// </summary>
	/// <param name="audioID">The id of the Audio to be retrieved</param>
	/// <returns>Audio that has as its id the audioID, null if no such Audio is found</returns>
	public Audio GetAudio (int audioID)
	{
		Audio audio;

		audio = GetMusicAudio (audioID);
		if (audio != null) {
			return audio;
		}

		audio = GetSoundAudio (audioID);
		if (audio != null) {
			return audio;
		}

		audio = GetUISoundAudio (audioID);
		if (audio != null) {
			return audio;
		}

		return null;
	}

	/// Returns the first occurrence of Audio that plays the given audioClip. Returns null if no such Audio is found
	/// </summary>
	/// <param name="audioClip">The audio clip of the Audio to be retrieved</param>
	/// <returns>First occurrence of Audio that has as plays the audioClip, null if no such Audio is found</returns>
	public Audio GetAudio (AudioClip audioClip)
	{
		Audio audio = GetMusicAudio (audioClip);
		if (audio != null) {
			return audio;
		}

		audio = GetSoundAudio (audioClip);
		if (audio != null) {
			return audio;
		}

		audio = GetUISoundAudio (audioClip);
		if (audio != null) {
			return audio;
		}

		return null;
	}

	/// <summary>
	/// Returns the music Audio that has as its id the audioID if one is found, returns null if no such Audio is found
	/// </summary>
	/// <param name="audioID">The id of the music Audio to be returned</param>
	/// <returns>Music Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
	public Audio GetMusicAudio (int audioID)
	{
		if (_musicAudio.ContainsKey (audioID))
			return _musicAudio [audioID];
		return null;
	}

	/// <summary>
	/// Returns the first occurrence of music Audio that plays the given audioClip. Returns null if no such Audio is found
	/// </summary>
	/// <param name="audioClip">The audio clip of the music Audio to be retrieved</param>
	/// <returns>First occurrence of music Audio that has as plays the audioClip, null if no such Audio is found</returns>
	public Audio GetMusicAudio (AudioClip audioClip)
	{
		foreach (var audio in _musicAudio.Values) {
			if (audio.clip == audioClip)
				return audio;
		}

		return null;
	}

	/// <summary>
	/// Returns the sound fx Audio that has as its id the audioID if one is found, returns null if no such Audio is found
	/// </summary>
	/// <param name="audioID">The id of the sound fx Audio to be returned</param>
	/// <returns>Sound fx Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
	public Audio GetSoundAudio (int audioID)
	{
		if (_soundsAudio.ContainsKey (audioID))
			return _soundsAudio [audioID];
		return null;
	}

	/// <summary>
	/// Returns the first occurrence of sound Audio that plays the given audioClip. Returns null if no such Audio is found
	/// </summary>
	/// <param name="audioClip">The audio clip of the sound Audio to be retrieved</param>
	/// <returns>First occurrence of sound Audio that has as plays the audioClip, null if no such Audio is found</returns>
	public Audio GetSoundAudio (AudioClip audioClip)
	{
		foreach (var audio in _soundsAudio.Values)
			if (audio.clip == audioClip)
				return audio;
		return null;
	}

	/// <summary>
	/// Returns the UI sound fx Audio that has as its id the audioID if one is found, returns null if no such Audio is found
	/// </summary>
	/// <param name="audioID">The id of the UI sound fx Audio to be returned</param>
	/// <returns>UI sound fx Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
	public Audio GetUISoundAudio (int audioID)
	{
		if (_UISoundsAudio.ContainsKey (audioID))
			return _UISoundsAudio [audioID];
		return null;
	}

	/// <summary>
	/// Returns the first occurrence of UI sound Audio that plays the given audioClip. Returns null if no such Audio is found
	/// </summary>
	/// <param name="audioClip">The audio clip of the UI sound Audio to be retrieved</param>
	/// <returns>First occurrence of UI sound Audio that has as plays the audioClip, null if no such Audio is found</returns>
	public Audio GetUISoundAudio (AudioClip audioClip)
	{
		foreach (var audio in _UISoundsAudio.Values)
			if (audio.clip == audioClip)
				return audio;
		return null;
	}

	#endregion

	#region Play Functions

	public int PlayMusic (PlayableAbstractMusic playableMusic, List<PlayableSubMusic> subMusics = null)
	{
		if (playableMusic.clip == null) {
			Debug.LogError ("Sound Manager: Audio clip is null, cannot play music", playableMusic.clip);
		}

		if (ignoreDuplicateMusic) {
			foreach (var audio in _musicAudio.Values)
				if (audio.clip == playableMusic.clip)
					return audio.audioID;
		}

		// Stop all current music playing
		StopAllMusic (playableMusic.currentMusicFadeOut);

		List<Audio> audioList = new List<Audio> ();
		if (subMusics != null) {
			foreach (var music in subMusics) {
				if (music != null) {
					var subAudio = new Audio (Audio.AudioType.Music,
						               music.clip, GetAudioMixerGroup (music.mixerGroupType), 
						               music.clip, music.persist, music.volume, music.fadeInSeconds,
						               music.fadeOutSeconds, 1.0f, 1.0f, playableMusic.randomStart, playableMusic.sourceTransform);
					audioList.Add (subAudio);
					music.audioId = subAudio.audioID;
					_musicAudio.Add (subAudio.audioID, subAudio);
				}
			}
		}

		// Create the audioSource
		var newAudio = new Audio (Audio.AudioType.Music, playableMusic.clip, GetAudioMixerGroup (playableMusic.mixerGroupType), 
			               playableMusic.loop, playableMusic.persist, playableMusic.volume, playableMusic.fadeInSeconds, playableMusic.fadeOutSeconds,
			               1.0f, 1.0f, playableMusic.randomStart, playableMusic.sourceTransform, audioList);

		// Add it to music list
		_musicAudio.Add (newAudio.audioID, newAudio);

		currentMainBackgroundMusicAudio = newAudio;

		newAudio.playableAudio = playableMusic;

		return newAudio.audioID;
	}

	public int PlaySound (PlayableSound playableSound)
	{
		if (playableSound.clip == null) {
			Debug.LogError ("Sound Manager: Audio clip is null, cannot play music", playableSound.clip);
		}

		if (ignoreDuplicateSounds) {
			foreach (var audio in _soundsAudio.Values) {
				if (audio.clip == playableSound.clip)
					return audio.audioID;
			}
		}

		// Create the audioSource
		Audio newAudio = new Audio (Audio.AudioType.Sound, playableSound.clip, GetAudioMixerGroup (playableSound.mixerGroupType), 
			                 playableSound.loop, false,
			                 playableSound.volume, playableSound.fadeInSeconds,
			                 playableSound.fadeOutSeconds, playableSound.minPitch,
			                 playableSound.maxPitch, playableSound.randomStart, playableSound.sourceTransform);

		// Add it to music list
		_soundsAudio.Add (newAudio.audioID, newAudio);

		newAudio.playableAudio = playableSound;

		return newAudio.audioID;
	}

	public int PlayUISound (PlayableUISound playableUISound)
	{
		if (playableUISound.clip == null) {
			Debug.LogError ("Sound Manager: Audio clip is null, cannot play music", playableUISound.clip);
		}

		if (ignoreDuplicateUISounds) {
			foreach (var audio in _UISoundsAudio.Values) {
				if (audio.clip == playableUISound.clip)
					return audio.audioID;
			}
		}

		// Create the audioSource
		Audio newAudio = new Audio (Audio.AudioType.UISound, playableUISound.clip,
			                 GetAudioMixerGroup (playableUISound.mixerGroupType), 
			                 false, false,
			                 playableUISound.volume, 0.0f, 0.0f,
			                 playableUISound.minPitch,
			                 playableUISound.maxPitch,
			                 playableUISound.randomStart,
			                 null);

		// Add it to music list
		_UISoundsAudio.Add (newAudio.audioID, newAudio);

		newAudio.playableAudio = playableUISound;

		return newAudio.audioID;
	}

	#endregion

	#region Stop Functions

	/// <summary>
	/// Stop all audio playing
	/// </summary>
	public void StopAll ()
	{
		StopAll (-1.0f);
	}

	/// <summary>
	/// Stop all audio playing
	/// </summary>
	/// <param name="fadeOutSeconds"> How many seconds it needs for all music audio to fade out. It will override  their own fade out seconds. If -1 is passed, all music will keep their own fade out seconds</param>
	public void StopAll (float fadeOutSeconds)
	{
		StopAllMusic (fadeOutSeconds);
		StopAllSounds ();
		StopAllUISounds ();
	}

	/// <summary>
	/// Stop all music playing
	/// </summary>
	public void StopAllMusic ()
	{
		StopAllMusic (-1.0f);
	}

	/// <summary>
	/// Stop all music playing
	/// </summary>
	/// <param name="fadeOutSeconds"> How many seconds it needs for all music audio to fade out. It will override  their own fade out seconds. If -1 is passed, all music will keep their own fade out seconds</param>
	public void StopAllMusic (float fadeOutSeconds)
	{
		foreach (var audio in _musicAudio.Values) {
			if (fadeOutSeconds > 0)
				audio.fadeOutSeconds = fadeOutSeconds;
			audio.Stop ();
		}
	}

	/// <summary>
	/// Stops all sound fx playing
	/// </summary>
	public void StopAllSounds ()
	{
		foreach (var audio in _soundsAudio.Values) {
			audio.Stop ();
		}
	}

	/// <summary>
	/// Stops all UI sound fx playing
	/// </summary>
	public void StopAllUISounds ()
	{
		foreach (var audio in _UISoundsAudio.Values) {
			audio.Stop ();
		}
	}

	#endregion

	#region Pause Functions

	/// <summary>
	/// Pause all audio playing
	/// </summary>
	public void PauseAll ()
	{
		PauseAllMusic ();
		PauseAllSounds ();
		PauseAllUISounds ();
	}

	/// <summary>
	/// Pause all music playing
	/// </summary>
	public void PauseAllMusic ()
	{
		foreach (var audio in _musicAudio.Values)
			audio.Pause ();
	}

	/// <summary>
	/// Pause all sound fx playing
	/// </summary>
	public void PauseAllSounds ()
	{
		foreach (var audio in _soundsAudio.Values)
			audio.Pause ();
	}

	/// <summary>
	/// Pause all UI sound fx playing
	/// </summary>
	public void PauseAllUISounds ()
	{
		foreach (var audio in _UISoundsAudio.Values)
			audio.Pause ();
	}

	#endregion

	#region Resume Functions

	/// <summary>
	/// Resume all audio playing
	/// </summary>
	public void ResumeAll ()
	{
		ResumeAllMusic ();
		ResumeAllSounds ();
		ResumeAllUISounds ();
	}

	/// <summary>
	/// Resume all music playing
	/// </summary>
	public void ResumeAllMusic ()
	{
		foreach (var audio in _musicAudio.Values)
			audio.Resume ();
	}

	/// <summary>
	/// Resume all sound fx playing
	/// </summary>
	public void ResumeAllSounds ()
	{
		foreach (var audio in _soundsAudio.Values)
			audio.Resume ();
	}

	/// <summary>
	/// Resume all UI sound fx playing
	/// </summary>
	public void ResumeAllUISounds ()
	{
		foreach (var audio in _UISoundsAudio.Values)
			audio.Resume ();
	}

	#endregion
}
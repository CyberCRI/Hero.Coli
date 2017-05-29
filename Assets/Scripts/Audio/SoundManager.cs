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
	/// The audio mixer group associated with sounds
	/// </summary>
	[Tooltip ("The audio mixer group associated with sound")]
	public AudioMixerGroup soundsMixerGroup;

	/// <summary>
	/// Tue audio mixer group associated with UI sounds
	/// </summary>
	[Tooltip ("The audio mixer group associated with UI sounds")]
	public AudioMixerGroup UIsoundsMixerGroup;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
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

	/// <summary>
	/// Play background music
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayMusic (AudioClip clip)
	{
		return PlayMusic (clip, 1f, false, false, 1f, 1f, -1f, null);
	}

	/// <summary>
	/// Play background music
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayMusic (AudioClip clip, float volume)
	{
		return PlayMusic (clip, volume, false, false, 1f, 1f, -1f, null);
	}

	/// <summary>
	/// Play background music
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <param name="loop">Wether the music is looped</param>
	/// <param name = "persist" > Whether the audio persists in between scene changes</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayMusic (AudioClip clip, float volume, bool loop, bool persist)
	{
		return PlayMusic (clip, volume, loop, persist, 1f, 1f, -1f, null);
	}

	/// <summary>
	/// Play background music
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <param name="loop">Wether the music is looped</param>
	/// <param name="persist"> Whether the audio persists in between scene changes</param>
	/// <param name="fadeInValue">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
	/// <param name="fadeOutValue"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayMusic (AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
	{
		return PlayMusic (clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
	}

	/// <summary>
	/// Play background music
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <param name="loop">Wether the music is looped</param>
	/// <param name="persist"> Whether the audio persists in between scene changes</param>
	/// <param name="fadeInValue">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
	/// <param name="fadeOutValue"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
	/// <param name="currentMusicfadeOutSeconds"> How many seconds it needs for current music audio to fade out. It will override its own fade out seconds. If -1 is passed, current music will keep its own fade out seconds</param>
	/// <param name="sourceTransform">The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayMusic (AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
	{
		if (clip == null) {
			Debug.LogError ("Sound Manager: Audio clip is null, cannot play music", clip);
		}

		if (ignoreDuplicateMusic) {
			foreach (var audio in _musicAudio.Values)
				if (audio.clip == clip)
					return audio.audioID;
		}

		// Stop all current music playing
		StopAllMusic (currentMusicfadeOutSeconds);

		// Create the audioSource
		var newAudio = new Audio (Audio.AudioType.Music, clip, loop, persist, volume, fadeInSeconds, fadeOutSeconds, 1.0f, 1.0f, sourceTransform);

		// Add it to music list
		_musicAudio.Add (newAudio.audioID, newAudio);

		return newAudio.audioID;
	}


	/// <summary>
	/// Play a sound fx
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlaySound (AudioClip clip)
	{
		return PlaySound (clip, 1f, false, 1.0f, 1.0f, null);
	}

	/// <summary>
	/// Play a sound fx
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlaySound (AudioClip clip, float volume)
	{
		return PlaySound (clip, volume, false, 1.0f, 1.0f, null);
	}

	/// <summary>
	/// Play a sound fx
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="loop">Wether the sound is looped</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlaySound (AudioClip clip, bool loop)
	{
		return PlaySound (clip, 1f, loop, 1.0f, 1.0f, null);
	}

	/// <summary>
	/// Play a sound fx
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <param name="loop">Wether the sound is looped</param>
	/// <param name="sourceTransform">The transform that is the source of the sound (will become 3D audio). If 3D audio is not wanted, use null</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlaySound (AudioClip clip, float volume, bool loop, float minPitch, float maxPitch, Transform sourceTransform)
	{
		if (clip == null) {
			Debug.LogError ("Sound Manager: Audio clip is null, cannot play music", clip);
		}

		if (ignoreDuplicateSounds) {
			foreach (var audio in _soundsAudio.Values) {
				if (audio.clip == clip)
					return audio.audioID;
			}
		}

		// Create the audioSource
		Audio newAudio = new Audio (Audio.AudioType.Sound, clip, loop, false, volume, 0.0f, 0.0f, minPitch, maxPitch, sourceTransform);

		// Add it to music list
		_soundsAudio.Add (newAudio.audioID, newAudio);

		return newAudio.audioID;
	}


	/// <summary>
	/// Play a UI sound fx
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayUISound (AudioClip clip)
	{
		return PlayUISound (clip, 1f, 1.0f, 1.0f);
	}

	/// <summary>
	/// Play a UI sound fx
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayUISound (AudioClip clip, float volume)
	{
		return PlayUISound (clip, volume, 1.0f, 1.0f);
	}

	/// <summary>
	/// Play a UI sound fx
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="volume"> The volume the music will have</param>
	/// <returns>The ID of the created Audio object</returns>
	public int PlayUISound (AudioClip clip, float volume, float minPitch, float maxPitch)
	{
		if (clip == null) {
			Debug.LogError ("Sound Manager: Audio clip is null, cannot play music", clip);
		}

		if (ignoreDuplicateUISounds) {
			foreach (var audio in _UISoundsAudio.Values) {
				if (audio.clip == clip)
					return audio.audioID;
			}
		}

		// Create the audioSource
		Audio newAudio = new Audio (Audio.AudioType.UISound, clip, false, false, volume, 0.0f, 0.0f, minPitch, maxPitch, null);

		// Add it to music list
		_UISoundsAudio.Add (newAudio.audioID, newAudio);

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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class AudioManager : MonoBehaviour
{
    public AudioItem[] AudioItems;

    public AudioManagerSettings Settings;

    /// <summary>
    /// List keeping track of all audio sources in the scene, used to play sound effects.
    /// </summary>
    private static List<AudioSource> audioSources = new List<AudioSource>();
    
    /// <summary>
    /// Used to acces instance method in a static context.
    /// </summary>
    private static AudioManager instance;

    public static float MasterVolume
    {
        get { return instance.Settings.MasterVolume; }
        set
        {
            instance.Settings.MasterVolume = Mathf.Clamp(value, 0, 1);

            instance.UpdateVolumeSettings();
        }
    }

    public static float MusicVolume
    {
        get { return instance.Settings.MusicVolume; }
        set
        {
            instance.Settings.MusicVolume = Mathf.Clamp(value, 0, 1);

            instance.UpdateVolumeSettings();
        }
    }

    public static float SoundEffectsVolume
    {
        get { return instance.Settings.SoundEffectsVolume; }
        set
        {
            instance.Settings.SoundEffectsVolume = Mathf.Clamp(value, 0, 1);

            instance.UpdateVolumeSettings();
        }
    }


    public bool IsAllMuted
    {
        get { return Settings.IsAllMuted; }
        private set {Settings.IsAllMuted = value; }
    }

    public bool IsMusicMuted
    {
        get { return Settings.IsMusicMuted; }
        private set { Settings.IsMusicMuted = value; }
    }

    public bool IsSoundEffectsMuted
    {
        get { return Settings.IsSoundEffectsMuted; }
        private set { Settings.IsSoundEffectsMuted = value; }
    }

    private void Awake()
    {
        // Save the static instance
        instance = this;

        RemoveAllMissingSources();
    }

	private void Start()
    {
        // Play start on awake sounds
	    for (int i = 0; i < AudioItems.Length; i++)
	    {
	        // Check the play on awake bool, and make sure the sound isn't already playing
            if (AudioItems[i].PlayOnAwake && !audioSources.Any(a => AudioItems[i].Clips.Contains(a.clip) && a.isPlaying))
	        {
                PlaySound(i, AudioItems[i].Volume); 
	        }
	    }
	}

    private void OnApplicationQuit()
    {
        // Reset audio sources list
        audioSources = new List<AudioSource>();
    }

    public static void MuteEverything()
    {
        instance.MuteAll();
    }

    public static void MuteSoundEffects()
    {
        instance.MuteAllSoundEffects();
    }

    public static void MuteMusic()
    {
        instance.MuteAllMusic();
    }

    public static void UnMuteEverything()
    {
        instance.UnMuteAll();
    }

    public static void UnMuteSoundEffects()
    {
        instance.UnMuteAllSoundEffects();
    }

    public static void UnMuteMusic()
    {
        instance.UnMuteAllMusic();
    }

    public void MuteAll()
    {
        IsAllMuted = true;

        foreach (AudioSourceComp a in audioSources.Select(a => a.GetComponent<AudioSourceComp>()))
        {
            a.Mute();
        }
    }

    public void UnMuteAll()
    {
        IsAllMuted = false;

        foreach (AudioSourceComp a in audioSources.Select(a => a.GetComponent<AudioSourceComp>()))
        {
            if ((AudioItems.FirstOrDefault(ai => ai.Clips.Contains(a.audio.clip)).Type == AudioItem.SoundType.SoundEffect &&
                IsSoundEffectsMuted)
               || (AudioItems.FirstOrDefault(ai => ai.Clips.Contains(a.audio.clip)).Type == AudioItem.SoundType.Music &&
                IsMusicMuted))
            {
                continue;
            }
            
            a.UnMute();
        }
    }

    public void MuteAllMusic()
    {
        IsMusicMuted = true;

        if (IsAllMuted)
        {
            return;
        }

        foreach (AudioSourceComp a in audioSources.Where(a => AudioItems.FirstOrDefault(ai => ai.Clips.Contains(a.clip)).Type == AudioItem.SoundType.Music).Select(a => a.GetComponent<AudioSourceComp>()))
        {
            a.Mute();
        }
    }

    public void UnMuteAllMusic()
    {
        IsMusicMuted = false;

        if (IsAllMuted)
        {
            return;
        }

        foreach (AudioSourceComp a in audioSources.Where(a => AudioItems.FirstOrDefault(ai => ai.Clips.Contains(a.clip)).Type == AudioItem.SoundType.Music).Select(a => a.GetComponent<AudioSourceComp>()))
        {
            a.UnMute();
        }
    }

    public void MuteAllSoundEffects()
    {
        IsSoundEffectsMuted = true;

        if (IsAllMuted)
        {
            return;
        }

        foreach (AudioSourceComp a in audioSources.Where(a => AudioItems.FirstOrDefault(ai => ai.Clips.Contains(a.clip)).Type == AudioItem.SoundType.SoundEffect).Select(a => a.GetComponent<AudioSourceComp>()))
        {
            a.Mute();
        }
    }

    public void UnMuteAllSoundEffects()
    {
        IsSoundEffectsMuted = false;

        if (IsAllMuted)
        {
            return;
        }

        foreach (AudioSourceComp a in audioSources.Where(a => AudioItems.FirstOrDefault(ai => ai.Clips.Contains(a.clip)).Type == AudioItem.SoundType.SoundEffect).Select(a => a.GetComponent<AudioSourceComp>()))
        {
            a.UnMute();
        }
    }

    #region Play sound by name overloads
    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    public static void PlaySound(string name)
    {
        // Play the sound, and omit explicit volume
        PlaySound(name, null, null, null);
    }

    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    /// <param name="volume">
    /// The volume factor, from 0 to 1.
    /// </param>
    public static void PlaySound(string name, float volume)
    {
        PlaySound(name, volume, null, null);
    }

    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    /// <param name="volume">
    /// The volume factor, from 0 to 1.
    /// </param>
    /// <param name="position">
    /// Where to place the audio source.
    /// </param>
    public static void PlaySound(string name, float volume, Vector3 position)
    {
        PlaySound(name, volume, position, null);
    }

    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    /// <param name="position">
    /// Where to place the audio source.
    /// </param>
    public static void PlaySound(string name, Vector3 position)
    {
        PlaySound(name, null, position, null);
    }

    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    /// <param name="position">
    /// Where to place the audio source.
    /// </param>
    /// <param name="parent">
    /// The used audio source will be a child of this transform.
    /// </param>
    public static void PlaySound(string name, Vector3 position, Transform parent)
    {
        PlaySound(name, null, position, parent);
    }

    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    /// <param name="volume">
    /// The volume factor, from 0 to 1.
    /// </param>
    /// <param name="parent">
    /// The used audio source will be a child of this transform.
    /// </param>
    public static void PlaySound(string name, float volume, Transform parent)
    {
        PlaySound(name, volume, null, parent);
    }

    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    /// <param name="parent">
    /// The used audio source will be a child of this transform.
    /// </param>
    public static void PlaySound(string name, Transform parent)
    {
        PlaySound(name, null, null, parent);
    }

    /// <summary>
    /// Plays a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    /// <param name="volume">
    /// The volume factor, from 0 to 1.
    /// </param>
    /// <param name="position">
    /// Where to place the audio source.
    /// </param>
    /// <param name="parent">
    /// The used audio source will be a child of this transform.
    /// </param>
    public static void PlaySound(string name, float? volume, Vector3? position, Transform parent)
    {
        // Find an audio item matching the given name
        AudioItem item = instance.AudioItems.FirstOrDefault(a => a.Name == name);

        if (item == null)
        {
            throw new NullReferenceException("No audio items matched the given name");
        }

        // Play the sound
        instance.PlaySound(item, volume, position, parent);
    } 
    #endregion

    /// <summary>
    /// Stops a sound matching the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the audio clip.
    /// </param>
    public static void StopSound(string name)
    {
        // Find an audio item matching the given name
        AudioItem item = instance.AudioItems.FirstOrDefault(a => a.Name == name);

        if (item == null)
        {
            throw new NullReferenceException("No audio items matched the given name");
        }

        foreach (AudioSource audioSource in audioSources)
        {
            if (item.Clips.Contains(audioSource.clip))
            {
                audioSource.Stop();
            }
        }
    }

    public bool IsAudioItemPlaying(AudioItem item)
    {
        // Remove destroyed audio sources from the list
        RemoveAllMissingSources();
        
        return audioSources.Any(audioSource => item.Clips.Contains(audioSource.clip) && audioSource.isPlaying);
    }

    /// <summary>
    /// Adds audio sources in the scene which are not contained in our list.
    /// This method should only be used in the editor to collect any leaked audio sources.
    /// It is very slow, so don't call continuously.
    /// </summary>
    public void AddLeakedAudioSources()
    {
        foreach (AudioSource audioSource in FindSceneObjectsOfType(typeof(AudioSource)).OfType<AudioSource>().Where(a => !audioSources.Contains(a)))
        {
             audioSources.Add(audioSource);
        }
    }


    /// <summary>
    /// Destroys any audio sources un the scene with a specified clip.
    /// This is used when deleting audio items.
    /// This method should only be used in the editor.
    /// It is very slow, so don't call continuously.
    /// </summary>
    /// <param name="clip">
    /// The audio clip to look for.
    /// </param>
    public void RemoveAllAudioSourcesWithClip(AudioClip clip)
    {
        AddLeakedAudioSources();

        for (int i = audioSources.Count - 1; i >= 0; i--)
        {
            if (audioSources[i].clip == clip)
            {
                DestroyImmediate(audioSources[i].gameObject);
                audioSources.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Updates all audio sources with the same clip with new settings.
    /// </summary>
    /// <param name="itemToUpdate">
    /// The audio item to update from.
    /// </param>
    public void UpdateAudioSourcesWithNewSettings(AudioItem itemToUpdate)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            // Check if audio clips are the same
            if (itemToUpdate.Clips.Contains(audioSource.clip))
            {
                // Apply settings
                ApplySettingsToAudioSource(audioSource, itemToUpdate, null);
            }
        }
    }

    /// <summary>
    /// Stops all audio sources matching the audio item.
    /// </summary>
    /// <param name="item">
    /// The audio item to stop.
    /// </param>
    public void StopAudioItem(AudioItem item)
    {
        for (int i = audioSources.Count - 1; i >= 0; i--)
        {
            // Check if audio clips are the same
            if (item.Clips.Contains(audioSources[i].clip))
            {
                // Stop the audio sources
                audioSources[i].Stop();
            }
        }
    }

    /// <summary>
    ///  Stops all audio sources from playing.
    /// </summary>
    public void StopAllSounds()
    {
        for (int i = audioSources.Count - 1; i >= 0; i--)
        {
            audioSources[i].Stop();
        }
    }

    /// <summary>
    /// Plays a sound, from a given audio item.
    /// </summary>
    /// <param name="audioItem">
    /// The audio item to play a sound from.
    /// </param>
    public void PlaySound(AudioItem audioItem)
    {
        PlaySound(audioItem, null, null, null);
    }

    /// <summary>
    /// Plays a sound with the given arguments, all overloads just delegates responsibity to this method.
    /// </summary>
    /// <param name="audioItem">
    /// The audio item to play a sound from.
    /// </param>
    /// <param name="volume">
    /// Volume factor from 0 to 1. (null means audio items standard volume)
    /// </param>
    public void PlaySound(AudioItem audioItem, float? volume, Vector3? position, Transform parent)
    {
        RemoveAllMissingSources();
        
        // We need an audio source to play a sound
        var audioSource = new AudioSource();
        bool didFindAudioSource = false;

        // Loops through all audio sources we've created so far TODO: check for destroyed audio sources
        foreach (AudioSource source in audioSources)
        {
            // If an existing audio source is not playing any sound, select that one
            if (!source.isPlaying)
            {
                audioSource = source;
                didFindAudioSource = true;
                break;
            }
        }

        // If we didn't find a usable audiosource in the scene, create a new one
        if (!didFindAudioSource)
        {
            // Create audio source
            audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();

            audioSource.gameObject.AddComponent<AudioSourceComp>();

            // Make sure play on awake defaults to false
            audioSource.playOnAwake = false;

            // TODO: Enable hide of audio sources
            //audioSource.gameObject.hideFlags = HideFlags.HideInHierarchy;

            // Add new audio source to our list
            audioSources.Add(audioSource);
        }

        // Assign position, default to origin
        audioSource.transform.position = (position ?? Vector3.zero);

        // Assign parent, null means no parent
        audioSource.transform.parent = parent;

        // Assign the clip to the selected audio source
        audioSource.clip = audioItem.GetClip();

        // Apply settings to audio source
        ApplySettingsToAudioSource(audioSource, audioItem, volume);

        // Set destroy on load
        audioSource.GetComponent<AudioSourceComp>().DoDestroyOnLoad = !audioItem.DontDestroyOnLoad;

        // Play the clip with the selected audio source
        audioSource.Play();
    }

    private static void PlaySound (int id, float? volume)
    {
        // Use the audio manager instance to play a sound
        instance.PlaySound(instance.AudioItems[id], volume ?? instance.AudioItems[id].Volume, null, null);
    }

    private static void StopSound(int id)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (instance.AudioItems[id].Clips.Contains(audioSource.clip))
            {
                audioSource.Stop();
            }
        }
    }

    private void UpdateVolumeSettings()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            AudioItem audioItem = AudioItems.FirstOrDefault(ai => ai.Clips.Contains(audioSource.clip));

            audioSource.GetComponent<AudioSourceComp>().UpdateVolume(Settings.MasterVolume, audioItem.Type == AudioItem.SoundType.Music ? Settings.MusicVolume : Settings.SoundEffectsVolume);
        }
    }

    private void ApplySettingsToAudioSource(AudioSource audioSource, AudioItem audioSettings, float? volume)
    {
       
        audioSource.volume = volume ?? audioSettings.Volume +
                             Random.Range(-audioSettings.RandomVolume, audioSettings.RandomVolume);

        audioSource.GetComponent<AudioSourceComp>().PreInitialize();

        audioSource.volume *= (audioSettings.Type == AudioItem.SoundType.Music
                                   ? Settings.MusicVolume
                                   : Settings.SoundEffectsVolume);

        audioSource.volume *= Settings.MasterVolume;

        audioSource.pitch = audioSettings.Pitch + Random.Range(-audioSettings.RandomPitch, audioSettings.RandomPitch);

        audioSource.loop = audioSettings.Loop;

        audioSource.pan = audioSettings.Pan2D;

        audioSource.GetComponent<AudioSourceComp>().Initialize();

        if (IsAllMuted // Is everything muted?
            || (audioSettings.Type == AudioItem.SoundType.Music && IsMusicMuted) // Is music muted?
            || ((audioSettings.Type == AudioItem.SoundType.SoundEffect && IsSoundEffectsMuted))) // Are sound effects muted?
        {
            // Mute the audio source
            audioSource.GetComponent<AudioSourceComp>().Mute();
        }
    }

    private void RemoveAllMissingSources()
    {
        // Remove destroyed audio sources from the list
        for (int i = audioSources.Count - 1; i >= 0; i--)
        {
            if (audioSources[i] == null)
            {
                audioSources.RemoveAt(i);
            }
        }
    }
}

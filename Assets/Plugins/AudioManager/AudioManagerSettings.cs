using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AudioManagerSettings
{

    public bool IsAllMuted;
    public bool IsMusicMuted;
    public bool IsSoundEffectsMuted;
    
    public float MasterVolume = 1;
    public float MusicVolume = 1;
    public float SoundEffectsVolume = 1;
}


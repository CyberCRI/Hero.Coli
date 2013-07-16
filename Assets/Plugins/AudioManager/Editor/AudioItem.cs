using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AudioItem
{
    public enum SoundType
    {
        SoundEffect,
        Music
    }

    public enum PlayMode
    {
        RandomAntiRepeat,
        Random,
        Sequential,
        Reverse
    }
    
    public AudioClip[] Clips;

    public SoundType Type;

    public PlayMode Mode;

    public string NameOfSyncSource;

    public bool SyncWithOtherAudioClip;

    public bool PlayOnAwake;

    public bool Loop;

    public bool DontDestroyOnLoad;

    public bool IsCollection;

    public float RandomPitch;

    public float RandomVolume;

    public float Volume = 1;

    public float Pitch = 1;

    public float Pan2D;

    public string Name;

    private int lastUsedClip = -1;

    public AudioClip GetClip()
    {
        if (!IsCollection || Clips.Length <= 1)
        {
            return Clips[0];
        }

        int clipToPlay = lastUsedClip;

        switch (Mode)
        {
            case PlayMode.RandomAntiRepeat:
                clipToPlay = Random.Range(0, Clips.Length);

                // Check if we found a random clip that is not the latest one used
                if (clipToPlay != lastUsedClip)
                {
                    break;
                }

                clipToPlay++;
                clipToPlay %= Clips.Length;
                break;
            case PlayMode.Random:
                clipToPlay = Random.Range(0, Clips.Length);
                break;
            case PlayMode.Sequential:
                clipToPlay++;
                clipToPlay %= Clips.Length;
                break;
            case PlayMode.Reverse:
                clipToPlay--;

                if (clipToPlay < 0)
                {
                    clipToPlay = Clips.Length - 1;
                }
                break;
        }

        lastUsedClip = clipToPlay;

        return Clips[clipToPlay];
    }
}


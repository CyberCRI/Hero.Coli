using UnityEngine;
using System.Collections;

public class AudioSourceComp : MonoBehaviour
{
    private bool doDestroyOnLoad;

    private AudioSource source;

    public float volume;
    public float originVolume;

    public bool DoDestroyOnLoad
    {
        get { return doDestroyOnLoad; }
        set
        {
            if (!value)
            {
                DontDestroyOnLoad(gameObject);
            }

            doDestroyOnLoad = value;
        }
    }

	void Awake ()
    {
        if (doDestroyOnLoad)
	    {
	        Destroy(gameObject);
	    }
	}

    public void PreInitialize()
    {
        source = audio;
        originVolume = source.volume;
    }

    public void Initialize()
    {
        volume = source.volume;
    }

    public void UpdateVolume(float masterVolume, float otherFactor)
    {
        volume = originVolume * masterVolume * otherFactor;
        source.volume = volume;
    }

    public void Mute()
    {
        source.volume = 0;
    }

    public void UnMute()
    {
        source.volume = volume;
    }
}

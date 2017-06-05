using UnityEngine;
using System.Collections;

public class WorldSoundManager : MonoBehaviour {
	public float lightOnOffFading;

	public PlayableSound illuminateSound;
	public PlayableSound darkIlluminateSound;
	public PlayableSound lightOffSound;
	public PlayableSound doorOpenSound;
	public PlayableSound doorCloseSound;

	void OnEnable()
	{
		AmbientLighting.onLightToggle += OnAmbientLightToggle;
		PhenoLight.onLightToggle += OnPhenoLightToggle;
		TriggeredDoor.onDoorToggle += OnDoorToggle;
	}

	void OnDisable()
	{
		AmbientLighting.onLightToggle -= OnAmbientLightToggle;
		PhenoLight.onLightToggle -= OnPhenoLightToggle;
		TriggeredDoor.onDoorToggle -= OnDoorToggle;
	}

	void OnPhenoLightToggle (PhenoLight.LightType type, bool lightOn, bool isGFP)
	{
		PlayableSound sound = null;
		switch (type) {
		case PhenoLight.LightType.Dark:
			sound = darkIlluminateSound;
			OnAmbientLightToggle (lightOn);
			break;
		case PhenoLight.LightType.Default:
			sound = illuminateSound;
			break;
		case PhenoLight.LightType.None:
			sound = lightOffSound;
			break;
		}
		if (lightOn)
		{
			sound.Play ();
			if (isGFP)
			{
				ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_gfp_start);
			}
			else
			{
				ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_rfp_start);
			}
		}
		else
		{
			sound.StopAll ();
			if (isGFP)
			{
				ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_gfp_end);
			}
			else
			{
				ArcadeManager.instance.playAnimation(ArcadeManager.Animation.bacterium_rfp_end);
			}
		}
	}

	void OnAmbientLightToggle (bool lightOn)
	{
		if (lightOn)
			SoundManager.instance.ActivateNormalLightAudioMix (lightOnOffFading);
		else
			SoundManager.instance.ActivateLowLightAudioMix (lightOnOffFading);
	}
		
	void OnDoorToggle (bool isDoorOpening)
	{
		if (isDoorOpening) {
			doorOpenSound.Play ();
			doorCloseSound.StopAll ();
		} else {
			doorCloseSound.Play ();
			doorOpenSound.StopAll ();
		}
	}
}

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

	void OnPhenoLightToggle (PhenoLight.LightType type, bool lightOn)
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
			sound.Play ();
		else {
			sound.StopAll ();
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

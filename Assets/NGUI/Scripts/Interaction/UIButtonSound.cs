//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Plays the specified sound.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Sound")]
public class UIButtonSound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
	}

	public Trigger trigger = Trigger.OnClick;
	public PlayableUISound sound;
	[SerializeField]
	private ArcadeManager.Animation _animation = ArcadeManager.Animation.none;

	void OnHover (bool isOver)
	{
		if (enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
		{
			//NGUITools.PlaySound (audioClip, volume, pitch);
			sound.Play();
			ArcadeManager.instance.playAnimation(_animation);
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			//NGUITools.PlaySound (audioClip, volume, pitch);
			sound.Play();
			ArcadeManager.instance.playAnimation(_animation);
		}
	}

	void OnClick ()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			//NGUITools.PlaySound (audioClip, volume, pitch);
			sound.Play();
			ArcadeManager.instance.playAnimation(_animation);
		}
	}
}
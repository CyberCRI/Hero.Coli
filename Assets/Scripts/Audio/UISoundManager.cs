using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISoundManager : MonoBehaviour
{
	public PlayableUISound craftMenuSound;
	public PlayableUISound mainMenuSound;

	public PlayableUISound unsetModalSound;
	public PlayableUISound setModalSound;

	public PlayableUISound menuSelectItemSound;

	public PlayableUISound mainMenuItemDepth1Sound;
	public PlayableUISound mainMenuItemDepth2Sound;
	public PlayableUISound mainMenuItemDepth3Sound;
	public PlayableUISound mainMenuItemBackSound;
	public PlayableUISound mainMenuItemBasicSound;

	// successful new genetic construction
	public PlayableUISound successfulCraftSound;
	// successful well-known genetic construction
	public PlayableUISound genericCraftSound;

	public PlayableUISound[] focusOnSoundArray;

	int focusOnSoundIndex = 0;

	void OnEnable ()
	{
		GameStateController.onMenuChange += OnMenuChange;
		ModalManager.onModalToggle += OnModalToggle;
		MainMenuManager.onMenuSelectItem += OnMenuSelectItem;
		MainMenuItem.onMainMenuItemClick += OnMainMenuItemClick;
		CraftDeviceSlot.onCraftEvent += OnCraftEvent;
		FocusMaskManager.onFocusOn += OnFocusOn;
	}

	void OnDisable ()
	{
		GameStateController.onMenuChange -= OnMenuChange;
		ModalManager.onModalToggle -= OnModalToggle;
		MainMenuManager.onMenuSelectItem -= OnMenuSelectItem;
		MainMenuItem.onMainMenuItemClick -= OnMainMenuItemClick;
		CraftDeviceSlot.onCraftEvent -= OnCraftEvent;
		FocusMaskManager.onFocusOn -= OnFocusOn;
	}

	void OnMenuChange (GameStateController.MenuType type, bool toggle)
	{
		switch (type) {
		case GameStateController.MenuType.CraftMenu:
			craftMenuSound.Play ();
			break;
		case GameStateController.MenuType.MainMenu:
			mainMenuSound.Play ();
			break;
		}
	}

	void OnMainMenuItemClick (MainMenuItem.MenuItemType itemType)
	{
		switch (itemType) {
		case MainMenuItem.MenuItemType.Depth1:
			mainMenuItemDepth1Sound.Play ();
			break;
		case MainMenuItem.MenuItemType.Depth2:
			mainMenuItemDepth2Sound.Play ();
			break;
		case MainMenuItem.MenuItemType.Depth3:
			mainMenuItemDepth3Sound.Play ();
			break;
		case MainMenuItem.MenuItemType.Basic:
			mainMenuItemBasicSound.Play ();
			break;
		case MainMenuItem.MenuItemType.Back:
			mainMenuItemDepth1Sound.Play ();
			break;
		}
	}

	void OnMenuSelectItem ()
	{
		menuSelectItemSound.Play ();
	}

	void OnModalToggle (bool playSound, bool toggle)
	{
		if (!playSound)
			return;
		if (toggle) {
			setModalSound.Play ();
			unsetModalSound.StopAll ();
		} else {
			unsetModalSound.Play ();
			setModalSound.StopAll ();
		}
	}
		
	void OnCraftEvent (bool successful)
	{
		if (successful)
		{
			successfulCraftSound.Play ();
			ArcadeManager.instance.playAnimation(ArcadeManager.Animation.gui_create_device);
		}
		else
		{
			genericCraftSound.Play ();
			ArcadeManager.instance.playAnimation(ArcadeManager.Animation.gui_assemble_device);
		}
	}
		
	void OnFocusOn ()
	{
		if (focusOnSoundArray.Length != 0) {
			if (focusOnSoundIndex < focusOnSoundArray.Length)
				focusOnSoundArray [focusOnSoundIndex].Play ();
			focusOnSoundIndex = (focusOnSoundIndex + 1) % focusOnSoundArray.Length;
		}
	}
}

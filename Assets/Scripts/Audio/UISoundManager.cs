using UnityEngine;
using System.Collections;

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
	public PlayableUISound mainMenuItemBasicSound;
	public PlayableUISound mainMenuItemBackSound;

	public PlayableUISound successfulCraftSound;
	public PlayableUISound genericCraftSound;

	void OnEnable ()
	{
		GameStateController.onMenuChange += OnMenuChange;
		ModalManager.onModalToggle += OnModalToggle;
		MainMenuManager.onMenuSelectItem += OnMenuSelectItem;
		MainMenuItem.onMainMenuItemClick += OnMainMenuItemClick;
		CraftDeviceSlot.onCraftEvent += OnCraftEvent;
	}

	void OnDisable ()
	{
		GameStateController.onMenuChange -= OnMenuChange;
		ModalManager.onModalToggle -= OnModalToggle;
		MainMenuManager.onMenuSelectItem -= OnMenuSelectItem;
		MainMenuItem.onMainMenuItemClick -= OnMainMenuItemClick;
		CraftDeviceSlot.onCraftEvent -= OnCraftEvent;
	}

	void OnMenuChange (GameStateController.MenuType type)
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
			mainMenuItemDepth1Sound.Play ();
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
			successfulCraftSound.Play ();
		else
			genericCraftSound.Play ();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowAnimationManager : MonoBehaviour {


	public bool isWorldScreenAnimPlaying = false;
	public bool isInventoryAnimPlaying = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void launchAnimation ()
	{
		//screen 1 pointer
		if ( isWorldScreenAnimPlaying == true && GUITransitioner.get()._worldScreen.activeInHierarchy)
		{

				Inventory.get().scriptAnimator.tutorialArrowAnimation.play(GUITransitioner.GameScreen.screen1);
				isWorldScreenAnimPlaying = false;

		}
		//screen 2 pointer
		else if (Inventory.get().getDeviceAdded() && GUITransitioner.get()._currentScreen == GUITransitioner.GameScreen.screen2)
		{
			if (isInventoryAnimPlaying && Inventory.get().getDeviceAdded())
			{

				ArrowAnimation.delete("InventoryDevicesSlotsPanel");
	
				isInventoryAnimPlaying = false;
			}
			Inventory.get ().scriptAnimator.tutorialArrowAnimation.play (GUITransitioner.GameScreen.screen2);
			isInventoryAnimPlaying = true;
		}
	}
}

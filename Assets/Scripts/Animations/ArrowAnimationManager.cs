using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowAnimationManager : MonoBehaviour {


	public bool worldScreenAnim = false;
	public bool inventoryAnim = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void launchAnimation ()
	{
		//screen 1 pointer
		if ( worldScreenAnim == true && GUITransitioner.get()._worldScreen.activeInHierarchy)
		{

				Inventory.get().scriptAnimator.tutorialArrowAnimation.Play(GUITransitioner.GameScreen.screen1);
				worldScreenAnim = false;

		}
		//screen 2 pointer
		else if (Inventory.get().getDeviceAdded() && GUITransitioner.get()._currentScreen == GUITransitioner.GameScreen.screen2)
		{
			if (inventoryAnim && Inventory.get().getDeviceAdded())
			{

				ArrowAnimation.Delete("InventoryDevicesSlotsPanel");
	
				inventoryAnim = false;
			}
			Inventory.get ().scriptAnimator.tutorialArrowAnimation.Play (GUITransitioner.GameScreen.screen2);
			inventoryAnim = true;
		}
	}
}

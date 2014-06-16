using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowAnimationManager : MonoBehaviour {

	public  LinkedList<ArrowAnimation> arrowList;

	public int worldScreenAnim = 0;
	//public int inventoryAnim = 0;
	
	// Use this for initialization
	void Start () {

		arrowList = new LinkedList<ArrowAnimation>();
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void launchAnim ()
	{
		if ( worldScreenAnim > 0 && GUITransitioner.get()._worldScreen.activeInHierarchy)
		{
			for(int i=0 ; i< worldScreenAnim ; i++)
			{
				Inventory.get().scriptAnimator.arrowTuto.Play(GUITransitioner.GameScreen.screen1);
				worldScreenAnim-=1;
			}
		}
		else if (Inventory.get().getDeviceAdded() && GUITransitioner.get()._currentScreen == GUITransitioner.GameScreen.screen2)
		{
			Inventory.get ().scriptAnimator.arrowTuto.Play (GUITransitioner.GameScreen.screen2);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowAnimationManager : MonoBehaviour {

	public  LinkedList<ArrowAnimation> arrowList;

	public int worldScreenAnim = 0;
	public bool inventoryAnim = false;
	
	// Use this for initialization
	void Start () {

		arrowList = new LinkedList<ArrowAnimation>();
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void launchAnim ()
	{
		//screen 1 pointer
		if ( worldScreenAnim > 0 && GUITransitioner.get()._worldScreen.activeInHierarchy)
		{
			for(int i=0 ; i< worldScreenAnim ; i++)
			{
				Inventory.get().scriptAnimator.arrowTuto.Play(GUITransitioner.GameScreen.screen1);
				worldScreenAnim-=1;
			}
		}
		//screen 2 pointer
		else if (Inventory.get().getDeviceAdded() && GUITransitioner.get()._currentScreen == GUITransitioner.GameScreen.screen2)
		{
			if (inventoryAnim)
			{
				GameObject parent = GameObject.Find ("InventoryDevicesSlotsPanel");
				Destroy (parent.transform.GetChild(parent.transform.childCount-4).gameObject.transform.Find ("TutoArrow(Clone)").gameObject);
					
				inventoryAnim = false;
			}
			Inventory.get ().scriptAnimator.arrowTuto.Play (GUITransitioner.GameScreen.screen2);
			inventoryAnim = true;
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowAnimationManager : MonoBehaviour {

	public  LinkedList<ArrowAnimation> arrowList;

	public int waitingAnim = 0;
	
	// Use this for initialization
	void Start () {

		arrowList = new LinkedList<ArrowAnimation>();
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void launchAnim ()
	{
		if ( waitingAnim > 0 && GUITransitioner.get()._worldScreen.activeInHierarchy)
		{
			for(int i=0 ; i< waitingAnim ; i++)
			{
				Inventory.get().scriptAnimator.arrowTuto.Play();
				waitingAnim-=1;
			}
		}
	}
}

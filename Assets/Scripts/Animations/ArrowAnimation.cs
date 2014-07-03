using UnityEngine;
using System.Collections;


/*
 * pointer animation for the tutorial

 * */
public class ArrowAnimation : MonoBehaviour {
	
  //TODO check usefulness
	bool toSwitch = false; // say if the animation have to be launch  next update()
	GameObject clone1;

	//variable used for the animation
	float time = 0;									//the actual state of the animation
	float step = 0.5f;
	float duration = 10;							//the duration of the animation
	bool isGoingUp = true;							//the direction of the animation (up or down)
	
	// Update is called once per frame
	void Update () {
			move ();
	}


	public void play(GUITransitioner.GameScreen screen) {
		GameObject panel;
		Vector3 targetVector;
		Vector3 rotateVector;
		if( screen == GUITransitioner.GameScreen.screen1)
		{
			if(GUITransitioner.get()._currentScreen == GUITransitioner.GameScreen.screen1)
				{
					
					panel = GameObject.Find("WorldEquipButtonPanel");

					float shift = 50f;
					targetVector = panel.transform.FindChild("WorldEquipButton").localPosition;
					targetVector.Set(targetVector.x, targetVector.y+shift, targetVector.z);
					create(targetVector, Vector3.zero, panel);
					toSwitch = false;
				}
				else
				{

					GUITransitioner.get().arrowManager.isWorldScreenAnimPlaying = true;
					toSwitch = true;
					//playing = true;

				}

		}
		if ( screen == GUITransitioner.GameScreen.screen2)
		{
				GameObject parent = GameObject.Find ("InventoryDevicesSlotsPanel");
                    
        //TODO find non-empirical solution
				panel = parent.transform.GetChild(parent.transform.childCount -3).gameObject;

				targetVector = new Vector3(0,-80,0);
				rotateVector = new Vector3(180,0,0);

				create(targetVector,rotateVector,panel);
				Inventory.get().setDeviceAdded(false);
		}
	}

	public static void delete(string name)
	{
		GameObject parent = GameObject.Find (name);
		if(null != parent)
		{
			ArrowAnimation ts = parent.transform.GetComponentInChildren<ArrowAnimation>();
			if(GUITransitioner.get ()._currentScreen == GUITransitioner.GameScreen.screen2)
				Inventory.get ().setDeviceAdded(false);
			Destroy(ts.gameObject);
		}
	}

	private void create(Vector3 vec, Vector3 rot, GameObject g) {
		//Clone the arrow
		clone1 = NGUITools.AddChild(g, this.gameObject);
		clone1.SetActive(true);

		clone1.transform.localPosition = vec;
		clone1.transform.Rotate(rot);
	}
	

	private void  move ()
	{
		Vector3 pos = gameObject.transform.localPosition;
		if(isGoingUp)
		{
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y + step,pos.z);

			if(time < duration)
				time += step;
			else
				isGoingUp = !isGoingUp;
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(pos.x, pos.y - step, pos.z);


			if(time > 0)
				time -= step;
			else
			{
				isGoingUp = !isGoingUp;
			}
		}

	}
}

using UnityEngine;
using System.Collections;


/*
 * pointer animation for the tutorial
 * */
public class ArrowAnimation : MonoBehaviour {
	
	bool toSwitch = false; // say if the animation has to be launched on next update()
	GameObject clone1;

	//variable used for the animation
	float time = 0;									//the actual state of the animation
	float step = 0.5f;
	float duration = 10;							//the duration of the animation
	bool isGoingUp = true;							//the direction of the animation (up or down)
	
	// Update is called once per frame
	void Update () {
			Move ();
	}


	public void Play(GUITransitioner.GameScreen screen) {
		GameObject g;
		Vector3 targetVector;
		Vector3 rotateVector;
		if( screen == GUITransitioner.GameScreen.screen1)
		{
			if(GUITransitioner.get()._currentScreen == GUITransitioner.GameScreen.screen1)
				{
					
					g = GameObject.Find("WorldEquipButtonPanel");

					float shift = 50f;
					targetVector = g.transform.FindChild("WorldEquipButton").localPosition;
					targetVector.Set(targetVector.x,targetVector.y+shift,targetVector.z);
					Create(targetVector,new Vector3(0,0,0),g);
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
				g = parent.transform.GetChild(parent.transform.childCount -3).gameObject;

				targetVector = new Vector3(0,-80,0);
				rotateVector = new Vector3(180,0,0);


				Create(targetVector,rotateVector,g);
				Inventory.get().setDeviceAdded(false);

		}

	}

	public static void Delete(string name)
	{
		GameObject parent = GameObject.Find (name);
		if(parent)
		{
			ArrowAnimation ts = parent.transform.GetComponentInChildren<ArrowAnimation>();
			if(GUITransitioner.get ()._currentScreen == GUITransitioner.GameScreen.screen2)
				Inventory.get ().setDeviceAdded(false);
			Destroy(ts.gameObject);
		}
	}

	private void Create(Vector3 vec, Vector3 rot, GameObject g) {



		//Clone the arrow
		clone1 = NGUITools.AddChild(g,this.gameObject);
		clone1.SetActive(true);

		clone1.transform.localPosition = new Vector3(vec.x,vec.y,vec.z);
		clone1.transform.Rotate (new Vector3(rot.x,rot.y,rot.z));


	}
	

	private void  Move ()
	{

		Vector3 pos = gameObject.transform.localPosition;
		if(isGoingUp)
		{
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y+(step),pos.z);

			if(time < duration)
				time += step;
			else
				isGoingUp = !isGoingUp;
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y-(step),pos.z);


			if(time > 0)
				time -= step;
			else
			{
				isGoingUp = !isGoingUp;
			}
		}

	}
}

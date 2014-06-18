using UnityEngine;
using System.Collections;


/*
 * pointer animation for the tutorial

 * */
public class ArrowAnimation : MonoBehaviour {
	
	bool toswitch = false; // say if the animation have to be launch  next update()
	GameObject clone1;
	GameObject clone2;

	//variable used for the animation
	float time =0;									//the actual state of the animation
	float duration = 10;							//the duration of the animation
	bool direction = true;							//the direction of the animation (up or down)

	bool InInventory = false;
	
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

					targetVector = g.transform.FindChild("WorldEquipButton").localPosition;
					targetVector.Set(targetVector.x,targetVector.y+50,targetVector.z);
					Create(targetVector,new Vector3(0,0,0),g);
					toswitch = false;
				}
				else
				{

					GUITransitioner.get().arrowManager.worldScreenAnim = true;
					toswitch = true;
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

	public void ToggleSwitch ()
	{
		toswitch = !toswitch;
	}

	private void  Move ()
	{

		Vector3 pos = gameObject.transform.localPosition;
		if(direction)
		{
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y+(0.5f),pos.z);

			if(time < duration)
				time+=0.5f;
			else
				direction = !direction;
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y-(0.5f),pos.z);


			if(time > 0)
				time-=0.5f;
			else
			{
				direction = !direction;
			}
		}

	}
}

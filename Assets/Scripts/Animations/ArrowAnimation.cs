using UnityEngine;
using System.Collections;

public class ArrowAnimation : MonoBehaviour {

	bool playing = false;
	bool toswitch = false; // say if the animation have to be launch  next update()
	GameObject clone;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void CreateAnim() {
	
		toswitch = true;
		playing = false;
	}

	public void Play() {
		if (playing == false)
		{
			if(GUITransitioner.get()._worldScreen.activeInHierarchy)
			{
				Create();
				toswitch = false;
				playing = true;
			}
			else
			{

				GUITransitioner.get().arrowManager.waitingAnim +=1;
				toswitch = true;
				//playing = true;

			}
		}
		else 
		{
			Destroy(clone);
			playing = false;
		}

	}

	private void Create() {

		Vector3 targetVector;

		GameObject g = GameObject.Find("WorldEquipButtonPanel");




		targetVector = g.transform.FindChild("WorldEquipButton").localPosition;

		//Clone the arrow
		clone = NGUITools.AddChild(g,this.gameObject);
		clone.SetActive(true);

		//GUITransitioner.get().arrowManager.arrowList.AddLast(clone.GetComponent<ArrowAnimation>());

		//Bounds sizeOfChild = g.transform.FindChild("Background").renderer.bounds;
		clone.transform.localPosition = new Vector3(targetVector.x,targetVector.y+50,targetVector.z);


	}

	public void ToggleSwitch ()
	{
		toswitch = !toswitch;
	}
}

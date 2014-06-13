using UnityEngine;
using System.Collections;

public class ArrowAnimation : MonoBehaviour {

	bool playing = false;
	bool toswitch = false; // say if the animation have to be launch  next update()
	GameObject clone;

	float time =0;
	float duration = 10;
	bool direction = true;

	// Use this for initialization
	void Start () {

	
		//iTween.MoveTo(gameObject,iTween.Hash("x",30,"time",4,"delay",1,"looptype",iTween.LoopType.pingPong));
		//iTween.MoveTo (gameObject, new Vector3(0,1f,0),4f);
	}


	
	// Update is called once per frame
	void Update () {

			movement ();

		//iTween.MoveTo(gameObject,iTween.Hash("y",10,"time",3,"looptype",iTween.LoopType.pingPong));
		//iTween.MoveTo(gameObject,iTween.Hash("x",3,"time",4,"delay",1,"looptype",iTween.LoopType.pingPong));
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

	private void  movement ()
	{

		Vector3 pos = gameObject.transform.localPosition;
		if(direction)
		{
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y+(1f/2f),pos.z);

			if(time < duration)
				time+=1f/2f;
			else
				direction = !direction;
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y-(1f/2f),pos.z);


			if(time > 0)
				time-=1f/2f;
			else
			{
				direction = !direction;
			}
		}

	}
}

using UnityEngine;
using System.Collections;

public class QueryButtonManager : MonoBehaviour {


	public GameObject mediumInfoInstruction;
	public GameObject cellInfoInstruction;
	public GameObject helpButton;

	// Use this for initialization
	void Start () {
		GameObject parent = GameObject.Find("MediumInfoPanelRoom");
		GameObject clone = NGUITools.AddChild(parent,helpButton);
		clone.SetActive(true);

		Vector3 vec = new Vector3(-210,-21,0);
		Vector3 size = new Vector3(15,15,1);


		clone.transform.localPosition = new Vector3(vec.x,vec.y,vec.z);
		clone.transform.GetChild(0).localScale = new Vector3(size.x,size.y,size.z);


		mediumInfoInstruction.transform.parent = parent.transform;
		mediumInfoInstruction.transform.localPosition = new Vector3(-160,-280,0);


		parent = GameObject.Find ("MediumInfoPanelCell");
		clone = NGUITools.AddChild(parent,helpButton);
		clone.SetActive(true);

		vec = new Vector3(-80,70,0);
		size = new Vector3(15,15,1);

		clone.transform.localPosition = new Vector3(vec.x,vec.y,vec.z);
		clone.transform.GetChild(0).localScale = new Vector3(size.x,size.y,size.z);
		
		
		cellInfoInstruction.transform.parent = parent.transform;
		cellInfoInstruction.transform.localPosition = new Vector3(-30,150,0);
	

	}
	
	// Update is called once per frame
	void Update () {
	
	}


}

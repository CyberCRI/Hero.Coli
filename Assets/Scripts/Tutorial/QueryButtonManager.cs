using UnityEngine;
using System.Collections;

public class QueryButtonManager : MonoBehaviour {


	public GameObject mediumInfoInstruction;
	public GameObject queryButton;

	// Use this for initialization
	void Start () {
		Logger.Log("QueryMnager start",Logger.Level.WARN);
		GameObject clone = NGUITools.AddChild(GameObject.Find("MediumInfoPanelRoom"),queryButton);
		clone.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}

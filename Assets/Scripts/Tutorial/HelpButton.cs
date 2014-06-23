using UnityEngine;
using System.Collections;

public class HelpButton : MonoBehaviour {

	private bool isActivate = false;

	void OnPress(bool isDown) {

		if(isDown)
		{
			bool isFound = false;
			int i = 0;

			//Get all the transform in his parent, even the inactive ones
			Transform[] tr = gameObject.transform.parent.GetComponentsInChildren<Transform>(true);

			while(!isFound &&  i< tr.Length)
			{
				// Find the InstructionPanel which was inactive, and active it
				if(tr[i].gameObject.name =="InstructionPanel")
				{
					isActivate = !isActivate;
					tr[i].transform.parent.gameObject.SetActive(isActivate);
					isFound = true;
				}
				i+=1;
			}
		}
	}
}

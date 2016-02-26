using UnityEngine;
using System.Collections.Generic;

public class TriggeredIndicator : TriggeredBehaviour {

	public List<GameObject> indicators;
	
	public override void triggerStart(){
		foreach(GameObject indicator in indicators)
		{
            if(null != indicator)
            {
                indicator.SetActive(true);
            }
        }
	}
	
	public override void triggerExit(){
		foreach(GameObject indicator in indicators)
		{
            if(null != indicator)
            {
                indicator.SetActive(false);
            }
        }
	}
	
	public override void triggerStay(){}
}

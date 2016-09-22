using UnityEngine;
using System.Collections;

public class slowDown : MonoBehaviour {
	
	public string animName;
	public float percentage = 50;
	
	void Update(){
		//percentage = Mathf.Clamp(percentage, 0f, 500f);
		GetComponent<Animation>()[animName].speed = percentage / 100f;
	}
}

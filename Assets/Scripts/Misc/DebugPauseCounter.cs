using UnityEngine;
using System.Collections;

public class DebugPauseCounter : MonoBehaviour {

    public UILabel label;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        string state = GameStateController.get ().getState().ToString();
        string pause = 0 == Time.timeScale? "P\n":"";
        label.text = pause+GameStateController.getPausesInStackCount().ToString()+"\n"+state;
	}
}

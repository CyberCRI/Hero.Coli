using UnityEngine;
using System.Collections;
using System;

public class TimeCounter : MonoBehaviour {

  private UILabel _label;
  private float _elapsed;

	// Use this for initialization
	void Start () {
    _elapsed = 0;
    _label = gameObject.GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
    _elapsed += Time.deltaTime;

    System.TimeSpan t = System.TimeSpan.FromSeconds(_elapsed);
        
    string timerFormatted = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
    
    _label.text = timerFormatted;  
	}
}


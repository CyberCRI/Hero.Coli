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

    _label.text = getFormattedTime();  
	}

  public string getFormattedTime() {
    System.TimeSpan t = System.TimeSpan.FromSeconds(_elapsed);
    return string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
  }
}


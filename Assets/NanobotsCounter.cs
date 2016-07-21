using UnityEngine;
using System.Collections;

public class NanobotsCounter : MonoBehaviour {

    private UILabel _label;

	// Use this for initialization
	void Start () {
        _label = this.GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void UpdateLabel(int numNanoBots)
    {
        _label.text = numNanoBots.ToString() + "/3";
    }
}

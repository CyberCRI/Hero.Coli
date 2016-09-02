using UnityEngine;

public class NanobotsCounter : MonoBehaviour {

    private UILabel _label;

	// Use this for initialization
	void Start () {
        _label = this.GetComponent<UILabel>();
	}

    public void updateLabel(int numNanoBots)
    {
        _label.text = numNanoBots.ToString() + "/3";
    }
}

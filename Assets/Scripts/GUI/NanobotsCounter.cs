using UnityEngine;

public class NanobotsCounter : MonoBehaviour {

    private UILabel _label;
    private int _totalNanobots;

	// Use this for initialization
	void Start () {
        _totalNanobots = GameObject.FindGameObjectsWithTag("Droid").Length;
        _label = this.GetComponent<UILabel>();
        updateLabel(0);
    }

    public void updateLabel(int numNanoBots)
    {
        _label.text = numNanoBots.ToString() + "/" + _totalNanobots.ToString();
    }
}

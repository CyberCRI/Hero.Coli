using UnityEngine;

public class NanobotsCounter : MonoBehaviour {

    private UILabel _label;
    [SerializeField]
    private int _totalNanobots;

	// Use this for initialization
	void Start () {
        _label = this.GetComponent<UILabel>();
	}

    public void updateLabel(int numNanoBots)
    {
        _label.text = numNanoBots.ToString() + "/" + _totalNanobots.ToString();
    }
}

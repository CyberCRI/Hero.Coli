using UnityEngine;

public class NanobotsCounter : MonoBehaviour {

    [SerializeField]
    private UILabel _label;
    private int _totalNanobots;
    private int actualNumNanoBot;

	// Use this for initialization
	void Start () {
        _totalNanobots = GameObject.FindGameObjectsWithTag("NanoBot").Length;
        updateLabel(0);
    }

    public void updateLabel(int numNanoBots)
    {
        _label.text = numNanoBots.ToString() + "/" + _totalNanobots.ToString();
        actualNumNanoBot = numNanoBots;
    }

    public int GetNanoCount()
    {
        return actualNumNanoBot;
    }
}

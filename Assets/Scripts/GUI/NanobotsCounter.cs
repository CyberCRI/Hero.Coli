using UnityEngine;

public class NanobotsCounter : MonoBehaviour {

    [SerializeField]
    private UILabel _label;
    private int _totalNanobots;

	// Use this for initialization
	void Start () {
        _totalNanobots = GameObject.FindGameObjectsWithTag("Droid").Length;
        updateLabel(0);
        SwitchZoneOnOff.triggerSwitchZone();
    }

    public void updateLabel(int numNanoBots)
    {
        _label.text = numNanoBots.ToString() + "/" + _totalNanobots.ToString();
    }
}

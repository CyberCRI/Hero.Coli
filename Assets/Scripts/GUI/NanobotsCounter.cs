using UnityEngine;

public class NanobotsCounter : MonoBehaviour {

    [SerializeField]
    private UILabel _label;
    private int _totalNanobots;

	// Use this for initialization
	void Start () {
        _totalNanobots = GameObject.FindGameObjectsWithTag("NanoBot").Length;
        updateLabel(0);
        GameObject go = GameObject.Find("StartZoneSetter");
        if(null != go)
        {
            go.GetComponent<SwitchZoneOnOff>().triggerSwitchZone();
        }
    }

    public void updateLabel(int numNanoBots)
    {
        _label.text = numNanoBots.ToString() + "/" + _totalNanobots.ToString();
    }
}

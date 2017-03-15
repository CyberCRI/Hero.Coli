using UnityEngine;

public class SwitchZoneOnOff : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _activeZone;

	public GameObject[] activeZone {
		get { return _activeZone; }
	}

    [SerializeField]
    private GameObject[] _inactiveZone;

	public GameObject[] inactiveZone {
		get { return _inactiveZone; }
	}
    //private static SwitchZoneOnOff _instance;

    [SerializeField]
    private bool _devMode;
    [SerializeField]
    private GameObject[] _devActiveZone;
    [SerializeField]
    private GameObject[] _devInactiveZone;

    void Awake()
    {
        //_instance = this;
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag)
        {
			triggerSwitchZone ();
        }
    }

    public void triggerSwitchZone()
    {
        GameObject[] activeZone = _devMode?_devActiveZone:_activeZone;
        GameObject[] inactiveZone = _devMode?_devInactiveZone:_inactiveZone;

        for (int i = 0; i < activeZone.Length; i++)
        {
            activeZone[i].SetActive(true);
        }
        for (int i = 0; i < inactiveZone.Length; i++)
        {
            inactiveZone[i].SetActive(false);

        }
    }
}


using UnityEngine;

public class SwitchZoneOnOff : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _activeZone;
    [SerializeField]
    private GameObject[] _inactiveZone;
    //private static SwitchZoneOnOff _instance;

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
            for (int i = 0; i < _activeZone.Length; i++)
            {
                _activeZone[i].SetActive(true);
            }
            for (int i = 0; i < _inactiveZone.Length; i++)
            {
                _inactiveZone[i].SetActive(false);
            }
        }
    }

    public void triggerSwitchZone()
    {
        for (int i = 0; i < _activeZone.Length; i++)
        {
            _activeZone[i].SetActive(true);
        }
        for (int i = 0; i < _inactiveZone.Length; i++)
        {
            _inactiveZone[i].SetActive(false);

        }
    }
}


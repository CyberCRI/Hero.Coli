using UnityEngine;
using System.Collections;

public class EndCutScene : CutScene {

    [SerializeField]
    private GameObject[] NPCs;
    [SerializeField]
    private GameObject _nanoBot;
    private NanobotsCounter _nanoCounter;
    private int iteration = 0;
    private bool _started = false;
    private PlatformMvt _platformMvt;
    [SerializeField]
    private GameObject _waypoint1;
    [SerializeField]
    private GameObject _waypoint2;
    [SerializeField]
    private GameObject _waypoint1NPC1;
    [SerializeField]
    private GameObject _waypoint2NPC1;
    [SerializeField]
    private GameObject _waypoint3NPC1;

    // Use this for initialization
    void Start () {
        _nanoCounter = GameObject.Find("NanobotsIndicator").GetComponent<NanobotsCounter>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void startCutScene()
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            NPCs[i].GetComponent<RotationUpdate>().ObjectDirectedRotationUpdate(_cellControl.gameObject);
        }
        StartCoroutine(InstantiateNanobot());
    }

    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
    }

    public override void initialize()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "CutSceneElement" && _started == false)
        {
            _started = true;
            start();
        }
    }

    void CheckForNext()
    {
        if (iteration < _nanoCounter.GetNanoCount() -1)
        {
            Debug.Log(_nanoCounter.GetNanoCount());
            iteration++;
            StopAllCoroutines();
            StartCoroutine(InstantiateNanobot());
        }
        else
        {
            StartCoroutine(WaitForSecondPart());
            iteration = 0;
        }
    }

    void SecondPart()
    {
        CopyComponent(NPCs[0].GetComponent<PlatformMvt>(), _cellControl.gameObject);
        _cellControl.gameObject.AddComponent<RotationUpdate>();
        _platformMvt = _cellControl.GetComponent<PlatformMvt>();
        _waypoint1.transform.position = _cellControl.transform.position;
        _platformMvt.ClearWaypoints();
        _platformMvt.AddWayPoint(_waypoint1);
        _platformMvt.AddWayPoint(_waypoint2);
        _platformMvt.speed = 4;

        _platformMvt = NPCs[0].GetComponent<PlatformMvt>();
        _platformMvt.ClearWaypoints();
        _platformMvt.AddWayPoint(_waypoint1NPC1);
        _platformMvt.AddWayPoint(_waypoint2NPC1);
        _platformMvt.AddWayPoint(_waypoint3NPC1);
        StartCoroutine(GoOneByOne());
    }

    IEnumerator InstantiateNanobot()
    {
        GameObject instance = Instantiate(_nanoBot, _cellControl.transform.position, _cellControl.transform.rotation) as GameObject;
        PlatformMvt platformMvt = instance.transform.GetChild(0).GetComponent<PlatformMvt>();
        platformMvt.ChangeWayPointPosition(instance.transform.position, instance.transform.rotation, 0);
        platformMvt.ChangeWayPointPosition(NPCs[iteration].transform.position, NPCs[iteration].transform.rotation, 1);
        platformMvt.enabled = true;
        yield return new WaitForSeconds(1.5f);
        CheckForNext();
        yield return null;
    }

    IEnumerator WaitForSecondPart()
    {
        yield return new WaitForSeconds(5f);
        SecondPart();
        yield return null;
    }

    IEnumerator GoOneByOne()
    {
        while (iteration < _nanoCounter.GetNanoCount())
        {
            yield return new WaitForSeconds(1.5f);
            NPCs[iteration].GetComponent<PlatformMvt>().enabled = true;
            NPCs[iteration].GetComponent<RotationUpdate>().SetIsControlledExternally(false);
            iteration++;
            yield return null;
        }
        yield return null;
    }




    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
}

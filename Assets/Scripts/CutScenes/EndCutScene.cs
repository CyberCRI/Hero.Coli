// #define QUICKTEST

using UnityEngine;
#if !QUICKTEST
using System.Collections;
#endif

public class EndCutScene : CutScene
{
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

    public override void initialize()
    {
    }


#if QUICKTEST
    public override void startCutScene()
    {
    }
    public override void endCutScene()
    {
    }
#else

    // Use this for initialization
    void Start()
    {
        _nanoCounter = GameObject.Find("NanobotsIndicator").GetComponent<NanobotsCounter>();
    }

    public override void startCutScene()
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            NPCs[i].GetComponent<RotationUpdate>().objectDirectedRotationUpdate(_cellControl.gameObject);
        }
        checkForNext();
        //StartCoroutine(InstantiateNanobot());
    }

    public override void endCutScene()
    {
        GameStateController.get().triggerEnd();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "CutSceneElement" && !_started)
        {
            _started = true;
            start();
        }
        else if (col.tag == Character.playerTag)
        {
            end();
        }
    }

    void checkForNext()
    {
        if (iteration < _nanoCounter.getNanobotCount())
        {
            // Debug.Log(_nanoCounter.GetNanoCount());
            iteration++;
            StopAllCoroutines();
            StartCoroutine(instantiateNanobot());
        }
        else
        {
            StartCoroutine(waitForSecondPart());
            iteration = 0;
        }
    }

    void secondPart()
    {
        copyComponent(NPCs[0].GetComponent<PlatformMvt>(), _cellControl.gameObject);
        _cellControl.gameObject.AddComponent<RotationUpdate>();
        _platformMvt = _cellControl.GetComponent<PlatformMvt>();
        _waypoint1.transform.position = _cellControl.transform.position;
        _platformMvt.clearWaypoints();
        _platformMvt.addWayPoint(_waypoint1);
        _platformMvt.addWayPoint(_waypoint2);
        _platformMvt.speed = 8;

        _platformMvt = NPCs[0].GetComponent<PlatformMvt>();
        _platformMvt.clearWaypoints();
        _platformMvt.addWayPoint(_waypoint1NPC1);
        _platformMvt.addWayPoint(_waypoint2NPC1);
        _platformMvt.addWayPoint(_waypoint3NPC1);
        StartCoroutine(goOneByOne());
    }

    IEnumerator instantiateNanobot()
    {
        GameObject instance = Instantiate(_nanoBot, _cellControl.transform.position, _cellControl.transform.rotation) as GameObject;
        PlatformMvt platformMvt = instance.transform.GetChild(0).GetComponent<PlatformMvt>();
        platformMvt.changeWayPointPosition(instance.transform.position, instance.transform.rotation, 0);
        platformMvt.changeWayPointPosition(NPCs[iteration].transform.position, NPCs[iteration].transform.rotation, 1);
        platformMvt.enabled = true;
        yield return new WaitForSeconds(1.5f);
        checkForNext();
        yield return null;
    }

    IEnumerator waitForSecondPart()
    {
        yield return new WaitForSeconds(5f);
        secondPart();
        yield return null;
    }

    IEnumerator goOneByOne()
    {
        while (iteration < _nanoCounter.getNanobotCount())
        {
            yield return new WaitForSeconds(1.5f);
            NPCs[iteration].GetComponent<PlatformMvt>().enabled = true;
            NPCs[iteration].GetComponent<RotationUpdate>().setIsControlledExternally(false);
            iteration++;
            yield return null;
        }
        yield return null;
    }

    Component copyComponent(Component original, GameObject destination)
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

#endif
}

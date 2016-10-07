using UnityEngine;
using System.Collections;

public class EndCutScene : CutScene {

    [SerializeField]
    private GameObject[] NPCs;
    [SerializeField]
    private GameObject _nanoBot;
    private NanobotsCounter _nanoCounter;
    private int iteration = 0;

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
        if (col.tag == "CutSceneElement")
        {
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
            end();
        }
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
}

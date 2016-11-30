// #define QUICKTEST

using UnityEngine;
using System.Collections;

public class MineTutorialCutScene : CutScene
{
    #if QUICKTEST
        private const float _endWaitingTime = 0.1f;
    #else
        private const float _endWaitingTime = 5f;
    #endif

    [SerializeField]
    private GameObject[] _dummies;
    private int _dummyIndex = 0;
    private bool _triggered = false;

    override public void initialize()
    {
    }

    public override void startCutScene()
    {
        startDummy();
    }

    private void startDummy()
    {
        if (_dummyIndex < _dummies.Length)
        {
            GameObject dummy = _dummies[_dummyIndex];
            Destroy(dummy.GetComponent<PlatformMvt>());
            dummy.GetComponent<iTweenEvent>().enabled = true;
            dummy.GetComponent<RotationUpdate>().enabled = false;
            StartCoroutine(waitForDummyDeath(dummy));
            _dummyIndex++;
        }
        else
        {
            Debug.Log(this.GetType() + " called startDummy on _dummyIndex >= _dummies.Length");
        }
    }

    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
        this.enabled = false;
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
        if (col.tag == "Door" && !_triggered)
        {
            _triggered = true;
            start();
        }
    }

    IEnumerator waitForDummyDeath(GameObject dummy)
    {
        if (_dummyIndex < _dummies.Length-1)
        {
            while (dummy != null)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(_endWaitingTime);
            end();
        }
        startDummy();
        yield return null;
    }
}

using UnityEngine;
using System.Collections;

public class Zone8CutScene : CutScene
{
    [SerializeField]
    private GameObject _dummy;
    private int _collisionIteration = 0;

    public override void initialize()
    {
    }

    public override void startCutScene()
    {
    }

    public override void endCutScene()
    {
        this.transform.parent.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == CutSceneElements.doorTag)
        {
            if (_collisionIteration == 0)
            {
                start();
                _collisionIteration++;
                StartCoroutine(waitForOpeningDoor());
            }
            else
            {
                /*Destroy(_dummy.GetComponent<RotationUpdate>());
                Destroy(_dummy.GetComponent<PlatformMvt>());
                _dummy.GetComponent<iTweenEvent>().enabled = true;
                StartCoroutine(WaitForEnd());*/
            }
        }
    }

    IEnumerator waitForOpeningDoor()
    {
        yield return new WaitForSeconds(3f);
        Destroy(_dummy.GetComponent<RotationUpdate>());
        Destroy(_dummy.GetComponent<PlatformMvt>());
        _dummy.GetComponent<iTweenEvent>().enabled = true;
        StartCoroutine(waitForEnd());
        yield return null;
    }

    IEnumerator waitForEnd()
    {
        yield return new WaitForSeconds(3f);
        end();
        yield return null;
    }
}

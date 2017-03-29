// #define QUICKTEST

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone2WallCutScene : CutScene
{
    [SerializeField]
    private PlatformMvt _pltMvt2Flagellum;
    [SerializeField]
    private PlatformMvt _pltMvt3Flagellum;
    private GameObject _player;
    [SerializeField]
    private Camera _cutSceneCam;
    private bool _moveCam = false;
    private bool _moveCam2 = false;
    [SerializeField]
    private List<GameObject> _points = new List<GameObject>();

    bool _played = false;
    bool _initialized = false;

#if QUICKTEST
        private const float _bacterium1Speed = 100f;
        private const float _bacterium2Speed = 250f;
        private const float _waitDuration1 = 0.1f;
        private const float _waitDuration2 = 0.1f;
#else
    private const float _bacterium1Speed = 10f;
    private const float _bacterium2Speed = 25f;
    private const float _waitDuration1 = 4.5f;
    private const float _waitDuration2 = 3f;
#endif

    public override void initialize()
    {
        // Debug.Log(this.GetType() + " initialize");
        _initialized = true;
    }

    public void resetCameraTarget()
    {
        _moveCam = false;
        _moveCam2 = true;
    }

    void OnTriggerEnter(Collider col)
    {
        // Debug.Log(this.GetType() + " OnTriggerEnter");
        if (_initialized && col.tag == CutSceneElements.doorTag)
        {
            if (!_played)
            {
                start();
                _played = true;
            }
        }
        else if (col.tag == "CutSceneElement")
        {
            Destroy(col.gameObject);
        }
    }

    void startEscape(PlatformMvt _pltMvt, float speed)
    {
        // Debug.Log(this.GetType() + " startEscape");
        _pltMvt.points.Remove(_pltMvt.points[3]);
        for (int i = 0; i < 3; i++)
        {
            _pltMvt.points[i] = _points[i];
            _pltMvt.setCurrentDestination(0);
            _pltMvt.speed = speed;
            _pltMvt.loop = false;
        }
    }

    public override void startCutScene()
    {
        // Debug.Log(this.GetType() + " startCutScene");
        startEscape(_pltMvt2Flagellum, _bacterium1Speed);
        StartCoroutine(waitForSecondBacterium());
    }

    public override void endCutScene()
    {
        Destroy(this.transform.parent.gameObject);
    }

    IEnumerator waitForSecondBacterium()
    {
        yield return new WaitForSeconds(_waitDuration1);
        startEscape(_pltMvt3Flagellum, _bacterium2Speed);
        yield return new WaitForSeconds(_waitDuration2);
        end();
        yield return null;
    }
}

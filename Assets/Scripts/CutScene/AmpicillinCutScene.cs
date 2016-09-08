using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmpicillinCutScene : CutScene {

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
    
    public override void initialize() { }

    public void resetCameraTarget()
    {
        _moveCam = false;
        _moveCam2 = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Door")
        {
            if(!_played)
            {
                start();
                _played = true;
            }
        }
        if (col.tag == "CutSceneElement")
        {
            Destroy(col.gameObject);
        }
    }

    void startEscape(PlatformMvt _pltMvt, float speed)
    {
        _pltMvt.points.Remove(_pltMvt.points[3]);
        for (int i = 0; i < 3; i++)
        {
            _pltMvt.points[i] = _points[i];
            _pltMvt.SetCurrentDestination(0);
            _pltMvt.speed = speed;
            _pltMvt.loop = false;
        }
    }

    public override void startCutScene()
    {
        startEscape(_pltMvt2Flagellum, 10);
        StartCoroutine(waitForSecondBacteria());
    }

    public override void endCutScene() {
        Destroy(this.transform.parent.gameObject);
        _cellControl.freezePlayer(false);
    }

    IEnumerator waitForSecondBacteria()
    {
        yield return new WaitForSeconds(4.5f);
        startEscape(_pltMvt3Flagellum, 25);
        yield return new WaitForSeconds(3f);
        end();
        yield return null;
    }
}

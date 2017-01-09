// #define QUICKTEST

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CutScene : CutSceneElements
{

    private const float _blackBarWait1 = 0.1f;
#if QUICKTEST
    private const float _blackBarWait2 = 0.1f;
#else
    private const float _blackBarWait2 = 2.5f;
#endif

    private const float _normalTimeScale = 1f;
    private const float _highTimeScale = 50f;
    private const string _iTweenSpeedKey = "speed";
    private const float _speedIncreaseFactor = 10;
    private Dictionary<iTweenEvent, float> _iTweenSpeeds = new Dictionary<iTweenEvent, float>();
    // determines whether this cut scene is to be reinitialized when the player enters the cut scene instantiator's collider
    // necessary for cut scenes that allow the player to move and to die
    protected bool _reinstantiateOnTrigger = false;
    public bool reinstantiateOnTrigger()
    {
        return _reinstantiateOnTrigger;
    } 

    // must be implemented in each cut scene
    public abstract void initialize();

    // must be implemented in each cut scene
    public abstract void startCutScene();

    public static new void reset()
    {
        // Debug.Log("CutScene reset");
        CutSceneElements.reset();
        _isPlaying = false;
    }

    void OnEnable()
    {
        // cut scene initialization
        initialize();
    }

    private void saveAndEditAlliTweenEvents()
    {
        object speedObject;
        float speed;
        foreach (iTweenEvent itEvent in GameObject.FindObjectsOfType<iTweenEvent>())
        {
            if ((null != itEvent) && itEvent.Values.TryGetValue(_iTweenSpeedKey, out speedObject))
            {
                speed = (float)speedObject;
                _iTweenSpeeds.Add(itEvent, speed);
                itEvent.Values.Remove(_iTweenSpeedKey);
                itEvent.Values.Add(_iTweenSpeedKey, speed * _speedIncreaseFactor);
            }
        }
    }

    private void reinitializeiTweenEvents()
    {
        foreach (KeyValuePair<iTweenEvent, float> entry in _iTweenSpeeds)
        {
            entry.Key.Values.Remove(_iTweenSpeedKey);
            entry.Key.Values.Add(_iTweenSpeedKey, entry.Value);
        }
    }

    private static bool _isPlaying = false;
    public static bool isPlaying()
    {
        return _isPlaying;
    }

    // must be called when starting a cut scene
    public void start()
    {
        _isPlaying = true;
#if QUICKTEST
        Time.timeScale = _highTimeScale;
        saveAndEditAlliTweenEvents();
#endif        
        _cellControl.freezePlayer(true);
        FocusMaskManager.get().blockClicks(true);
        _blackBar.closeBar(true);
        StartCoroutine(waitForBlackBar(true));
        //startCutScene ();
        _cullingMaskHandler.hideInterface(true);
    }

    // must be implemented in each cut scene
    public abstract void endCutScene();

    // must be called when ending a cut scene
    public void end()
    {
#if QUICKTEST
        Time.timeScale = _normalTimeScale;
        reinitializeiTweenEvents();
#endif
        FocusMaskManager.get().blockClicks(false);
        _blackBar.closeBar(false);
        StartCoroutine(waitForBlackBar(false));
        //_cullingMaskHandler.hideInterface(false);
        //_cellControl.freezePlayer(false);
        //endCutScene();
        //this.enabled = false;
    }

    IEnumerator waitForBlackBar(bool start)
    {
        if (start)
        {
            setCutSceneCamera(start);
            _cellControl.freezePlayer(true);
        }
        yield return new WaitForSeconds(_blackBarWait1);


        yield return new WaitForSeconds(_blackBarWait2);
        if (start)
        {
            startCutScene();
        }
        else
        {
            setCutSceneCamera(start);
            endCutScene();
            _cellControl.freezePlayer(false);
            _isPlaying = false;
        }
        yield return null;
    }

    public void setCutSceneCamera(bool value)
    {
        _cullingMaskHandler.hideInterface(value);
    }
}

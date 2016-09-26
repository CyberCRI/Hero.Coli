// #define QUICKTEST
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CutScene : CutSceneElements {

    private const float blackBarWait1 = 0.1f;
#if QUICKTEST
    private const float blackBarWait2 = 0.1f;
#else
    private const float blackBarWait2 = 1.5f;
#endif

    private const float normalTimeScale = 1f;
    private const float highTimeScale = 50f;

    void OnEnable()
    {
        // cut scene initialization
        initialize();
    }

    // must be implemented in each cut scene
    public abstract void initialize ();  

	// must be implemented in each cut scene
    public abstract void startCutScene ();    
    
    private const string iTweenSpeedKey = "speed";
    private const float _speedIncreaseFactor = 10;
    private Dictionary<iTweenEvent, float> iTweenSpeeds = new Dictionary<iTweenEvent, float>();

    private void saveAndEditAlliTweenEvents()
    {
        object speedObject;
        float speed;
        foreach(iTweenEvent itEvent in GameObject.FindObjectsOfType<iTweenEvent>())
        {
            if((null != itEvent) && itEvent.Values.TryGetValue(iTweenSpeedKey, out speedObject))
            {
                speed = (float)speedObject;
                iTweenSpeeds.Add(itEvent, speed);
                itEvent.Values.Remove(iTweenSpeedKey);
                itEvent.Values.Add(iTweenSpeedKey, speed * _speedIncreaseFactor);
            }
        }
    }

    private void reinitializeiTweenEvents()
    {
        foreach(KeyValuePair<iTweenEvent, float> entry in iTweenSpeeds)
        {
            entry.Key.Values.Remove(iTweenSpeedKey);
            entry.Key.Values.Add(iTweenSpeedKey, entry.Value);
        }
    }

    // must be called when starting a cut scene
	public void start () {
#if QUICKTEST
        Time.timeScale = highTimeScale;
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
    public abstract void endCutScene ();
    
    // must be called when ending a cut scene
    public void end () {
#if QUICKTEST
        Time.timeScale = normalTimeScale;
        reinitializeiTweenEvents();
#endif
		FocusMaskManager.get().blockClicks(false);
        _blackBar.closeBar(false);
        StartCoroutine(waitForBlackBar(false));
        _cullingMaskHandler.hideInterface(false);
        _cellControl.freezePlayer(false);
        //endCutScene();
        //this.enabled = false;
    }

    IEnumerator waitForBlackBar(bool start)
    {
        yield return new WaitForSeconds(blackBarWait1);
        SetCutSceneCamera(start);
        _cellControl.freezePlayer(true);
        yield return new WaitForSeconds(blackBarWait2);
        if (start == true)
        {
            startCutScene();
        }
        else
        {
            endCutScene();
        }
        yield return null;
    }

    public void SetCutSceneCamera(bool value)
    {
        _cullingMaskHandler.hideInterface(value);
    }
}

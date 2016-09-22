//#define QUICKTEST
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CutScene : MonoBehaviour {

    private const float blackBarWait1 = 0.1f;
#if QUICKTEST
    private const float blackBarWait2 = 0.1f;
#else
    private const float blackBarWait2 = 1.5f;
#endif

    private CellControl __cellControl;
    protected CellControl _cellControl
    {
        get {
            __cellControl = (null == __cellControl)?GameObject.FindGameObjectWithTag("Player").GetComponent<CellControl>():__cellControl;
            return __cellControl;
        }
    }
    
    protected static CutSceneBlackBarHandler _blackBar;
    private static CullingMaskHandler _cullingMaskHandler;
    private int _originCullingMask;
    protected static Camera _cutSceneCamera;
    protected static BoundCamera _boundCamera;
    
    private T lazyInitObject<T>(T t, string gameObjectOrTagName, bool isTag = false)
    {
        if (null != t)
        {
            return t;
        }
        else
        {
            if(isTag)
            {
                //Debug.LogError(" GameObject.FindGameObjectWithTag("+gameObjectOrTagName+")="+GameObject.FindGameObjectWithTag(gameObjectOrTagName));
                return GameObject.FindGameObjectWithTag(gameObjectOrTagName).GetComponent<T>();
            }
            else
            {
                //Debug.LogError(" GameObject.Find("+gameObjectOrTagName+")="+GameObject.Find(gameObjectOrTagName));
                return GameObject.Find(gameObjectOrTagName).GetComponent<T>();   
            }
        }
    }

    void OnEnable()
    {
        // initialization of static elements
        _blackBar = lazyInitObject<CutSceneBlackBarHandler>(_blackBar, "CutSceneBlackBars");
        _cullingMaskHandler = lazyInitObject<CullingMaskHandler>(_cullingMaskHandler, "InterfaceCamera", true);
        _cutSceneCamera = lazyInitObject<Camera>(_cutSceneCamera, "CutSceneCamera");
        _boundCamera = lazyInitObject<BoundCamera>(_boundCamera, "MainCamera", true);

        // cut scene initialization
        initialize ();
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
        Time.timeScale = 50f;
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
        Time.timeScale = 1f;
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

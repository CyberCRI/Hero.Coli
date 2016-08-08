using UnityEngine;
using System.Collections;

public abstract class CutScene : MonoBehaviour {

    private CellControl __cellControl;
    protected CellControl _cellControl
    {
        get {
            __cellControl = (null == __cellControl)?GameObject.FindGameObjectWithTag("Player").GetComponent<CellControl>():__cellControl;
            return __cellControl;
        }
    }
    
    private static CutSceneBlackBarHandler _blackBar;
    private static CullingMaskHandler _cullingMaskHandler;
    private int _originCullingMask;
    private static Camera _cutSceneCamera;
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
        _blackBar = lazyInitObject<CutSceneBlackBarHandler>(_blackBar, "CutSceneBlackBar");
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
    
    // must be called when starting a cut scene
	public void start () {
        _cellControl.freezePlayer(true);
		FocusMaskManager.get().blockClicks(true);
        StartCoroutine(WaitForBlackBar(true));
        //startCutScene ();
        _blackBar.closeBar(true);
        _cullingMaskHandler.hideInterface(true);
        _cutSceneCamera.enabled = true;
	}
	
    // must be implemented in each cut scene
    public abstract void endCutScene ();
    
    // must be called when ending a cut scene
    public void end () {
		FocusMaskManager.get().blockClicks(false);
        _cellControl.freezePlayer(false);
        StartCoroutine(WaitForBlackBar(false));
        //endCutScene ();
        _blackBar.closeBar(false);
        _cullingMaskHandler.hideInterface(false);
        _cutSceneCamera.enabled = false;
    }

    IEnumerator WaitForBlackBar(bool start)
    {
        yield return new WaitForSeconds(1.5f);
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
}

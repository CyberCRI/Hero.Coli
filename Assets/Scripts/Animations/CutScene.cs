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
    private static Camera _cameraCutScene;
    
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
        _blackBar = lazyInitObject<CutSceneBlackBarHandler>(_blackBar, "CutSceneBlackBar");
        _cullingMaskHandler = lazyInitObject<CullingMaskHandler>(_cullingMaskHandler, "InterfaceCamera", true);
        _cameraCutScene = lazyInitObject<Camera>(_cameraCutScene, "CutSceneCamera");
    }
    
	// must be implemented in each cut scene
    public abstract void startCutScene ();    
    
    // must be called when starting a cut scene
	public void start () {
        _cellControl.freezePlayer(true);
		FocusMaskManager.get().blockClicks(true);
        StartCoroutine(WaitForBlackBar(true));
        //startCutScene ();
        _blackBar.CloseBar(true);
        _cullingMaskHandler.hideInterface(true);
        _cameraCutScene.enabled = true;
	}
	
    // must be implemented in each cut scene
    public abstract void endCutScene ();
    
    // must be called when ending a cut scene
    public void end () {
		FocusMaskManager.get().blockClicks(false);
        _cellControl.freezePlayer(false);
        StartCoroutine(WaitForBlackBar(false));
        //endCutScene ();
        _blackBar.CloseBar(false);
        _cullingMaskHandler.hideInterface(false);
        _cameraCutScene.enabled = false;
    }

    IEnumerator WaitForBlackBar(bool start)
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("1");
        if (start == true)
        {
            startCutScene();
            Debug.Log("2");
        }
        else
        {
            endCutScene();
        }
        yield return null;
    }
}

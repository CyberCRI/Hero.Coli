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
    
    private CutSceneBlackBarHandler _blackBar;
    private CullingMaskHandler _cullingMaskHandler;
    private int _originCullingMask;
    private Camera _cameraCutScene;

    void OnEnable()
    {
        _blackBar = GameObject.Find("CinematicBlackBar").GetComponent<CutSceneBlackBarHandler>();
        _cullingMaskHandler = GameObject.FindGameObjectWithTag("CameraInterface").GetComponent<CullingMaskHandler>();
        _cameraCutScene = GameObject.Find("CameraCutScene").GetComponent<Camera>();
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
        _cullingMaskHandler.HideInterface(true);
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
        _cullingMaskHandler.HideInterface(false);
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

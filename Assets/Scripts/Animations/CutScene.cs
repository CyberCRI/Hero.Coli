using UnityEngine;

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
    private Camera _uiCam;
    private int _originCullingMask;

    void OnEnable()
    {
        _blackBar = GameObject.Find("CinematicBlackBar").GetComponent<CutSceneBlackBarHandler>();
        _uiCam = GameObject.FindGameObjectWithTag("CameraInterface").GetComponent<Camera>();
        _originCullingMask = _uiCam.cullingMask;
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
        _uiCam.cullingMask = LayerMask.NameToLayer("Nothing");
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
        _uiCam.cullingMask = _originCullingMask;
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

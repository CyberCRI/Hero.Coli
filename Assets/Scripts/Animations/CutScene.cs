using UnityEngine;
using System.Collections;

public abstract class CutScene : MonoBehaviour {

    private CellControl __cellControl;
    private CellControl _cellControl
    {
        get {
            __cellControl = (null == __cellControl)?GameObject.FindGameObjectWithTag("Player").GetComponent<CellControl>():__cellControl;
            return __cellControl;
        }
    }  
    
	// must be implemented in each cut scene
    public abstract void startCutScene ();    
    
    // must be called when starting a cut scene
	public virtual void start () {
        _cellControl.freezePlayer(true);
		FocusMaskManager.get().blockClicks(true);
        startCutScene ();
	}
	
    // must be implemented in each cut scene
    public abstract void endCutScene ();
    
    // must be called when ending a cut scene
    public virtual void end () {
		FocusMaskManager.get().blockClicks(false);
        _cellControl.freezePlayer(false);
        endCutScene ();
	}
}

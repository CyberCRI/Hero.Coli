using UnityEngine;
using System.Collections;

public class GenericBoxColliderMatcher : BoxColliderMatcher {

    public enum CenteringMode {
        BGCENTER,
        BGLOCALCENTER,
        GOCENTER,
        GOLOCALCENTER,
        BGCOMPUTED,
        GOCOMPUTED,
        BOXCOMPUTED
    }

    public CenteringMode centeringMode = CenteringMode.BGLOCALCENTER;        
    	
    protected override void specificUpdate () {
        switch(centeringMode) {
            case CenteringMode.BGCENTER:
                if(null != bg) {
                    boxCollider.center = bg.transform.position;
                } else {
                    centeringMode = CenteringMode.BOXCOMPUTED;
                }
                break;
            case  CenteringMode.BGLOCALCENTER:
                if(null != bg) {
                    bgLocalCenterSpecificUpdate ();
                } else {
                    centeringMode = CenteringMode.BOXCOMPUTED;
                }
                break;
            case CenteringMode.GOCENTER:
                boxCollider.center = gameObject.transform.position;
                break;
            case  CenteringMode.GOLOCALCENTER:
                goLocalCenterSpecificUpdate ();
                break;
            case CenteringMode.BGCOMPUTED:
                if(null != bg) {
                    boxCollider.center = new Vector3(
                        bg.transform.localScale.x*factor+offset+Mathf.Sign(offset)*label.transform.localScale.x, 
                        boxCollider.center.y,
                        boxCollider.center.z);
                } else {
                    centeringMode = CenteringMode.BOXCOMPUTED;
                }
                break;
            case CenteringMode.GOCOMPUTED:
                boxCollider.center = new Vector3(
                    gameObject.transform.localScale.x*factor+offset+Mathf.Sign(offset)*label.transform.localScale.x, 
                    boxCollider.center.y,
                    boxCollider.center.z);
                break;
            case CenteringMode.BOXCOMPUTED:
                boxColliderComputedSpecificUpdate ();
                break;
            default:
                Debug.LogError("autoswitch to BOXCOMPUTED");
                centeringMode = CenteringMode.BOXCOMPUTED;
                break;
        }
    }
}

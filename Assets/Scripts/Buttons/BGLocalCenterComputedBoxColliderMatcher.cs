using UnityEngine;
using System.Collections;

public class BGLocalCenterComputedBoxColliderMatcher : BoxColliderMatcher {
    protected override void specificUpdate()
    {
        bgLocalCenterSpecificUpdate ();
    }
}

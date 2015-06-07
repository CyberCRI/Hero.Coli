using UnityEngine;
using System.Collections;

public class GOLocalCenterComputedBoxColliderMatcher : BoxColliderMatcher {
    protected override void specificUpdate()
    {
        goLocalCenterSpecificUpdate();
    }
}

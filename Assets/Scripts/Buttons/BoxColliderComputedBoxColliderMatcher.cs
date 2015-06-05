using UnityEngine;
using System.Collections;

public class BoxColliderComputedBoxColliderMatcher : BoxColliderMatcher {
    protected override void specificUpdate () {
        boxColliderComputedSpecificUpdate ();
    }
}

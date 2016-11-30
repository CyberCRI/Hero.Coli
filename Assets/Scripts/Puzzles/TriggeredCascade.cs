using System.Collections.Generic;
using UnityEngine;

public class TriggeredCascade : TriggeredBehaviour
{
    [SerializeField]
    private List<TriggeredBehaviour> toTrigger;

    public override void triggerStart()
    {
        foreach (TriggeredBehaviour tb in toTrigger)
        {
            // Debug.Log(this.GetType() + " triggerStart " + tb.name);
            tb.triggerStart();
        }
    }

    public override void triggerExit()
    {
        foreach (TriggeredBehaviour tb in toTrigger)
        {
            // Debug.Log(this.GetType() + " triggerExit " + tb.name);
            tb.triggerExit();
        }
    }

    public override void triggerStay()
    {
        foreach (TriggeredBehaviour tb in toTrigger)
        {
            // Debug.Log(this.GetType() + " triggerStay " + tb.name);
            tb.triggerStay();
        }
    }
}

using UnityEngine;
using System.Collections;

public abstract class PickableBioBrick : PickableItem
{
    protected override void addTo ()
    {
        BioBrick biobrick = _dnaBit as BioBrick;
        if (null == biobrick) {
            biobrick = produceDNABit () as BioBrick;
        }
        if (null == biobrick) {
            Debug.LogWarning(this.GetType() + " addTo() - failed to produce non-null dna bit");
        } else {
            // Debug.Log(this.GetType() + " addTo " + _dnaBit);
            AvailableBioBricksManager.get ().addAvailableBioBrick (biobrick, false);
        }
    }
}

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
            Logger.Log ("PickableBioBrick::addTo() - failed to produce non-null dna bit", Logger.Level.WARN);
        } else {
            Logger.Log ("PickableBioBrick::addTo " + _dnaBit, Logger.Level.INFO);
            AvailableBioBricksManager.get ().addAvailableBioBrick (biobrick, false);
        }
    }
}

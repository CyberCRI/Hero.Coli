using UnityEngine;

public class PickablePlasmid : MonoBehaviour {
    private static bool _alreadyPicked = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            if (!_alreadyPicked)
            {
                ModalManager.setModal("T1_PLASMID");
            }
            _alreadyPicked = true;
            RedMetricsManager.get ().sendRichEvent(TrackingEvent.PICKUP, new CustomData(CustomDataTag.PLASMID, gameObject.name));
            CraftZoneManager.get().addSlot();
            Destroy(this.gameObject);
        }
    }

    public static void reset()
    {
        _alreadyPicked = false;
    }
}
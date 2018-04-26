#define TUTORIAL2

using UnityEngine;

public class PickablePlasmid : MonoBehaviour {
    [SerializeField]
    private bool _playTutorial = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            string modalCode = _playTutorial?"T1_PLASMID":"T2_PLASMID";
            ModalManager.setModal(modalCode);
#if TUTORIAL2
            RedMetricsManager.get().sendEvent(TrackingEvent.HINT, new CustomData(CustomDataTag.MESSAGE, modalCode));
#endif
            RedMetricsManager.get ().sendRichEvent(TrackingEvent.PICKUP, new CustomData(CustomDataTag.PLASMID, gameObject.name));
            CraftZoneManager.get().addSlot();
            Destroy(this.gameObject);
        }
    }
}
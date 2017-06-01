using UnityEngine;

public class PickablePlasmid : MonoBehaviour {
    [SerializeField]
    private bool _playTutorial = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            if (_playTutorial)
            {
                ModalManager.setModal("T1_PLASMID");
            }
            RedMetricsManager.get ().sendRichEvent(TrackingEvent.PICKUP, new CustomData(CustomDataTag.PLASMID, gameObject.name));
            CraftZoneManager.get().addSlot();
            Destroy(this.gameObject);
        }
    }
}
using UnityEngine;

public class PushableBoxFlagellaRequirementHint : MonoBehaviour
{
    [SerializeField]
    private int requiredFlagellaCount;
    [SerializeField]
    private string _modalWindowCode;

    void OnCollisionEnter(Collision col)
    {
        if ((null != col.collider) && (col.collider.tag == Character.playerTag))
        {
            if(!CellControl.get(this.GetType().ToString()).hasAtLeastFlagellaCount(requiredFlagellaCount))
            {
                ModalManager.setModal(_modalWindowCode);
                RedMetricsManager.get().sendEvent(TrackingEvent.HINT, new CustomData(CustomDataTag.MESSAGE, _modalWindowCode));
            }
        }
    }
}

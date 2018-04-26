using UnityEngine;

public class PushableBoxFlagellaRequirementHint : MonoBehaviour
{
    [SerializeField]
    private int requiredFlagellaCount;
    [SerializeField]
    private string _modalWindowCode;
    [SerializeField]
    private float reminderTimeDelta = 5.0f;
    private float reminderTime = 0.0f;

    void OnCollisionEnter(Collision col)
    {
        if ((null != col.collider) && (col.collider.tag == Character.playerTag))
        {
            if(!CellControl.get(this.GetType().ToString()).hasAtLeastFlagellaCount(requiredFlagellaCount))
            {
                if(Time.time - reminderTime > reminderTimeDelta)
                {
                    reminderTime = Time.time;
                    ModalManager.setModal(_modalWindowCode);
                    RedMetricsManager.get().sendEvent(TrackingEvent.HINT, new CustomData(CustomDataTag.MESSAGE, _modalWindowCode));
                }
            }
        }
    }
}

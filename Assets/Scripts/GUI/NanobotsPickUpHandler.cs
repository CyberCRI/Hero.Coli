using UnityEngine;

public class NanobotsPickUpHandler : MonoBehaviour {

    private static NanobotsCounter _nanoCounter;
    private static int _pickedUpNumber = 0;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag)
        {
            RedMetricsManager.get ().sendEvent(TrackingEvent.PICKUP, new CustomData(CustomDataTag.NANOBOT, transform.parent.transform.gameObject.name));
            _pickedUpNumber += 1;
            if(_pickedUpNumber == 1)
            {
                ModalManager.setModal("T1_NANOBOT");
            }
            _nanoCounter = (null==_nanoCounter)?GameObject.Find("NanobotsIndicator").GetComponent<NanobotsCounter>():_nanoCounter;
            _nanoCounter.updateLabel(_pickedUpNumber);
            Destroy(this.gameObject);
        }
    }

    public static void clear()
    {
        _nanoCounter = null;
        _pickedUpNumber = 0;
    }

    /*void OnDestroy()
    {
        _nanoCounter = null;
        _pickedUpNumber = 0;
    }*/
}

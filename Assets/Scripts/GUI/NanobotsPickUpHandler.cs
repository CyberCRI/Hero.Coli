using UnityEngine;

public class NanobotsPickUpHandler : MonoBehaviour
{
    public static string nanobotTag = "NanoBot";
    private static NanobotsCounter _nanoCounter;
    private static int _pickedUpNumber = 0;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            _pickedUpNumber++;
            CustomData data = new CustomData(CustomDataTag.NANOBOT, transform.parent.transform.gameObject.name);
            data.Add(CustomDataTag.COUNT, _pickedUpNumber.ToString());
            RedMetricsManager.get().sendEvent(TrackingEvent.PICKUP, data);
            if (_pickedUpNumber == 1)
            {
				ModalManager.setModal("T1_NANOBOT", true, false);
            }
            _nanoCounter = (null == _nanoCounter) ? GameObject.Find("NanobotsIndicator").GetComponent<NanobotsCounter>() : _nanoCounter;
            _nanoCounter.updateLabel(_pickedUpNumber);
            Destroy(this.gameObject);
        }
    }

    public static void reset()
    {
        _nanoCounter = null;
        NanobotsCounter.reset();
        _pickedUpNumber = 0;
    }

    /*void OnDestroy()
    {
        _nanoCounter = null;
        _pickedUpNumber = 0;
    }*/
}

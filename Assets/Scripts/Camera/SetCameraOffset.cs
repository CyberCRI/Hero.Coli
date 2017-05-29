using UnityEngine;

public class SetCameraOffset : MonoBehaviour
{
    [SerializeField]
    private float _offSetValue;
    [SerializeField]
    private float _timeToSetCam;
    [SerializeField]
    private bool _resetCam = false;
    private float _originOffsetY;

    // Use this for initialization
    void Start()
    {
        _originOffsetY = BoundCamera.instance.offset.y;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            if (_resetCam)
            {
                // Debug.Log(this.GetType() + " " + name + " resetting camera offset");
                BoundCamera.instance.ZoomInOut(_originOffsetY, _timeToSetCam);
            }
            else
            {
                // Debug.Log(this.GetType() + " " + name + " setting camera with offset="+_offSetValue);
                BoundCamera.instance.ZoomInOut(_offSetValue, _timeToSetCam);
            }
        }
    }
}

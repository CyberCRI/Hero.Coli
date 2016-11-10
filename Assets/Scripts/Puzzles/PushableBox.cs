using UnityEngine;

[System.Serializable]
public class PushableBox : MonoBehaviour
{
    [SerializeField]
    private float minSpeed;
    private Vector3 _initPos;
    private CellControl _control = null;
    private RigidbodyConstraints _noPush = RigidbodyConstraints.FreezeAll;
    private RigidbodyConstraints _canPush = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

    void Start()
    {
        _initPos = transform.position;
    }

    private CellControl lazySafeGetCellControl(Collision col)
    {
        if (null == _control)
        {
            _control = col.gameObject.GetComponent<CellControl>();
        }
        return _control;
    }

    void OnCollisionEnter(Collision col)
    {
        if (null != col.collider)
        {
            lazySafeGetCellControl(col);
            if (_control && _control.currentMoveSpeed >= minSpeed)
            {
                GetComponent<Rigidbody>().constraints = _canPush;
            }
            else if (_control && _control.currentMoveSpeed < minSpeed)
            {
                GetComponent<Rigidbody>().constraints = _noPush;
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (null != col.collider)
        {
            lazySafeGetCellControl(col);
            if (_control)
                GetComponent<Rigidbody>().constraints = _noPush;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (GetComponent<Rigidbody>().constraints != _canPush)
        {
            if (null != col.collider)
            {
                lazySafeGetCellControl(col);
                if (_control && _control.currentMoveSpeed >= minSpeed)
                {
                    GetComponent<Rigidbody>().constraints = _canPush;
                }
                else if (_control && _control.currentMoveSpeed < minSpeed)
                {
                    GetComponent<Rigidbody>().constraints = _noPush;
                }
            }
        }
        if (col.collider.tag == "Door")
        {
            GetComponent<Rigidbody>().constraints = _canPush;
        }
    }

    public void resetPos()
    {
        transform.position = _initPos;
    }

}

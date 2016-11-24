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
    [SerializeField]
    private Rigidbody _rigidBody;

    void Start()
    {
        _initPos = transform.position;
        _control = CellControl.get("PushableBox");
    }

    void OnCollisionEnter(Collision col)
    {
        processCollision(col);
    }

    void processCollision(Collision col)
    {
        if (_rigidBody.constraints != _canPush)
        {
            if (null != col.collider)
            {
                if (col.collider.tag == Hero.playerTag)
                {
                    if (_control.currentMoveSpeed >= minSpeed)
                    {
                        _rigidBody.constraints = _canPush;
                    }
                    else if (_control.currentMoveSpeed < minSpeed)
                    {
                        _rigidBody.constraints = _noPush;
                    }
                }
                else if (col.collider.tag == "Door")
                {
                    _rigidBody.constraints = _canPush;
                }
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        _rigidBody.constraints = _noPush;
    }

    void OnCollisionStay(Collision col)
    {
        processCollision(col);
    }

    public void resetPos()
    {
        transform.position = _initPos;
    }

}

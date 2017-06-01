using UnityEngine;

[System.Serializable]
public class PushableBox : MonoBehaviour
{
	public delegate void PushEvent(bool canPush);
	public static event PushEvent onPlayerPushRock;
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
        _control = CellControl.get(this.GetType().ToString());
        _rigidBody = _rigidBody == null ? this.GetComponent<Rigidbody>() : _rigidBody;
    }

    void OnCollisionEnter(Collision col)
    {
        processCollision(col);
    }

    void processCollision(Collision col)
    {
        if (null != _rigidBody && _rigidBody.constraints != _canPush)
        {
            if (null != col.collider)
            {
                if (col.collider.tag == Character.playerTag)
                {
                    if (_control.currentMoveSpeed >= minSpeed)
					{
						onPlayerPushRock (true);
                        _rigidBody.constraints = _canPush;
                    }
                    else if (_control.currentMoveSpeed < minSpeed)
                    {
						onPlayerPushRock (false);
                        _rigidBody.constraints = _noPush;
                    }
                }
                else if (col.collider.tag == CutSceneElements.doorTag)
                {
                    _rigidBody.constraints = _canPush;
                }
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (null != _rigidBody)
        {
            _rigidBody.constraints = _noPush;
        }
    }

    void OnCollisionStay(Collision col)
    {
        processCollision(col);
    }

    public void resetPosition()
    {
        transform.position = _initPos;
    }

}

using UnityEngine;
using System.Collections;

public class CellControl : MonoBehaviour
{


    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = Character.gameObjectName;
    private static CellControl _instance;
    public static CellControl get(string origin)
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find(gameObjectName);
            if (null != go)
            {
                _instance = go.GetComponent<CellControl>();
                if (null != _instance)
                {
                    _instance.initializeIfNecessary();
                }
                else
                {
                    Debug.LogError("Component CellControl of GameObject " + gameObjectName + " not found");
                }
            }
            else
            {
                Debug.LogWarning("GameObject " + gameObjectName + " not found by " + origin);
            }
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if((_instance != null) && (_instance != this))
        {            
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        
        _targetPosition = transform.position;
    }

    public void initialize ()
    {
        if (isAbsoluteWASD)
        {
            switchControlTypeToAbsoluteWASD();
        }
        else
        {
            switchControlTypeToRelativeWASD();
        }
        if (isLeftClickToMove)
        {
            switchControlTypeToLeftClickToMove();
        }
        else
        {
            switchControlTypeToRightClickToMove();
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private bool _initialized = false;

	// Events
	public delegate void MovingAction(bool isMoving);
	public static event MovingAction onCellMove;

    // public float baseMoveSpeed;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float relativeRotationSpeed;
    [SerializeField]
    private Character character;
    [SerializeField]
    private float moveEnergyCost;
    public float currentMoveSpeed;
    [SerializeField]
    private string wallname;

    public AbsoluteWASDButton absoluteWASDButton;
    public LeftClickToMoveButton leftClickToMoveButton;
    public RelativeWASDButton relativeWASDButton;
    public RightClickToMoveButton rightClickToMoveButton;
    public UISprite selectedMouseControlTypeSprite;
    public UISprite selectedKeyboardControlTypeSprite;
    public ParticleSystemFeedback clickFeedback;

    private bool _pause;
    private Vector3 _inputMovement;
    private SwimAnimator _swimAnimator;
    private bool _isPlayerFrozen = false;

    /* 
    * Click to move variables
    */
    private float _hitdist = 0.0f;
    private Vector3 _targetPosition;
    private bool _isFirstUpdate = false;
    private bool _isClickBlockedAfterPause = false;

    public enum ControlType
    {
        RightClickToMove = 0,
        LeftClickToMove = 1,
        AbsoluteWASD = 2,
        RelativeWASD = 3
    };

    private bool _isLeftClickToMove;
    private bool isLeftClickToMove
    {
        get
        {
            return _isLeftClickToMove;
        }
        set
        {
            _isLeftClickToMove = value;
            MemoryManager.get().configuration.isLeftClickToMove = value;
        }
    }
    private bool _isAbsoluteWASD;
    private bool isAbsoluteWASD
    {
        get
        {
            return _isAbsoluteWASD;
        }
        set
        {
            _isAbsoluteWASD = value;
            MemoryManager.get().configuration.isAbsoluteWASD = value;
        }
    }

    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            _isAbsoluteWASD = MemoryManager.get().configuration.isAbsoluteWASD;
            _isLeftClickToMove = MemoryManager.get().configuration.isLeftClickToMove;
            reset();
            _initialized = false;
        }
    }

    public void Pause(bool pause)
    {
        if (_pause && !pause)
        {
            StopAllCoroutines();
            StartCoroutine(blockClicksAfterPause());
        }
        _pause = pause;
    }

    public bool isPaused()
    {
        return _pause;
    }

    private void setClickFeedback(Vector3 position)
    {
        clickFeedback.burst(position);
    }

    private void clickToMoveUpdate(KeyCode mouseButtonCode)
    {
        if (Input.GetKeyDown(mouseButtonCode) || Input.GetKey(mouseButtonCode))
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);

            // TODO FIXME
            if (null != Camera.main)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (playerPlane.Raycast(ray, out _hitdist) && !UICamera.hoveredObject)
                {
                    _targetPosition = ray.GetPoint(_hitdist);
                    setClickFeedback(_targetPosition);
                }
            }
        }

        if (Vector3.zero == _inputMovement)
        {
            Vector3 aim = _targetPosition - transform.position;
            _inputMovement = new Vector3(aim.x, 0, aim.z);

            if (_inputMovement.sqrMagnitude <= 5f)
            {
                stopMovement();
            }
            else
            {
                _inputMovement = _inputMovement.normalized;
            }
            rotationUpdate();
        }
    }

    private void AbsoluteWASDUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {

            cancelMouseMove();

            //Translate
            _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (_inputMovement.sqrMagnitude > 1)
            {
                _inputMovement /= Mathf.Sqrt(2);
            }

            rotationUpdate();

        }
        else if (Vector3.zero != _inputMovement)
        {
            stopMovement();
        }
    }

    private void RelativeWASDUpdate()
    {

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            cancelMouseMove();

            float norm = Input.GetAxis("Vertical");

            if (norm < 0) norm = 0;

            float deltaAngle = Input.GetAxis("Horizontal");

            transform.RotateAround(transform.position, Vector3.up, deltaAngle * relativeRotationSpeed);

            float angle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            _inputMovement = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * norm;
        }
    }

    public void reset()
    {
        stopMovement();
    }

    private void cancelMouseMove()
    {
        _targetPosition = transform.position;
    }

    public void stopMovement()
    {
        _inputMovement = Vector3.zero;
        cancelMouseMove();
		setSpeed ();
		onCellMove (false);
    }

    public void freezePlayer(bool value)
    {
        stopMovement();
        _isPlayerFrozen = value;
    }

    private void rotationUpdate()
    {
        if (Vector3.zero != _inputMovement)
        {
            //Rotation
            float rotation = Mathf.Atan2(_inputMovement.x, _inputMovement.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * rotationSpeed);
        }
    }

    private void commonUpdate()
    {
        if (Vector3.zero != _inputMovement)
        {
			onCellMove (true);
            Vector3 moveAmount = _inputMovement * currentMoveSpeed * Time.deltaTime;

            this.GetComponent<Collider>().attachedRigidbody.AddForce(moveAmount);

            updateEnergy(moveAmount);

            setSpeed();
        }
    }

    private void setSpeed()
    {
        //SetSpeed
        float speed = _inputMovement.sqrMagnitude + 0.3f;
        //TODO put variable for this
        if (null == _swimAnimator)
        {
            _swimAnimator = (SwimAnimator)gameObject.GetComponent<SwimAnimator>();
        }
        _swimAnimator.setSpeed(speed);
    }

    private void updateEnergy(Vector3 moveAmount)
    {

        //float cost = moveAmount.sqrMagnitude*moveEnergyCost;

        //TODO remove this temporary modification
        //TODO use Time.deltaTime inside "react" methods
        float cost = 0;

        character.subEnergy(cost);
    }

    IEnumerator blockClicksAfterPause()
    {
        _isClickBlockedAfterPause = true;
        yield return new WaitForSeconds(0.1f);
        _isClickBlockedAfterPause = false;
        yield return null;
    }

    void Update()
    {
        if (!_pause)
        {
            _inputMovement = Vector3.zero;
            //Keyboard controls
            if (!_isPlayerFrozen)
            {
                if (isAbsoluteWASD)
                {
                    AbsoluteWASDUpdate();
                }
                else
                {
                    RelativeWASDUpdate();
                }

                //Mouse controls
                if (!_isFirstUpdate && !_isClickBlockedAfterPause)
                {
                    if (isLeftClickToMove)
                    {
                        clickToMoveUpdate(KeyCode.Mouse0);
                    }
                    else
                    {
                        clickToMoveUpdate(KeyCode.Mouse1);
                    }
                }
                else
                {
                    _isFirstUpdate = false;
                }
                commonUpdate();
            }
        }
    }

    public void teleport(Vector3 position)
    {
        transform.position = position;
        stopMovement();
    }

    public void teleport(Vector3 position, Quaternion rotation)
    {
        teleport(position);
        transform.rotation = rotation;
    }

    private void switchControlTypeTo(ControlType newControlType)
    {
        switch (newControlType)
        {
            case ControlType.AbsoluteWASD:
                switchControlTypeToAbsoluteWASD();
                break;
            case ControlType.LeftClickToMove:
                switchControlTypeToLeftClickToMove();
                break;
            case ControlType.RelativeWASD:
                switchControlTypeToRelativeWASD();
                break;
            case ControlType.RightClickToMove:
                switchControlTypeToRightClickToMove();
                break;
        }
    }

    public void switchControlTypeToRightClickToMove()
    {
        switchControlTypeTo(ControlType.RightClickToMove, rightClickToMoveButton.transform.position);
    }

    public void switchControlTypeToLeftClickToMove()
    {
        switchControlTypeTo(ControlType.LeftClickToMove, leftClickToMoveButton.transform.position);
    }

    public void switchControlTypeToAbsoluteWASD()
    {
        switchControlTypeTo(ControlType.AbsoluteWASD, absoluteWASDButton.transform.position);
    }

    public void switchControlTypeToRelativeWASD()
    {
        switchControlTypeTo(ControlType.RelativeWASD, relativeWASDButton.transform.position);
    }

    private void switchControlTypeTo(ControlType newControlType, Vector3 position)
    {

        // Debug.Log(this.GetType() + " switchControlTypeTo(" + newControlType + ") with old isLeftClickToMove=" + isLeftClickToMove + " & isAbsoluteWASD=" + isAbsoluteWASD);

        if (ControlType.LeftClickToMove == newControlType)
        {
            _isFirstUpdate = true;
        }

        switch (newControlType)
        {
            case ControlType.AbsoluteWASD:
                isAbsoluteWASD = true;
                selectedKeyboardControlTypeSprite.transform.position = absoluteWASDButton.transform.position;
                break;
            case ControlType.RelativeWASD:
                isAbsoluteWASD = false;
                selectedKeyboardControlTypeSprite.transform.position = relativeWASDButton.transform.position;
                break;
            case ControlType.LeftClickToMove:
                isLeftClickToMove = true;
                selectedMouseControlTypeSprite.transform.position = leftClickToMoveButton.transform.position;
                break;
            case ControlType.RightClickToMove:
                isLeftClickToMove = false;
                selectedMouseControlTypeSprite.transform.position = rightClickToMoveButton.transform.position;
                break;
            default:
                break;

        }

        _targetPosition = transform.position;

        // Debug.Log(this.GetType() + " switchControlTypeTo(" + newControlType + ") with new isLeftClickToMove=" + isLeftClickToMove + " & isAbsoluteWASD=" + isAbsoluteWASD);
    }

    public void refreshControlType()
    {
        // Debug.Log(this.GetType() + " refreshControlType before refreshControlType isLeftClickToMove=" + isLeftClickToMove + " & isAbsoluteWASD=" + isAbsoluteWASD);
        if (isLeftClickToMove)
        {
            switchControlTypeToLeftClickToMove();
        }
        else
        {
            switchControlTypeToRightClickToMove();
        }
        if (isAbsoluteWASD)
        {
            switchControlTypeToAbsoluteWASD();
        }
        else
        {
            switchControlTypeToRelativeWASD();
        }
        // Debug.Log(this.GetType() + " refreshControlType after refreshControlType isLeftClickToMove=" + isLeftClickToMove + " & isAbsoluteWASD=" + isAbsoluteWASD);
    }

    void OnCollisionStay(Collision col)
    {
        if ((Vector3.zero != _inputMovement) && col.collider && (wallname == col.collider.name))
        {
            stopMovement();
        }
    }

    public bool hasAtLeastFlagellaCount(int count)
    {
        return count <= character.flagellaCount;
    }
}
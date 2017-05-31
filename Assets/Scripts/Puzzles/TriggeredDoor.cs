using UnityEngine;
using System.Collections;

public class TriggeredDoor : TriggeredBehaviour
{
	public delegate void DoorEvent(bool isDoorOpening);
	public static event DoorEvent onDoorToggle;
    [SerializeField]
    private Transform moveTo;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float closeDelay = 0f;
    [SerializeField]
    private float minDistance = 0.1f;
    [SerializeField]
    private bool _preventReset = false;

    // TODO switch/channeling mode
    // // If true, switch mode: the door opens completely however short the entering collision was.
    // // Otherwise, channeling mode: the player has to wait in the collider for the door to completely open.
    // [SerializeField]
    // private bool _isSwitchMode = true;

    private Vector3 _origin;
    private bool _isDoorOpening = false;
    private bool _isDoorClosing = false;
    private bool _isBeingTriggered = false;
    private const string openDoorITweenName = "openDoor";
    private const string closeDoorITweenName = "closeDoor";
    private IEnumerator waitCoroutine;

    void Awake()
    {
        _origin = transform.position;
    }

    public void resetPosition()
    {
        // Debug.Log(this.GetType() + " resetPosition");
        if (!_preventReset)
        {
            stopMovement();
            transform.position = _origin;
        }
    }

    Hashtable generateOpenHash()
    {
        return iTween.Hash(
                "position", moveTo,
                "speed", speed,
                "easetype", iTween.EaseType.easeInOutQuad,
                "oncomplete", "onDoorOpened",
                "name", openDoorITweenName
            );
    }

    Hashtable generateCloseHash()
    {
        return iTween.Hash(
                "position", _origin,
                "speed", speed,
                "easetype", iTween.EaseType.easeInOutQuad,
                "oncomplete", "onDoorClosed",
                "name", closeDoorITweenName);
    }

    void openDoor()
    {
        // Debug.Log(this.GetType() + " openDoor");
        _isDoorOpening = true;
        _isDoorClosing = false;
        iTween.MoveTo(gameObject, generateOpenHash());
    }

    void onDoorOpened()
    {
        // Debug.Log(this.GetType() + " onDoorOpened");
        _isDoorOpening = false;
        // delay time counts as closing time
        _isDoorClosing = !_isBeingTriggered;
        if (_isDoorClosing)
        {
            if (null != waitCoroutine)
            {
                // Debug.Log(this.GetType() + " onDoorOpened stops waitCoroutine");
                StopCoroutine(waitCoroutine);
            }
            waitCoroutine = waitAndCloseDoor();
            StartCoroutine(waitCoroutine);
        }
    }

    IEnumerator waitAndCloseDoor()
    {
        // Debug.Log(this.GetType() + " waitAndCloseDoor");
        _isDoorOpening = false;
        // delay time counts as closing time
        _isDoorClosing = true;
        yield return new WaitForSeconds(closeDelay);
        closeDoor();
        yield return null;
    }

    private void closeDoor()
    {
        // Debug.Log(this.GetType() + " closeDoor");
        _isDoorOpening = false;
        _isDoorClosing = true;
        iTween.MoveTo(gameObject, generateCloseHash());
    }

    void onDoorClosed()
    {
        // Debug.Log(this.GetType() + " onDoorClosed");
        _isDoorOpening = false;
        _isDoorClosing = false;
    }

    void stopMovement(bool closing = true, bool opening = true)
    {
        // halt the potential coroutine
        if (null != waitCoroutine)
        {
            // Debug.Log(this.GetType() + " triggerStart stops waitCoroutine");
            StopCoroutine(waitCoroutine);
        }
        // halt the closing
        // Debug.Log(this.GetType() + " triggerStart searches iTween " + closeDoorITweenName);
        iTween[] itweens = gameObject.GetComponents(typeof(iTween)) as iTween[];
        if (null != itweens)
        {
            foreach (iTween it in itweens)
            {
                if ((closing && closeDoorITweenName == it.name)
                || (opening && openDoorITweenName == it.name))
                {
                    Destroy(it);
                    // Debug.Log(this.GetType() + " triggerStart stops iTween " + closeDoorITweenName);
                    break;
                }
            }
        }
        _isDoorOpening = false;
        _isDoorClosing = false;
    }

    public override void triggerStart()
    {
        // Debug.Log(this.GetType() + " triggerStart");

        _isBeingTriggered = true;
        if (_isDoorClosing)
        {
            // halt the closing
            stopMovement(true, false);
            openDoor();
        }
        else if (!_isDoorOpening)
        {
            openDoor();
        }
    }

    public override void triggerStay()
    {
    }

    public override void triggerExit()
    {
        // Debug.Log(this.GetType() + " triggerExit");

        _isBeingTriggered = false;

        if (!_isDoorOpening && !_isDoorClosing)
        {
            if (null != waitCoroutine)
            {
                // Debug.Log(this.GetType() + " triggerExit stops waitCoroutine");
                StopCoroutine(waitCoroutine);
            }
            waitCoroutine = waitAndCloseDoor();
            StartCoroutine(waitCoroutine);
        }
    }
}

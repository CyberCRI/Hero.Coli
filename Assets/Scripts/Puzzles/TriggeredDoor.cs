using UnityEngine;

public class TriggeredDoor : TriggeredBehaviour
{
    [SerializeField]
    private Transform moveTo;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float closeDelay = 0f;
    [SerializeField]
    private float minDistance = 0.1f;
    private bool _isTriggered = false;
    private Vector3 _origin;

    void Start()
    {
        _origin = transform.position;
    }

    void Update()
    {
        if (_isTriggered && transform.position == _origin)
        {
            _isTriggered = false;
        }
    }

    private void trigger(string _name)
    {
        if (!_isTriggered && Vector3.Distance(transform.position, moveTo.position) > minDistance)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "position", moveTo,
                "speed", speed,
                "easetype", iTween.EaseType.easeInOutQuad,
                "name", _name
            ));
            _isTriggered = true;
        }
    }

    public override void triggerStart()
    {
        trigger("triggerStart");
    }

    public override void triggerStay()
    {
        // trigger("triggerStay");
    }

    public override void triggerExit()
    {
        iTween.MoveTo(gameObject, iTween.Hash(
        "position", _origin,
        "speed", speed,
        "easetype", iTween.EaseType.easeInOutQuad,
        "delay", closeDelay,
        "name", "triggerExit"));
        _isTriggered = false;
    }
}

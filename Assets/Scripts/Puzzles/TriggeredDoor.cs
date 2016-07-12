using UnityEngine;

public class TriggeredDoor : TriggeredBehaviour {

	public Transform moveTo;
	public float speed = 10f;
	public float closeDelay = 0f;
    private bool _isTriggered = false;
	
	private Vector3 origin;
	
	void Start(){
		origin = transform.position;
	}
	
    void Update()
    {
        if (transform.position == origin && _isTriggered == true)
        {
            _isTriggered = false;
        }
    }

	public override void triggerStart(){
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", moveTo,
			"speed", speed,
			"easetype", iTween.EaseType.easeInOutQuad
		));
        _isTriggered = true;
	}
	
	public override void triggerExit(){
        if (transform.position != moveTo.position)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
            "position", origin,
            "speed", speed,
            "easetype", iTween.EaseType.easeInOutQuad,
            "delay", closeDelay
        ));
            _isTriggered = false;
        }
        else
        {
            iTween.MoveTo(gameObject, iTween.Hash(
            "position", origin,
            "speed", speed,
            "easetype", iTween.EaseType.easeInOutQuad,
            "delay", 0
        ));
        }
		
	}
	
	public override void triggerStay(){
        iTween.MoveTo(gameObject, iTween.Hash(
            "position", moveTo,
            "speed", speed,
            "easetype", iTween.EaseType.easeInOutQuad
        ));
        _isTriggered = true;
    }
}

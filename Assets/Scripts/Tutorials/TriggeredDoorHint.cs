using UnityEngine;

public class TriggeredDoorHint : TriggeredBehaviour
{
    bool _displayHint = true;

    void OnTriggerEnter(Collider col)
    {
        if (_displayHint && (col.tag == Character.playerTag))
        {
            Character.get().gameObject.AddComponent<DoorHint>();
        }
    }
	
	public override void triggerStart(){
		_displayHint = false;
	}
	
	public override void triggerExit(){}
	
	public override void triggerStay(){}
}

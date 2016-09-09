using UnityEngine;
using System.Collections;

public class RotationUpdate : MonoBehaviour {

    private Vector3 _inputMovement;
    private PlatformMvt _platformMvt;
    private float rotationSpeed = 6f;

	// Use this for initialization
	void Start () {
        _platformMvt = this.GetComponent<PlatformMvt>();
	}
	
	// Update is called once per frame
	void Update () {

        _inputMovement = _platformMvt.getCurrentDestination() - this.transform.position;
        rotationUpdate();
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
}

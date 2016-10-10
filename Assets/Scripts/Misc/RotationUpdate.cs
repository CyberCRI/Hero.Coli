using UnityEngine;
using System.Collections;

public class RotationUpdate : MonoBehaviour {

    private Vector3 _inputMovement;
    private PlatformMvt _platformMvt;
    private float rotationSpeed = 6f;
	private Vector3 _previousPosition;
    [SerializeField]
    private bool _isControlledExternally = false;
	// Use this for initialization
	void Start () {
		_previousPosition = this.transform.position;
        _platformMvt = this.GetComponent<PlatformMvt>();
	}
	
	// Update is called once per frame
	void Update () {

        /*if (_platformMvt != null) {
			_inputMovement = _platformMvt.getCurrentDestination () - this.transform.position;
		} else 
		{
			_inputMovement = this.transform.position - _previousPosition;
		}*/
        if (_isControlledExternally == false)
        {
            _inputMovement = this.transform.position - _previousPosition;
            _previousPosition = this.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            ObjectDirectedRotationUpdate(player);
        }

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

    public void ObjectDirectedRotationUpdate(GameObject obj)
    {
        _inputMovement = obj.transform.localPosition - this.transform.localPosition;
        rotationUpdate();
    }

    IEnumerator SmoothRotate(float time)
    {
        float originTime = time;
        while(time >=0)
        {
            time -= Time.deltaTime;
            rotationUpdate();
        }
        yield return null;
    }

    public void SetIsControlledExternally(bool  value)
    {
        _isControlledExternally = value;
    }
}

using UnityEngine;
using System.Collections;

public class BoundCamera : MonoBehaviour
{
	public static BoundCamera instance = null;
	Transform _target;
	public Transform target {
		get {
			if (_target == null) {
				var character = GameObject.FindGameObjectWithTag(Character.playerTag);
				if (character != null)
					_target = character.transform;
			}
			return _target;
		}
		set {
			_target = value;
		}
	}
	public bool useScenePosition = true;
	public Vector3 offset;
	
	public bool _transition = true;
	public bool _zoomed = false;
	public float zoomedCameraDistanceMin = 5;
	public float cameraDistanceMin = 20;
	public float cameraDistanceMax = 75;
      
	[SerializeField]
	private float scrollSpeed = 5;
      
	public float zoomSmooth = 3;
	public float fovZoomed = 10.0f;
	
	private float fovUnzoomed;
	private float fov;

	public void SetZoom (bool zoomIn)
	{
		_zoomed = zoomIn;
		if (zoomIn) {
			fov = fovZoomed;
		} else {
			fov = fovUnzoomed;
		}
	}
		
	private float _zoomingTime;
	private float _originZoomingTime;
	private Vector3 _originalOffset;
	private bool _isZooming;
	private Coroutine _currentCoroutine;

	void Awake()
	{
		if (instance != null && instance != this) {
			Destroy (gameObject);
			Destroy (this);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	void Start()
	{
		if (useScenePosition)
        {
			offset = new Vector3 (0, transform.position.y, 0);
			_originalOffset = offset;
		}
		fovUnzoomed = GetComponent<Camera> ().fieldOfView;
		fov = fovUnzoomed;
	}

	public void Reset()
	{
		if (_currentCoroutine != null)
			StopCoroutine (_currentCoroutine);
		offset = _originalOffset;
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		
		//if(_isZooming)
		transform.position = target.position + offset;
		
		if (_transition) {
			/*
			if(!_zoomed) {
				fov = Mathf.Clamp(fov - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, cameraDistanceMin, cameraDistanceMax);
			}
			*/
			GetComponent<Camera> ().fieldOfView = Mathf.Lerp (GetComponent<Camera> ().fieldOfView, fov, Time.unscaledDeltaTime * zoomSmooth);
		}
	}

	public void ZoomInOut (float YOffset, float zoomingTime)
	{
		if (_isZooming) {
			StopCoroutine (_currentCoroutine);
		}
		_zoomingTime = zoomingTime;
		_originZoomingTime = zoomingTime;
		var offSetDif = -(offset.y - YOffset);
		_currentCoroutine = StartCoroutine (Zoom (offSetDif, offset.y));
	}

	IEnumerator Zoom (float YOffset, float originalOffset)
	{
		_isZooming = true;
		while (_zoomingTime > 0) {
			_zoomingTime -= Time.deltaTime;
			offset.y = originalOffset + (YOffset * (1 - (_zoomingTime / _originZoomingTime)));
			yield return null;
		}
		_isZooming = false;
		yield return null;
	}
}

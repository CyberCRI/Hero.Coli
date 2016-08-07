using UnityEngine;

public class CullingMaskHandler : MonoBehaviour {

    private Camera _uiCamera;
    private int _originalCullingMask;

	void Start () {
        _uiCamera = this.GetComponent<Camera>();
        _originalCullingMask = _uiCamera.cullingMask;
	}

    public void hideInterface(bool value)
    {
        GUITransitioner.get().showGraphs(!value);
        if (value == true)
        {
            _uiCamera.cullingMask = 0;
        }
        else
        {
            _uiCamera.cullingMask = _originalCullingMask;
        }
    }
}

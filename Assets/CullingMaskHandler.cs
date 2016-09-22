using UnityEngine;

public class CullingMaskHandler : MonoBehaviour
{
    [SerializeField]
    private Camera _uiCamera;
    [SerializeField]
    private Camera _cutSceneCamera;
    private bool _isMenuMaskSet = false;
    private int _cullingMaskBeforeMainMenu;
    private int _originalCullingMask = int.MinValue;
    private int originalCullingMask
    {
        get
        {
            if(int.MinValue == _originalCullingMask)
            {
                _originalCullingMask = _uiCamera.cullingMask;
            }
            return _originalCullingMask;
        }
    }

    public void hideInterface(bool hide)
    {
        // Debug.LogError("hideInterface(" + hide + ")");
        GUITransitioner.showGraphs(!hide, GUITransitioner.GRAPH_HIDER.CUTSCENE);
        if (hide)
        {
            _uiCamera.cullingMask = 0;
            _cutSceneCamera.enabled = true;
        }
        else
        {
            _uiCamera.cullingMask = originalCullingMask;
            _cutSceneCamera.enabled = false;
        }
    }

    public void showMainMenu(bool show)
    {
        // Debug.LogError("showMainMenu(" + show + ")");
        if (show)
        {
            if(!_isMenuMaskSet)
            {
                _isMenuMaskSet = true;
                _cullingMaskBeforeMainMenu = _uiCamera.cullingMask;
                hideInterface(false);                
            }
        }
        else
        {
            _isMenuMaskSet = false;
            hideInterface(_cullingMaskBeforeMainMenu != originalCullingMask);
        }
    }
}

using UnityEngine;

public class CutSceneInstantiator : CutSceneElements
{
    [SerializeField]
    private GameObject _cutScenePrefab;
    [SerializeField]
    private GameObject _cutSceneInstance;
    [SerializeField]
    private Transform _originTransform;
    [SerializeField]
    private string _triggerTag;
    private bool _isResetCameraSaved = false;
    private Transform _resetCamera;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == _triggerTag)
        {
            if (null == _cutSceneInstance)
            {
                _cutSceneInstance = (GameObject)Instantiate(_cutScenePrefab, _originTransform.position, _originTransform.rotation);
            }
        }

        if (!_isResetCameraSaved)
        {
            _resetCamera = _boundCamera.transform;
            _isResetCameraSaved = true;
        }
        else
        {
            _boundCamera.transform.position = _resetCamera.position;
            _boundCamera.transform.rotation = _resetCamera.rotation;
            _boundCamera.target = _cellControl.gameObject.transform;
            _boundCamera.gameObject.SetActive(true);
            _cutSceneCameraUI.enabled = false;
            _blackBar.closeBar(false);
            _cellControl.freezePlayer(false);
        }
    }
}

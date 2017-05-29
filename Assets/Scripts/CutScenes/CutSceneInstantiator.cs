using UnityEngine;

public class CutSceneInstantiator : CutSceneElements
{
    [SerializeField]
    private GameObject _cutScenePrefab;
    private GameObject _instantiatedCutSceneGameObject;
    private CutScene _instantiatedCutSceneScript;
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
            instantiateIfNeeded();
        }

        if (!_isResetCameraSaved)
        {
            _resetCamera = BoundCamera.instance.transform;
            _isResetCameraSaved = true;
        }
        else
        {
            BoundCamera.instance.transform.position = _resetCamera.position;
            BoundCamera.instance.transform.rotation = _resetCamera.rotation;
            BoundCamera.instance.target = _cellControl.gameObject.transform;
            BoundCamera.instance.gameObject.SetActive(true);
            _cutSceneCameraUI.enabled = false;
            _blackBar.closeBar(false);
            _cellControl.freezePlayer(false);
        }
    }

    private void instantiateIfNeeded()
    {
        if ((null == _instantiatedCutSceneScript) || (null == _instantiatedCutSceneScript.gameObject || _instantiatedCutSceneScript.reinstantiateOnTrigger()))
        {
            if (null != _instantiatedCutSceneGameObject)
            {
                Destroy(_instantiatedCutSceneGameObject);
            }
            _instantiatedCutSceneGameObject = (GameObject)Instantiate(_cutScenePrefab, _originTransform.position, _originTransform.rotation);
            _instantiatedCutSceneScript = _instantiatedCutSceneGameObject.GetComponentInChildren<CutScene>();
        }
    }

    public void instantiateAndSetToEnd()
    {
        instantiateIfNeeded();

        if (null != _instantiatedCutSceneScript)
        {
            _instantiatedCutSceneScript.setToEnd();
        }
    }
}

using UnityEngine;

public class CutSceneEnder : CutSceneElements
{
    [SerializeField]
    private CutSceneInstantiator _cutSceneInstantiator;
    [SerializeField]
    private string _triggerTag;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == _triggerTag && null != _cutSceneInstantiator)
        {
            _cutSceneInstantiator.instantiateAndSetToEnd();
        }
    }
}
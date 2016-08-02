using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BioBricksCollapse : MonoBehaviour {

    [SerializeField]
    private Transform[] _bricksTransform;
    private float _distanceBetweenMiddleBricks;
    private float _distanceBetweenSideBricksL;
    private float _distanceBetweenSideBricksR;
    [SerializeField]
    private Texture _referenceTexture;
    private CraftDeviceSlot _craftDeviceSlot;

    // Use this for initialization
    void Start () {
        _distanceBetweenMiddleBricks = Vector3.Distance(_bricksTransform[1].localPosition, _bricksTransform[2].localPosition) - _referenceTexture.width;
        _distanceBetweenSideBricksL = Vector3.Distance(_bricksTransform[0].localPosition, _bricksTransform[1].localPosition) + (_distanceBetweenMiddleBricks / 2) - _referenceTexture.width;
        _distanceBetweenSideBricksR = Vector3.Distance(_bricksTransform[2].localPosition, _bricksTransform[3].localPosition) + (_distanceBetweenMiddleBricks / 2) - _referenceTexture.width;

        _craftDeviceSlot = this.gameObject.GetComponent<CraftDeviceSlot>();
    }

    public void Collapse()
    {
        _bricksTransform[1].localPosition = new Vector3(_bricksTransform[1].localPosition.x + (_distanceBetweenMiddleBricks / 2), _bricksTransform[1].localPosition.y, _bricksTransform[1].localPosition.z);
        _bricksTransform[2].localPosition = new Vector3(_bricksTransform[2].localPosition.x - (_distanceBetweenMiddleBricks / 2), _bricksTransform[2].localPosition.y, _bricksTransform[2].localPosition.z);

        _bricksTransform[0].localPosition = new Vector3(_bricksTransform[0].localPosition.x + (_distanceBetweenSideBricksL), _bricksTransform[0].localPosition.y, _bricksTransform[0].localPosition.z);
        _bricksTransform[3].localPosition = new Vector3(_bricksTransform[3].localPosition.x - (_distanceBetweenSideBricksL), _bricksTransform[3].localPosition.y, _bricksTransform[3].localPosition.z);

        var slotList = _craftDeviceSlot.GetCraftZoneDisplayedBioBricks() as CraftZoneDisplayedBioBrick[];
        for (var i = 0; i < slotList.Length; i++)
        {
            slotList[i].transform.localPosition = _bricksTransform[i].localPosition;
        }
    }
}

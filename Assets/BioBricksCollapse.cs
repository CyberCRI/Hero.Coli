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
    private Vector3[] _goTo = new Vector3[4];
    private Vector3[] _origin = new Vector3[4];

    // Use this for initialization
    void Start () {
        _distanceBetweenMiddleBricks = Vector3.Distance(_bricksTransform[1].localPosition, _bricksTransform[2].localPosition) - _referenceTexture.width;
        _distanceBetweenSideBricksL = Vector3.Distance(_bricksTransform[0].localPosition, _bricksTransform[1].localPosition) + (_distanceBetweenMiddleBricks / 2) - _referenceTexture.width;
        _distanceBetweenSideBricksR = Vector3.Distance(_bricksTransform[2].localPosition, _bricksTransform[3].localPosition) + (_distanceBetweenMiddleBricks / 2) - _referenceTexture.width;

        for (var i = 0; i < _origin.Length; i++)
        {
            _origin[i] = _bricksTransform[i].localPosition;
        }

        _craftDeviceSlot = this.gameObject.GetComponent<CraftDeviceSlot>();
    }

    public void Collapse()
    {
        /*_bricksTransform[1].localPosition*/ _goTo[1] = new Vector3(_bricksTransform[1].localPosition.x + (_distanceBetweenMiddleBricks / 2), _bricksTransform[1].localPosition.y, _bricksTransform[1].localPosition.z);
        /*_bricksTransform[2].localPosition*/ _goTo[2] = new Vector3(_bricksTransform[2].localPosition.x - (_distanceBetweenMiddleBricks / 2), _bricksTransform[2].localPosition.y, _bricksTransform[2].localPosition.z);

        /*_bricksTransform[0].localPosition*/ _goTo[0] = new Vector3(_bricksTransform[0].localPosition.x + (_distanceBetweenSideBricksL), _bricksTransform[0].localPosition.y, _bricksTransform[0].localPosition.z);
        /*_bricksTransform[3].localPosition*/ _goTo[3] = new Vector3(_bricksTransform[3].localPosition.x - (_distanceBetweenSideBricksL), _bricksTransform[3].localPosition.y, _bricksTransform[3].localPosition.z);
        
        var slotList = _craftDeviceSlot.GetCraftZoneDisplayedBioBricks() as CraftZoneDisplayedBioBrick[];
        for (var i = 0; i < slotList.Length; i++)
        {
            slotList[i].transform.localPosition = _bricksTransform[i].localPosition;
        }

        StartCoroutine(MoveBricks(50f));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            StartCoroutine(MoveBricksBack(50f));
        }
    }

    IEnumerator MoveBricks(float speed)
    {
        float time = Time.realtimeSinceStartup;
        float deltaTime = Time.realtimeSinceStartup - time;
        float multiplicator = 1f;
        while (Vector3.Distance(_bricksTransform[0].localPosition , _goTo[0]) >= 0.002f)
        {
            multiplicator += 1f;
            deltaTime = Time.realtimeSinceStartup - time;
            time = Time.realtimeSinceStartup;
            for (var i = 0; i < _bricksTransform.Length; i++)
            {
                Debug.Log("executing");
                _bricksTransform[i].localPosition = Vector3.MoveTowards(_bricksTransform[i].localPosition, _goTo[i], speed * deltaTime * multiplicator);
                yield return null;
            }

            var slotList = _craftDeviceSlot.GetCraftZoneDisplayedBioBricks() as CraftZoneDisplayedBioBrick[];
            for (var i = 0; i < slotList.Length; i++)
            {
                slotList[i].transform.localPosition = _bricksTransform[i].localPosition;
            }
        }
        yield return null;
    }

    IEnumerator MoveBricksBack(float speed)
    {
        float time = Time.realtimeSinceStartup;
        float deltaTime = Time.realtimeSinceStartup - time;
        float multiplicator = 1f;
        while(Vector3.Distance(_bricksTransform[0].localPosition, _origin[0]) >= 0.002f)
        {
            multiplicator += 1f;
            deltaTime = Time.realtimeSinceStartup - time;
            time = Time.realtimeSinceStartup;
            for (var i = 0; i < _bricksTransform.Length; i++)
            {
                if (_bricksTransform[i] != null)
                {
                    Debug.Log("executing");
                    _bricksTransform[i].localPosition = Vector3.MoveTowards(_bricksTransform[i].localPosition, _origin[i], speed * deltaTime * multiplicator);
                    yield return null;
                }
                yield return null;
            }

            var slotList = _craftDeviceSlot.GetCraftZoneDisplayedBioBricks() as CraftZoneDisplayedBioBrick[];
            for (var i = 0; i < slotList.Length; i++)
            {
                if (slotList[i] != null)
                {
                    slotList[i].transform.localPosition = _bricksTransform[i].localPosition;
                }
            }
        }
        yield return null;
    }

    public void StartMoveBricksBack()
    {
        StartCoroutine(MoveBricksBack(50f));
    }
}

// #define DEV
using UnityEngine;
using System.Collections;

public class BioBricksCollapse : MonoBehaviour {

    [SerializeField]
    private Transform[] _bricksTransform;
    private float _distanceBetweenMiddleBricks;
    private float _distanceBetweenSideBricksL;
    private float _distanceBetweenSideBricksR;
    [SerializeField]
    private Texture _referenceTexture;
    private CraftDeviceSlot _craftDeviceSlot;
    private const int _bricksCount = 4;
    private Vector3[] _goTo = new Vector3[_bricksCount];
    private Vector3[] _origin = new Vector3[_bricksCount];

    public delegate void Callback();
    public Callback onBricksStoppedMoving;
    
    private bool _initialized = false;
    private void lazyInitialize()
    {
        if(!_initialized)
        {   
        
            _distanceBetweenMiddleBricks = Vector3.Distance(_bricksTransform[1].localPosition, _bricksTransform[2].localPosition) - _referenceTexture.width;
            _distanceBetweenSideBricksL = Vector3.Distance(_bricksTransform[0].localPosition, _bricksTransform[1].localPosition) + (_distanceBetweenMiddleBricks / 2) - _referenceTexture.width;
            _distanceBetweenSideBricksR = Vector3.Distance(_bricksTransform[2].localPosition, _bricksTransform[3].localPosition) + (_distanceBetweenMiddleBricks / 2) - _referenceTexture.width;

            _craftDeviceSlot = this.gameObject.GetComponent<CraftDeviceSlot>();
        
            for (int i = 0; i < _bricksCount; i++)
            {
                _origin[i] = _bricksTransform[i].localPosition;
            }            
        
            _goTo[1] = new Vector3(_bricksTransform[1].localPosition.x + (_distanceBetweenMiddleBricks / 2), _bricksTransform[1].localPosition.y, _bricksTransform[1].localPosition.z);
            _goTo[2] = new Vector3(_bricksTransform[2].localPosition.x - (_distanceBetweenMiddleBricks / 2), _bricksTransform[2].localPosition.y, _bricksTransform[2].localPosition.z);

            _goTo[0] = new Vector3(_bricksTransform[0].localPosition.x + (_distanceBetweenSideBricksL), _bricksTransform[0].localPosition.y, _bricksTransform[0].localPosition.z);
            _goTo[3] = new Vector3(_bricksTransform[3].localPosition.x - (_distanceBetweenSideBricksL), _bricksTransform[3].localPosition.y, _bricksTransform[3].localPosition.z);
            
            _initialized = true;
        }
    }

    public void setPosition(bool expanded)
    {   
        lazyInitialize();
        
        Vector3[] targetPosition = expanded?_origin:_goTo;
        
        for(int index = 0; index < _bricksCount; index++)
        {
            if(null != _bricksTransform[index])
            {
                _bricksTransform[index].localPosition = targetPosition[index];
            }
        }
        
        moveBricks();
    }

    // Use this for initialization
    void Start () {
        lazyInitialize();
    }

    public void startCollapseBricks()
    {
        // Debug.Log("startCollapseBricks");
        
        StopAllCoroutines();
        
        lazyInitialize();
        
        moveBricks();

        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(collapseBricks(50f));
        }        
        else
        {
            // Debug.Log("startCollapseBricks !gameObject.activeInHierarchy");
            setPosition(false);
        }
        onBricksStoppedMoving();
    }

#if DEV
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            StartCoroutine(expandBricks(50f));
        }
    }
#endif

    IEnumerator collapseBricks(float speed)
    {
        float multiplicator = 1f;
        while (Vector3.Distance(_bricksTransform[0].localPosition , _goTo[0]) >= 0.002f)
        {
            multiplicator += 1f;
            for (int i = 0; i < _bricksCount; i++)
            {
                //Debug.Log("executing");
                _bricksTransform[i].localPosition = Vector3.MoveTowards(_bricksTransform[i].localPosition, _goTo[i], speed * Time.unscaledDeltaTime * multiplicator);
            }
            moveBricks();
            yield return null;
        }
        yield return null;
    }

    IEnumerator expandBricks(float speed)
    {
        float multiplicator = 1f;
        while(Mathf.Abs(_bricksTransform[0].localPosition.x - _origin[0].x) >= 0.002f)
        {
            multiplicator += 1f;
            for (int i = 0; i < _bricksCount; i++)
            {
                if (_bricksTransform[i] != null)
                {
                    //Debug.Log("executing");
                    _bricksTransform[i].localPosition = Vector3.MoveTowards(_bricksTransform[i].localPosition, _origin[i], speed * Time.unscaledDeltaTime * multiplicator);
                }
            }
            moveBricks();
            yield return null;
        }
        yield return null;
    }
    
    private void moveBricks()
    {
        // Debug.Log("moveBricks");
        CraftZoneDisplayedBioBrick[] slotList = _craftDeviceSlot.getCraftZoneDisplayedBioBricks();
        for (int i = 0; i < slotList.Length; i++)
        {
            if (slotList[i] != null)
            {
                slotList[i].transform.localPosition = _bricksTransform[i].localPosition;
            }
        }
    }

    public void startExpandBricks()
    {
        //Debug.Log("startExpandBricks");
        StopAllCoroutines();
        
        lazyInitialize();
       
       if(gameObject.activeInHierarchy)
        {
            StartCoroutine(expandBricks(50f));
        }
        else
        {
            setPosition(false);
        }
        onBricksStoppedMoving();
    }
}

﻿// #define DEV
using UnityEngine;

public class FocusMaskManager : MonoBehaviour
{
	public delegate void FocusEvent ();
	public static event FocusEvent onFocusOn;
    [SerializeField]
    private GameObject focusMask;
    [SerializeField]
    private GameObject hole;
    [SerializeField]
    private GameObject arrowSystem;
    [SerializeField]
    private UISprite arrowSprite;
    [SerializeField]
    private GameObject clickBlocker;
    private ExternalOnPressButton _target;
    private bool _isBlinking = false;
    private bool _isAlphaIncreasing = false;
    private const float _blinkingSpeed = 0.5f;
    private float _translatingSpeed = 100f;
    private const float _minAlpha = 0.7f;
    private const float _maxAlpha = 1f;
    private float _newAlpha;
    private bool _isClicksBlocked = false;
    private Vector3 _baseFocusMaskScale, _baseHoleScale;
    private float _baseHoleArrowDistance;
    [SerializeField]
    private Advisor _advisor;
    [HideInInspector]
    public CellControl cellControl;
    [SerializeField]
    private UIAnchor _anchor;
    [SerializeField]
    private Camera _interfaceCamera;
    private Camera _worldCamera;

    // test code
#if DEV
    public ExternalOnPressButton testClickable;
    public GameObject testObject;
    public Vector3 testPosition;
#endif

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "FocusMaskSystem";
    protected static FocusMaskManager _instance;
    public static FocusMaskManager get()
    {
        // Debug.Log(" FocusMaskManager get");
        if (_instance == null)
        {
            Debug.LogWarning("FocusMaskManager get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<FocusMaskManager>();
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            _initialized = true;

            _instance = this;
            _baseFocusMaskScale = focusMask.transform.localScale;
            _baseHoleScale = hole.transform.localScale;
            _baseHoleArrowDistance = arrowSprite.transform.localPosition.y;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        reset(false);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public delegate void Callback();
    private Callback _callback;

    public void focusOn(ExternalOnPressButton target, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        // Debug.Log(this.GetType() + " focusOn0 ExternalOnPressButton " + target.name);
        focusOn(target, Vector3.zero, callback, advisorTextKey, scaleToComponent);
    }

    public void focusOn(ExternalOnPressButton target, Vector3 manualScale, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        if (null != target)
        {
            // Debug.Log(this.GetType() + " focusOn ExternalOnPressButton " + target.name);
            float scaleFactor = computeScaleFactor(scaleToComponent, target.transform.localScale, manualScale);

            focusOn(getScreenPosition(_interfaceCamera, target.transform.position), callback, scaleFactor, false, advisorTextKey);
            _target = target;
        }
        else
        {
            // Debug.Log(this.GetType() + " focusOn null");
        }
    }

    public void focusOn(GameObject go, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        // Debug.Log(this.GetType() + " focusOn(GameObject go, Callback callback,...)");
        focusOn(go, Vector3.zero, callback, advisorTextKey, scaleToComponent);
    }

    public void focusOn(GameObject go, Vector3 manualScale, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        // Debug.Log(this.GetType() + " focusOn(GameObject go, Vector3 manualScale,...)");

        if (null != go)
        {

            float scaleFactor = computeScaleFactor(scaleToComponent, go.transform.localScale, manualScale);

            bool isInterfaceObject = (this.gameObject.layer == go.layer);
            // Debug.Log(this.GetType() + " isInterfaceObject=" + isInterfaceObject + " because layer=" + go.layer);
            Camera camera = _interfaceCamera;
            if (!isInterfaceObject)
            {
                // Debug.Log(this.GetType() + " !isInterfaceObject");
                _worldCamera = null == _worldCamera ? GameObject.FindWithTag("MainCamera").GetComponentInChildren<Camera>() : _worldCamera;
                camera = _worldCamera;
            }
            else
            {
                // Debug.Log(this.GetType() + " isInterfaceObject");
            }
            focusOn(getScreenPosition(camera, go.transform.position), callback, scaleFactor, !isInterfaceObject, advisorTextKey, true);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " focusOn: game object is null");
        }
    }

    float computeScaleFactor(bool scaleToComponent, Vector3 localScale, Vector3 manualScale)
    {
        Vector3 scale = Vector3.zero;
        float result = 1f;

        if (Vector3.zero != manualScale)
        {
            scale = manualScale;
        }
        else if (scaleToComponent)
        {
            scale = localScale;
        }

        if (Vector3.zero != scale)
        {
            float max = scale.x > scale.y ? scale.x : scale.y;
            result = max / _baseHoleScale.x;
        }

        result = result < 1f ? 1f : result;

        return result;
    }

    private static Vector2 getScreenPosition(Camera camera, Vector3 gameObjectPosition)
    {
        // Debug.Log("FocusMaskManager getScreenPosition");
        if (null != camera && null != gameObjectPosition)
        {
            // Debug.Log("FocusMaskManager getScreenPosition(" + camera.name + "," + gameObjectPosition + ")");
            Vector3 screenPoint = camera.WorldToScreenPoint(gameObjectPosition);
            Vector2 screenPosition = new Vector2(screenPoint.x / camera.pixelWidth - 0.5f, screenPoint.y / camera.pixelHeight - 0.5f);
            // Debug.Log("FocusMaskManager getScreenPosition(" + camera.name + "," + gameObjectPosition + ") result=" + screenPosition);
            return screenPosition;
        }
        else
        {
            Debug.LogWarning("FocusMaskManager getScreenPosition: null parameter");
            return Vector2.zero;
        }
    }

    // TODO add bool argument to force updated positioning of focus mask and arrow to prevent misplacement bugs
    // cf issue #345
    // position.x and position.y are in [0,1]
    public void focusOn(Vector2 position, Callback callback = null, float scaleFactor = 1f, bool local = true, string advisorTextKey = null, bool showButton = false)
    {
        // Debug.Log(this.GetType() + " focusOn(" + position + ")");
        if (null != position
            && position.x >= -1
            && position.x <= 1
            && position.y >= -1
            && position.y <= 1
            )
        {
            reset(true);

            _target = null;

            _anchor.relativeOffset = position;

            if (1f != scaleFactor)
            {
                // Debug.Log(this.GetType() + " will scale focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale + " with factor=" + scaleFactor);
                focusMask.transform.localScale = scaleFactor * _baseFocusMaskScale;
                hole.transform.localScale = scaleFactor * _baseHoleScale;
                // Debug.Log(this.GetType() + " now focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale);
            }
            else
            {
                // Debug.Log(this.GetType() + " will scale back focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale);
                focusMask.transform.localScale = _baseFocusMaskScale;
                hole.transform.localScale = _baseHoleScale;
                // Debug.Log(this.GetType() + " now focusMask=" + focusMask.transform.localScale + " and hole=" + hole.transform.localScale);
            }

            pointAt(position);

            _callback = callback;

            if (!string.IsNullOrEmpty(advisorTextKey))
            {
                _advisor.setUpNanoBot(position, advisorTextKey, scaleFactor, showButton);
                _advisor.gameObject.SetActive(true);
            }
            else
            {
                _advisor.gameObject.SetActive(false);
            }

            show(true);
			onFocusOn ();
        }

    }

    public void blockClicks(bool block)
    {
        _isClicksBlocked = block;
        clickBlocker.SetActive(block);
    }

    private void show(bool show)
    {
        _isBlinking = show;
        focusMask.SetActive(show);
        hole.SetActive(show);
        arrowSprite.gameObject.SetActive(show);
        _advisor.gameObject.SetActive(show);
        if (null != cellControl)
        {
            // Debug.Log(this.GetType() + " show(" + show + ") freezePlayer(" + show + ")");
            cellControl.freezePlayer(show);
        }
    }

    public void reset(bool keepDisplayed)
    {
        // Debug.Log(this.GetType() + " reinitialize");
        this.gameObject.SetActive(true);
        show(keepDisplayed);
        clickBlocker.SetActive(keepDisplayed);

        _isBlinking = false;
        _isAlphaIncreasing = false;
        arrowSprite.alpha = 1;
        focusMask.transform.localScale = _baseFocusMaskScale;
        hole.transform.localScale = _baseHoleScale;
        Vector3 lp = arrowSprite.transform.localPosition;
        arrowSprite.transform.localPosition = new Vector3(lp.x, _baseHoleArrowDistance, lp.z);
        _callback = null;
    }

    public void click()
    {
        if (!_isClicksBlocked)
        {
            if (_target)
            {
                _target.OnPress(true);
            }
            if (null != _callback)
            {
                _callback();
            }
        }
    }

    void Update()
    {
        if (_isBlinking)
        {
            Vector3 lp = arrowSprite.transform.localPosition;
            if (_isAlphaIncreasing)
            {
                _newAlpha = arrowSprite.alpha + _blinkingSpeed * Time.unscaledDeltaTime;
                arrowSprite.transform.localPosition = new Vector3(lp.x, lp.y + _translatingSpeed * Time.unscaledDeltaTime, lp.z);
                if (_newAlpha > _maxAlpha)
                {
                    _newAlpha = _maxAlpha;
                    _isAlphaIncreasing = false;
                }
            }
            else
            {
                _newAlpha = arrowSprite.alpha - _blinkingSpeed * Time.unscaledDeltaTime;
                arrowSprite.transform.localPosition = new Vector3(lp.x, lp.y - _translatingSpeed * Time.unscaledDeltaTime, lp.z);
                if (_newAlpha < _minAlpha)
                {
                    _newAlpha = _minAlpha;
                    _isAlphaIncreasing = true;
                }
            }
            arrowSprite.alpha = _newAlpha;
        }

        // test code
#if DEV
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            testClickable = GameObject.Find("CraftButton").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            testClickable = GameObject.Find("AvailableDisplayedDTER").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            focusOn(testPosition);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            focusOn(GameObject.Find("CraftButton"), null, null, true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            focusOn(Character.get().gameObject, null, null, false);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameObject testObject = GameObject.Find("TestRock12");
            focusOn(testObject, null, null, false);
        }
#endif
    }

    private void pointAt(Vector3 position)
    {
        float rotZ = 0f;

        TooltipManager.Quadrant quadrant = TooltipManager.getQuadrant(position);
        float quarterTurns = 0f;
        switch (quadrant)
        {
            case TooltipManager.Quadrant.BOTTOM_RIGHT:
                quarterTurns = 0;
                break;
            case TooltipManager.Quadrant.TOP_RIGHT:
                quarterTurns = 1f;
                break;
            case TooltipManager.Quadrant.TOP_LEFT:
                quarterTurns = 2f;
                break;
            case TooltipManager.Quadrant.BOTTOM_LEFT:
                quarterTurns = 3f;
                break;
            default:
                break;
        }
        rotZ = 45f + quarterTurns * 90f;
        arrowSystem.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public void stopFocusOn()
    {
        reset(false);
    }
}

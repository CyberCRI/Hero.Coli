using UnityEngine;

public class FocusMaskManager : MonoBehaviour {

    public GameObject focusMask, hole;
    public UISprite focusMaskSprite;
    public GameObject clickBlocker;
    private ExternalOnPressButton _target;
    private CellControl _cellControl;
    private bool _isAlphaIncreasing = false;
    private float _minAlpha = 0.2f;
    private float _maxAlpha = 1f;
    private float _newAlpha;
    private bool _isClicksBlocked = false;
    [SerializeField]
    private Advisor _advisor;
    
    // test code
    /*
    public ExternalOnPressButton testClickable;
    public GameObject testObject;
    public Vector3 testPosition;
    //*/
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
  protected const string gameObjectName = "FocusMaskSystem";
  protected static FocusMaskManager _instance;
  public static FocusMaskManager get() {
      //Debug.LogError("FocusMaskManager get");
    if(_instance == null) {
      Logger.Log("FocusMaskManager::get was badly initialized", Logger.Level.WARN);      
      _instance = GameObject.Find(gameObjectName).GetComponent<FocusMaskManager>();
    }
    return _instance;
  }
  
  void Awake()
  {
    Logger.Log("FocusMaskManager::Awake", Logger.Level.DEBUG);
    //Debug.LogError("FocusMaskManager Awake");
    _instance = this;
    initialize();
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

    public delegate void Callback(); 
    private Callback _callback;
    
	public void focusOn(ExternalOnPressButton target, Callback callback = null, string advisorTextKey = null)
    {
        if(null != target)
        {
            //Debug.LogError("FocusMaskManager focusOn "+target.name);
            focusOn(target.transform.position, false, advisorTextKey);
            _target = target;
            _callback = callback;
        }
        else
        {
            //Debug.LogError("FocusMaskManager focusOn null");
        }
    }
    
    public void focusOn(GameObject go, bool isInterfaceObject, string advisorTextKey = null)
    {   
        if(isInterfaceObject)
        {
            focusOn(go.transform.position, !isInterfaceObject, advisorTextKey);
        }
        else
        {
            //Vector3 convertedPosition = new Vector3(go.transform.position.x, go.transform.position.z, go.transform.position.y);
            
            Camera camera = GameObject.Find("Player").GetComponentInChildren<Camera>();
            Vector3 convertedPosition = camera.WorldToScreenPoint(go.transform.position);
            convertedPosition -= focusMask.transform.localScale/4;
            focusOn(convertedPosition, !isInterfaceObject,advisorTextKey);
        }
    }
    
    public void focusOn(Vector3 position, bool local = true, string advisorTextKey = null)
    {
        //Debug.LogError("focusOn("+position+")");
        if(null != position)
        {
            _target = null;
            show(true);
            //Debug.LogError("old pos="+this.transform.position);
            
            Vector3 newPosition;
            if(local)
            {
                newPosition = new Vector3(position.x, position.y, this.transform.localPosition.z);
                this.transform.localPosition = newPosition;
            }
            else
            {
                newPosition = new Vector3(position.x, position.y, this.transform.position.z);
                this.transform.position = newPosition;
            } 
            //Debug.LogError("new glopos="+this.transform.position);
            //Debug.LogError("new pos="+newPosition);

            
        }
        if (string.IsNullOrEmpty(advisorTextKey) == false)
        {
            if (this.transform.localPosition.y >= 0)
            {
                _advisor.setUpNanoBot(false, advisorTextKey);
            }
            else
            {
                _advisor.setUpNanoBot(true, advisorTextKey);
            }
            _advisor.gameObject.SetActive(true);
        }
        else
        {
            _advisor.gameObject.SetActive(false);
        }
    }

    public void blockClicks(bool block)
    {
        _isClicksBlocked = block;
        clickBlocker.SetActive(block);
    }
    
    private void show(bool show)
    {
        focusMask.SetActive(show);
        hole.SetActive(show);
        _advisor.gameObject.SetActive(show);
        GameObject perso = GameObject.Find("Perso");
        if(null != perso)
        {
            _cellControl = (_cellControl==null)?perso.GetComponent<CellControl>():_cellControl;
            _cellControl.freezePlayer(show);
        }
    }
    
    public void initialize()
    {
        //Debug.LogError("FocusMaskManager initialize");
        this.gameObject.SetActive(true);
        show(false);
        clickBlocker.SetActive(false);

        _isAlphaIncreasing = false;
        focusMaskSprite.alpha = 1;
    }
    
    public void click()
    {
        if(!_isClicksBlocked)
        {
            if(_target)
            {
                _target.OnPress(true);
            }
            if(null != _callback)
            {
                _callback();
            }
            initialize();
        }
    }

	void Update ()
    {
        if(_isAlphaIncreasing)
        {
            _newAlpha = focusMaskSprite.alpha + Time.deltaTime;
            if(_newAlpha > _maxAlpha)
            {
                _newAlpha = _maxAlpha;
                _isAlphaIncreasing = false;
            }
        }
        else
        {
            _newAlpha = focusMaskSprite.alpha - Time.deltaTime;
            if(_newAlpha < _minAlpha)
            {
                _newAlpha = _minAlpha;
                _isAlphaIncreasing = true;
            }
        }
        focusMaskSprite.alpha = _newAlpha;
    }
    
    //test code
    /*
    void Update()
    {
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
            focusOn(GameObject.Find("CraftButton"), true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            focusOn(GameObject.Find("Perso"), false);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            testObject = GameObject.Find("TestRock12");
            focusOn(testObject, false);
        }
    }
    //*/
}

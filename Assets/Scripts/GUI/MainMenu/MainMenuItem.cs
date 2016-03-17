using UnityEngine;

public class MainMenuItem : MonoBehaviour {
    
    private bool _initialized = false;

    private bool _displayed = true;
    public bool displayed {
        get {
            return _displayed;
        }
        set {
            _displayed = value; 

        }
    }
    protected UILocalize _localize;
    private string _itemName;
    public string itemName {
        get {
            return _itemName;
        }
        set {
            _itemName = value; 
            _localize = null == _localize?gameObject.GetComponentInChildren<UILocalize>():_localize;
            if(null == _localize) {
                Logger.Log("no localize found", Logger.Level.WARN);
            } else {
                _localize.key = value;
                _localize.Localize();
            }
        }
    }
    public float hoverExpandingFactor = 1.2f;
    private UIAnchor _anchor;
    public UIAnchor anchor {
        get {
            if(null == _anchor) {
                _anchor = this.gameObject.GetComponent<UIAnchor>();
            }
            return _anchor;
        }
    }
    
    public void select() {
        transform.localScale = new Vector3(transform.localScale.x*hoverExpandingFactor, transform.localScale.y*hoverExpandingFactor, transform.localScale.z);
    }

    public void deselect() {
        transform.localScale = new Vector3(transform.localScale.x/hoverExpandingFactor, transform.localScale.y/hoverExpandingFactor, transform.localScale.z);
    }

    public virtual void click () {
        Logger.Log("clicked "+itemName, Logger.Level.INFO);
    }

    void OnPress(bool isPressed) {
        if(isPressed) {
            click ();
        }
    }

    void OnHover(bool isOver) {
        if(isOver) {
            MainMenuManager.get ().onHover(this);
        }
    }

    private void initializeNameFromLocalizationKey() {
        //Debug.LogError("initializeNameFromLocalizationKey '"+itemName+"' starts");
        bool previousState = gameObject.activeInHierarchy;
        gameObject.SetActive(true);
        UILocalize localize = gameObject.GetComponentInChildren<UILocalize>();
        if(null == localize) {
            Logger.Log("no localize found", Logger.Level.WARN);
            itemName = gameObject.name;
            //Debug.LogError("no localize found, activeInHierarchy="+gameObject.activeInHierarchy+", activeSelf"+gameObject.activeSelf);
        } else {
            itemName = localize.key;
        }
        gameObject.SetActive(previousState);
        //Debug.LogError("initializeNameFromLocalizationKey '"+itemName+"' ends");
    }

  public virtual void initialize() {
  }
  
  public void initializeIfNecessary() {
      //Debug.LogError("initializeIfNecessary '"+itemName+"' starts");
      if(!_initialized) {
            //Debug.LogError("initializeIfNecessary '"+itemName+"': _initialized="+_initialized);
            initializeNameFromLocalizationKey();
            initialize();
            _initialized = true;
            //Debug.LogError("initializeIfNecessary '"+itemName+"': now _initialized="+_initialized);
      }
      //Debug.LogError("initializeIfNecessary '"+itemName+"' ends");
  }

	// Use this for initialization
	void Start () {
        initializeIfNecessary();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

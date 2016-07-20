using UnityEngine;

public class FocusMaskManager : MonoBehaviour {

    public GameObject focusMask, hole;
    private ExternalOnPressButton _target;
    
    public ExternalOnPressButton testObject;
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
  protected const string gameObjectName = "FocusMaskSystem";
  protected static FocusMaskManager _instance;
  public static FocusMaskManager get() {
      Debug.LogError("FocusMaskManager get");
    if(_instance == null) {
      Logger.Log("FocusMaskManager::get was badly initialized", Logger.Level.WARN);      
      _instance = GameObject.Find(gameObjectName).GetComponent<FocusMaskManager>();
    }
    return _instance;
  }
  
  void Awake()
  {
    Logger.Log("FocusMaskManager::Awake", Logger.Level.DEBUG);
    Debug.LogError("FocusMaskManager Awake");
    _instance = this;
    initialize();
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

	public void focusOn(ExternalOnPressButton target)
    {
        if(null != target)
        {
            Debug.LogError("FocusMaskManager focusOn "+target.name);
            _target = target;
            
            this.gameObject.SetActive(true);
            focusMask.SetActive(true);
            hole.SetActive(true);
            
            this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, this.transform.position.z);
        }
        else
        {
            Debug.LogError("FocusMaskManager focusOn null");
        }
    }
    
    public void initialize()
    {
        Debug.LogError("FocusMaskManager initialize");
        this.gameObject.SetActive(true);
        focusMask.SetActive(false);
        hole.SetActive(false);
    }
    
    public void click()
    {
        if(_target)
        {
            initialize();
            _target.OnPress(true);
        }
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            testObject = GameObject.Find("CraftButton").GetComponent<ExternalOnPressButton>();
            focusOn(testObject);
        } else if(Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            testObject = GameObject.Find("AvailableDisplayedDTER").GetComponent<ExternalOnPressButton>();
            focusOn(testObject);
        }
    }
}

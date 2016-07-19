using UnityEngine;

public class FocusMaskManager : MonoBehaviour {

    public GameObject focusMask, hole;
    private MonoBehaviour _target;
    
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

	public void focusOn(MonoBehaviour target)
    {
        Debug.LogError("FocusMaskManager focusOn "+target.name);
        _target = target;
        
        this.gameObject.SetActive(true);
        focusMask.SetActive(true);
        hole.SetActive(true);
        
        this.transform.position = target.transform.position;
    }
    
    public void initialize()
    {
        Debug.LogError("FocusMaskManager initialize");
        this.gameObject.SetActive(false);
        focusMask.SetActive(false);
        hole.SetActive(false);
    }
    
    public void click()
    {
        if(_target)
        {
            //TODO call click on target
        }
    }
}

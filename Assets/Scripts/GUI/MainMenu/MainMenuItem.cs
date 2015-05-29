using UnityEngine;
using System.Collections;

public class MainMenuItem : MonoBehaviour {

    private UILocalize _localize;
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
        UILocalize localize = gameObject.GetComponentInChildren<UILocalize>();
        if(null == localize) {
            Logger.Log("no localize found", Logger.Level.WARN);
            itemName = gameObject.name;
        } else {
            itemName = localize.key;
        }
    }

	// Use this for initialization
	void Start () {
        initializeNameFromLocalizationKey();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

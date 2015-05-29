using UnityEngine;
using System.Collections;

public class MainMenuItem : MonoBehaviour {

    public string itemName;
    public float hoverExpandingFactor = 1.2f;
    
    public void select() {
        transform.localScale = new Vector3(transform.localScale.x*hoverExpandingFactor, transform.localScale.y*hoverExpandingFactor, transform.localScale.z);
    }

    public void deselect() {
        transform.localScale = new Vector3(transform.localScale.x/hoverExpandingFactor, transform.localScale.y/hoverExpandingFactor, transform.localScale.z);
    }

    public virtual void click() {
        Debug.LogWarning("clicked "+itemName);
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
            Debug.LogWarning("no localize found");
            itemName = gameObject.name;
        } else {
            itemName = gameObject.GetComponentInChildren<UILocalize>().key;
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

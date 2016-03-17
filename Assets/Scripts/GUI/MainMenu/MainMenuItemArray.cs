using UnityEngine;

public class MainMenuItemArray : MonoBehaviour {
    
    public MainMenuItem[] _items;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public override string ToString()
    {
        return "MainMenuItemArray["+MainMenuItem.ToString(_items)+"]";
    }
}

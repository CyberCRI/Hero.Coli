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
        return "MainMenuItemArray["+ToString(_items)+"]";
    }
    
    public static string ToString(MainMenuItem[] items)
    {
        string result = "";
        foreach(MainMenuItem item in items) {
            if(!string.IsNullOrEmpty(result))
            {
                result+=", ";
            }
            result += item.itemName;
        }
        result = "items=["+result+"]";
        return result;
    }
}

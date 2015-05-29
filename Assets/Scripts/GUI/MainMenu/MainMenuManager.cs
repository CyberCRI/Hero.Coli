using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{

    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "MainMenuManager";
    private static MainMenuManager _instance;

    public static MainMenuManager get ()
    {
        if (_instance == null) {
            Logger.Log ("MainMenuManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find (gameObjectName).GetComponent<MainMenuManager> ();
        }
        return _instance;
    }

    void Awake ()
    {
        Logger.Log ("MainMenuManager::Awake", Logger.Level.DEBUG);
        _instance = this;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public MainMenuItem[] _items;

    //_currentIndex == -1 means nothing is selected
    private int _currentIndex = -1;

    private bool isAnItemSelected ()
    {
        return _currentIndex >= 0 && _currentIndex < _items.Length;
    }

    public MainMenuItem getCurrentItem() {
        if(isAnItemSelected()){
            return _items[_currentIndex];
        } else {
            return null;
        }
    }

    private void deselect ()
    {
        if (isAnItemSelected ()) {
            _items [_currentIndex].deselect ();
            Debug.LogWarning ("deselected item " + _currentIndex);
        } else {
            Debug.LogWarning ("couldn't deselect item " + _currentIndex);
        }
        _currentIndex = -1;
    }

    private bool selectItem (string name)
    {
        if (isAnItemSelected () && name == _items [_currentIndex].itemName) {
            Debug.LogWarning ("item " + name + " was already selected");
            return true;
        } else {
            for (int index = 0; index < _items.Length; index++) {
                if (name == _items [index].itemName) {
                    deselect ();
                    _items [index].select ();
                    _currentIndex = index;
                    Debug.LogWarning ("selected item " + index + " via its name '" + name + "'");
                    return true;
                }
            }
            return false;
        }
    }

    private bool selectItem (int index)
    {
        int normalizedIndex = index % _items.Length;
        if (null != _items [normalizedIndex]) {
            deselect ();
            _items [normalizedIndex].select ();
            _currentIndex = normalizedIndex;
            Debug.LogWarning ("selected item " + normalizedIndex);
            return true;
        }
        return false;
    }
    
    public bool selectNext ()
    {
        Debug.LogWarning ("selectNext");
        return selectItem (_currentIndex + 1);
    }

    public bool selectPrevious ()
    {
        Debug.LogWarning ("selectPrevious");
        return selectItem (_currentIndex - 1);
    }

    public void onHover (MainMenuItem item)
    {
        Debug.LogWarning (item.itemName + " onHover");
        selectItem (item.itemName);
    }

    public void open() {
        this.gameObject.SetActive(true);
    }

    public void close() {
        this.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start ()
    {
        selectItem (0);
    }
  
    // Update is called once per frame
    void Update ()
       {
    }
}

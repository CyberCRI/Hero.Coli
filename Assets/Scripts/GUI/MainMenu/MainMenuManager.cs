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
    private static string newGameKey = "MENU.NEWGAME";
    private static string resumeKey = "MENU.RESUME";

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
            Logger.Log ("deselected item " + _currentIndex, Logger.Level.DEBUG);
        } else {
            Logger.Log ("couldn't deselect item " + _currentIndex, Logger.Level.WARN);
        }
        _currentIndex = -1;
    }

    private bool selectItem (string name)
    {
        if (isAnItemSelected () && name == _items [_currentIndex].itemName) {
            Logger.Log ("item " + name + " was already selected", Logger.Level.DEBUG);
            return true;
        } else {
            for (int index = 0; index < _items.Length; index++) {
                if (name == _items [index].itemName) {
                    deselect ();
                    _items [index].select ();
                    _currentIndex = index;
                    Logger.Log ("selected item " + index + " via its name '" + name + "'", Logger.Level.DEBUG);
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
            Logger.Log ("selected item " + normalizedIndex, Logger.Level.DEBUG);
            return true;
        }
        return false;
    }
    
    public bool selectNext ()
    {
        Logger.Log ("selectNext", Logger.Level.INFO);
        return selectItem (_currentIndex + 1);
    }

    public bool selectPrevious ()
    {
        Logger.Log ("selectPrevious", Logger.Level.INFO);
        return selectItem (_currentIndex - 1);
    }

    public void onHover (MainMenuItem item)
    {
        Logger.Log (item.itemName + " onHover", Logger.Level.DEBUG);
        selectItem (item.itemName);
    }

    public void replaceTextBy(string target, string replacement, string debug = "") {
        for(int index = 0; index < _items.Length; index++) {
            if(_items[index].itemName == target) {
                _items[index].itemName = replacement;
                Debug.LogError(debug+" BINGO at "+index+" with "+_items[index].itemName);
                return;
            }
        }
        Debug.LogError(debug+" FAIL");
    }

    public void setNewGame() {
        replaceTextBy(resumeKey, newGameKey, "setNewGame");
    }
    
    public void setResume() {
        replaceTextBy(newGameKey, resumeKey, "setResume");
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

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

    public MainMenuItemArray mainMenuItems;
    public MainMenuItemArray controlItems;
    public MainMenuItemArray languageItems;

    public float verticalSpacing;
    private static string menuKeyPrefix = "MENU.";
    private static string newGameKey = menuKeyPrefix+"NEWGAME";
    private static string resumeKey = menuKeyPrefix+"RESUME";
    private static string restartKey = menuKeyPrefix+"RESTART";
    public enum MainMenuScreen {
        CONTROLS,
        LANGUAGES,
        DEFAULT
    }

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
            Logger.Log ("no item selected: current index is " + _currentIndex, Logger.Level.WARN);
        }
        _currentIndex = -1;
    }

    private bool selectItem (string name)
    {
        Logger.Log ("selectItem("+name+")", Logger.Level.DEBUG);
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

    private enum SelectionMode {
        NEXT,
        PREVIOUS,
        NONE
    }

    private bool selectItem (int index, SelectionMode mode = SelectionMode.NEXT)
    {
        int previousIndex = index;
        int normalizedIndex;
        int remainingTries = _items.Length;

        while (remainingTries > 0) {
            normalizedIndex = previousIndex >= 0 ? previousIndex % _items.Length : (previousIndex % _items.Length) + _items.Length;
            //Debug.LogError(string.Format("selectItem({0}) selects {1} on try {2}/{3}", previousIndex, normalizedIndex, _items.Length-remainingTries, _items.Length));
            if (null != _items [normalizedIndex] && _items [normalizedIndex].displayed) {
                deselect ();
                _items [normalizedIndex].select ();
                _currentIndex = normalizedIndex;
                Logger.Log ("selected item " + normalizedIndex, Logger.Level.DEBUG);
                return true;
            }

            switch(mode) {
                case SelectionMode.NEXT:
                    previousIndex = normalizedIndex+1;
                    break;
                case SelectionMode.PREVIOUS:
                    previousIndex = normalizedIndex-1;
                    break;
                case SelectionMode.NONE:
                default:
                    return false;                
            }
            remainingTries--;
        }
        return false;
    }
    
    public bool selectNext ()
    {
        Logger.Log ("selectNext", Logger.Level.INFO);
        return selectItem (_currentIndex + 1, SelectionMode.NEXT);
    }

    public bool selectPrevious ()
    {
        Logger.Log ("selectPrevious", Logger.Level.INFO);
        return selectItem (_currentIndex - 1, SelectionMode.PREVIOUS);
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
        setVisibility(restartKey, false);
    }
    
    public void setResume() {
        replaceTextBy(newGameKey, resumeKey, "setResume");
        setVisibility(restartKey, true);
    }

    private void setVisibility (string itemKey, bool isVisible)
    {
        for(int index = 0; index < _items.Length; index++) {
            if(_items[index].itemName == itemKey) {
                _items[index].displayed = isVisible;
                redraw ();
                return;
            }
        }
    }

    public void redraw ()
    {
        if(_items.Length != 0) {
            Vector2 nextRelativeOffset = _items[0].anchor.relativeOffset;
            foreach(MainMenuItem item in _items)
            {
                item.gameObject.SetActive(item.displayed);
                if(item.displayed) {
                    item.anchor.relativeOffset = nextRelativeOffset;
                    nextRelativeOffset = new Vector2(nextRelativeOffset.x, nextRelativeOffset.y + verticalSpacing);
                }
            }
        } else {
            Logger.Log ("MainMenuManager::redraw no item", Logger.Level.WARN);
        }
    }

    public void switchTo (MainMenuScreen screen) 
    {
        switch (screen) {
            case MainMenuScreen.CONTROLS:
                deselect ();
                mainMenuItems.gameObject.SetActive(false);
                controlItems.gameObject.SetActive(true);
                languageItems.gameObject.SetActive(false);
                copyItemsFrom(controlItems);
                selectItem(0);
                break;
            case MainMenuScreen.LANGUAGES:
                deselect ();
                mainMenuItems.gameObject.SetActive(false);
                controlItems.gameObject.SetActive(false);
                languageItems.gameObject.SetActive(true);
                copyItemsFrom(languageItems);
                selectItem(0);
                break;
            case MainMenuScreen.DEFAULT:
            default:
                deselect ();
                mainMenuItems.gameObject.SetActive(true);
                controlItems.gameObject.SetActive(false);
                languageItems.gameObject.SetActive(false);
                copyItemsFrom(mainMenuItems);
                selectItem(0);
                break;
        }
    }

    private void copyItemsFrom(MainMenuItemArray array)
    {
        Debug.LogError("copyItemsFrom with #items="+array._items.Length);
        _items = new MainMenuItem[array._items.Length];
        for(int index = 0; index < array._items.Length; index++)
        {
            _items[index] = array._items[index];
        }
        Debug.LogError("copyItemsFrom DONE");
    }

    public void open() {
        this.gameObject.SetActive(true);
        switchTo (MainMenuScreen.DEFAULT);
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

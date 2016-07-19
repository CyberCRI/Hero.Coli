using UnityEngine;


public class MainMenuManager : MonoBehaviour
{
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "MainMenu";
    private static MainMenuManager _instance;

    public static MainMenuManager get ()
    {
        if (_instance == null) {
            Logger.Log("MainMenuManager::get was badly initialized", Logger.Level.WARN);
            
            //set from InterfaceLinkManager
            setInstance(InterfaceLinkManager.get().mainMenu);
            
            if (_instance == null) {
                _instance = GameObject.Find (gameObjectName).GetComponent<MainMenuManager> ();
            }
        }
        return _instance;
    }
    
    public static void setInstance(MainMenuManager instance)
    {
        _instance = instance;
    }

    void Awake ()
    {
        Logger.Log("MainMenuManager::Awake", Logger.Level.DEBUG);
        _instance = this;
        get();
    }
    
    void OnDestroy()
    {
        Debug.LogError("MainMenuManager OnDestroy");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public MainMenuItem[] _items;

    public MainMenuItemArray mainMenuItems;
    public MainMenuItemArray controlItems;
    public MainMenuItemArray languageItems;
    public MainMenuItemArray soundItems;
    public LearnMoreOptionsMainMenuItemArray learnMoreItems;

    public float verticalSpacing;
    public const float defaultVerticalSpacing = -0.1f;
    
    private static string menuKeyPrefix = "MENU.";
    private static string newGameKey = menuKeyPrefix+"NEWGAME";
    private static string resumeKey = menuKeyPrefix+"RESUME";
    private static string restartKey = menuKeyPrefix+"RESTART";
    public enum MainMenuScreen {
        CONTROLS,
        LANGUAGES,
        SOUNDOPTIONS,
        LEARNMOREOPTIONS,
        DEFAULT
    }

    //_currentIndex == -1 means nothing is selected
    private int _currentIndex = -1;

    private bool isAnItemSelected ()
    {
        //Debug.Log("MainMenuManager::isAnItemSelected");
        return _currentIndex >= 0 && _currentIndex < _items.Length;
    }

    public MainMenuItem getCurrentItem() {
        //Debug.Log("MainMenuManager::getCurrentItem");
        if(isAnItemSelected()){
            return _items[_currentIndex];
        } else {
            return null;
        }
    }

    private void deselect ()
    {
        //Debug.Log("MainMenuManager::deselect");
        if (isAnItemSelected ()) {
            _items [_currentIndex].deselect ();
            Logger.Log("MainMenuManager::deselected item " + _currentIndex, Logger.Level.DEBUG);
        } else {
            Logger.Log("MainMenuManager::no item selected: current index is " + _currentIndex, Logger.Level.WARN);
        }
        _currentIndex = -1;
    }

    private bool selectItem (string name)
    {
        //Debug.Log(string.Format("selectItem({0})", name));
        if (isAnItemSelected () && name == _items [_currentIndex].itemName) {
            Logger.Log("MainMenuManager::item " + name + " was already selected", Logger.Level.DEBUG);
            return true;
        } else {
            for (int index = 0; index < _items.Length; index++) {
                if (name == _items [index].itemName) {
                    deselect ();
                    _items [index].select ();
                    _currentIndex = index;
                    Logger.Log("MainMenuManager::selected item " + index + " via its name '" + name + "'", Logger.Level.DEBUG);
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
        //Debug.Log(string.Format("selectItem({0}, {1})", index, mode.ToString()));
        int previousIndex = index;
        int normalizedIndex;
        int remainingTries = _items.Length;

        while (remainingTries > 0) {
            normalizedIndex = previousIndex >= 0 ? previousIndex % _items.Length : (previousIndex % _items.Length) + _items.Length;
            if (null != _items [normalizedIndex] && _items [normalizedIndex].displayed) {
                deselect ();
                _items [normalizedIndex].select ();
                _currentIndex = normalizedIndex;
                Logger.Log("MainMenuManager::selected item " + normalizedIndex, Logger.Level.DEBUG);
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
        //Debug.Log("MainMenuManager::selectNext");
        return selectItem (_currentIndex + 1, SelectionMode.NEXT);
    }

    public bool selectPrevious ()
    {
        //Debug.Log("MainMenuManager::selectPrevious");
        return selectItem (_currentIndex - 1, SelectionMode.PREVIOUS);
    }

    public void onHover (MainMenuItem item)
    {
        //Debug.Log(string.Format("onHover({0})", item._itemName));
        Logger.Log (item.itemName + " onHover", Logger.Level.DEBUG);
        selectItem (item.itemName);
    }

    public static void replaceTextBy(string target, string replacement, MainMenuItem[] items, string debug = "") {
        //Debug.Log(string.Format("replaceTextBy({0}, {1}, {2}, {3})", target, replacement, MainMenuItemArray.ToString(items), debug));
        for(int index = 0; index < items.Length; index++) {
            if(items[index].itemName == target) {
                items[index].itemName = replacement;
                MainMenuManager.redraw (items);
                return;
            }
        }
        Logger.Log("MainMenuManager::MainMenuItem::replaceTextBy static "+debug+" FAIL with target="+target+" and replacement="+replacement, Logger.Level.WARN);
    }

    private void replaceTextBy(string target, string replacement, string debug = "") {
        //Debug.Log(string.Format("replaceTextBy({0}, {1}, {2})", target, replacement, debug));
        MainMenuManager.replaceTextBy(target, replacement, _items, debug);
    }

    public void setNewGame() {
        //Debug.Log("MainMenuManager::setNewGame");
        replaceTextBy(resumeKey, newGameKey, "setNewGame");
        setVisibility(restartKey, false);
    }
    
    public void setResume() {
        //Debug.Log("MainMenuManager::setResume");
        replaceTextBy(newGameKey, resumeKey, "setResume");
        setVisibility(restartKey, true);
    }

    private void setVisibility (string itemKey, bool isVisible) {
        //Debug.Log(string.Format("setVisibility({0},{1})", itemKey, isVisible.ToString()));
        setVisibility(_items, itemKey, isVisible, "MainMenuManager");
    }

    public static void setVisibility (MainMenuItem[] items, string itemKey, bool isVisible, string debug = null, float spacing = defaultVerticalSpacing) {
        //Debug.Log(string.Format("setVisibility({0},{1},{2},{3},{4})", MainMenuItemArray.ToString(items), itemKey, isVisible.ToString(), debug, spacing.ToString()));
        if(!string.IsNullOrEmpty(debug)) {
            //Debug.LogError("MainMenuManager::setVisibility(items, "+itemKey+", "+isVisible+", "+debug+", "+spacing);
        }
        for(int index = 0; index < items.Length; index++) {
            items[index].initializeIfNecessary();
            if(items[index].itemName == itemKey) {
                items[index].displayed = isVisible;
                if(!string.IsNullOrEmpty(debug)) {
                    //Debug.LogError("MainMenuManager::setVisibility "+debug+" found "+itemKey+" and set its visibility to "+isVisible);
                }
                break;
            } else if(!string.IsNullOrEmpty(debug)) {
                //Debug.LogError("MainMenuManager::setVisibility "+debug+": '"+itemKey+"'≠'"+items[index].itemName+"'");
            } 
        }
        MainMenuManager.redraw (items, debug, spacing);
    }

    public static void redraw (MainMenuItem[] items, string debug = null, float spacing = defaultVerticalSpacing) {
        //Debug.Log(string.Format("redraw({0}, {1}, {2})", MainMenuItemArray.ToString(items), debug, spacing.ToString()));
        if(!string.IsNullOrEmpty(debug)) {
            //Debug.LogError("MainMenuManager::redraw "+debug);
        }
        if(items.Length != 0) {
            Vector2 nextRelativeOffset = items[0].anchor.relativeOffset;
            foreach(MainMenuItem item in items)
            {
                item.gameObject.SetActive(item.displayed);
                if(!string.IsNullOrEmpty(debug)) {
                    //Debug.LogError("MainMenuManager::redraw "+debug+" set "+item.itemName+" activity to "+item.displayed);
                }
                if(item.displayed) {
                    item.anchor.relativeOffset = nextRelativeOffset;
                    nextRelativeOffset = new Vector2(nextRelativeOffset.x, nextRelativeOffset.y + spacing);
                }
            }
        } else {
            Logger.Log("MainMenuManager::redraw static no item", Logger.Level.WARN);
        }
    }

    private void redraw ()
    {
        //Debug.Log("MainMenuManager::redraw");
        MainMenuManager.redraw (_items, null, verticalSpacing);
    }

    public void switchTo (MainMenuScreen screen) 
    {
        //Debug.Log(string.Format("switchTo({0})", screen.ToString()));
        switch (screen) {
            case MainMenuScreen.CONTROLS:
                deselect ();
                mainMenuItems.gameObject.SetActive(false);
                controlItems.gameObject.SetActive(true);
                languageItems.gameObject.SetActive(false);
                soundItems.gameObject.SetActive(false);
                learnMoreItems.gameObject.SetActive(false);
                copyItemsFrom(controlItems);
                selectItem(0);
                break;
            case MainMenuScreen.LANGUAGES:
                deselect ();
                mainMenuItems.gameObject.SetActive(false);
                controlItems.gameObject.SetActive(false);
                languageItems.gameObject.SetActive(true);
                soundItems.gameObject.SetActive(false);
                learnMoreItems.gameObject.SetActive(false);
                copyItemsFrom(languageItems);
                selectItem(0);
                break;
            case MainMenuScreen.SOUNDOPTIONS:
                deselect ();
                mainMenuItems.gameObject.SetActive(false);
                controlItems.gameObject.SetActive(false);
                languageItems.gameObject.SetActive(false);
                soundItems.gameObject.SetActive(true);
                learnMoreItems.gameObject.SetActive(false);
                copyItemsFrom(soundItems);
                selectItem(0);
                break;
            case MainMenuScreen.LEARNMOREOPTIONS:
                deselect ();
                mainMenuItems.gameObject.SetActive(false);
                controlItems.gameObject.SetActive(false);
                languageItems.gameObject.SetActive(false);
                soundItems.gameObject.SetActive(false);
                learnMoreItems.setPlatform();
                learnMoreItems.gameObject.SetActive(true);
                copyItemsFrom(learnMoreItems);
                selectItem(0);
                break;
            case MainMenuScreen.DEFAULT:
            default:
                deselect ();
                mainMenuItems.gameObject.SetActive(true);
                controlItems.gameObject.SetActive(false);
                languageItems.gameObject.SetActive(false);
                soundItems.gameObject.SetActive(false);
                learnMoreItems.gameObject.SetActive(false);
                copyItemsFrom(mainMenuItems);
                selectItem(0);
                break;
        }
    }

    private void copyItemsFrom(MainMenuItemArray array)
    {
        //Debug.Log(string.Format("copyItemsFrom({0})", array.ToString()));
        _items = new MainMenuItem[array._items.Length];
        for(int index = 0; index < array._items.Length; index++)
        {
            _items[index] = array._items[index];
        }
    }

    public bool escape() {
        //Debug.Log("MainMenuManager::escape");
        BackMainMenuItem bmmi;
        ResumeMainMenuItem rmmi;
        foreach(MainMenuItem item in _items) {
            bmmi = item as BackMainMenuItem;
            if(null != bmmi) {
                bmmi.click ();
                return true;
            } else {
                rmmi = item as ResumeMainMenuItem;
                if(null != rmmi) {
                    if(resumeKey == rmmi.itemName) {
                        rmmi.click ();
                        return true;
                    } else {                        
                        return false;
                    }
                }
            }
        }
        GameStateController.get ().leaveMainMenu();
        return true;
    }

    public void open() {
        //Debug.Log("MainMenuManager::open");
        this.gameObject.SetActive(true);
        switchTo (MainMenuScreen.DEFAULT);
    }

    public void close() {
        //Debug.Log("MainMenuManager::close");
        this.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start ()
    {
        //Debug.Log("MainMenuManager::Start");
        selectItem (0);
    }
}

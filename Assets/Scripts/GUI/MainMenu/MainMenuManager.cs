using UnityEngine;


public class MainMenuManager : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "MainMenu";
    private static MainMenuManager _instance;
    [SerializeField]
    private bool _first = true;
    [SerializeField]
    private CullingMaskHandler cullingMaskHandler;

    public static MainMenuManager get()
    {
        if (_instance == null)
        {
            Logger.Log("MainMenuManager::get was badly initialized", Logger.Level.WARN);

            //set from InterfaceLinkManager
            _instance = InterfaceLinkManager.get().mainMenu;

            if (_instance == null)
            {
                GameObject go = GameObject.Find(gameObjectName);
                if (go)
                {
                    _instance = go.GetComponent<MainMenuManager>();
                }
            }
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            _initialized = true;
        }
    }

    // Use this for initialization
    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        selectItem(0);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public MainMenuItem[] _items;

    public MainMenuItemArray mainMenuItems;
    public MainMenuItemArray settingsItems;
    public MainMenuItemArray controlItems;
    public MainMenuItemArray languageItems;
    public MainMenuItemArray soundItems;
    public LearnMoreOptionsMainMenuItemArray learnMoreItems;

    [SerializeField]
    private ParticleSystemFeedback feedback;
    private Vector3 shift = new Vector3(0, 0, -0.4f);

    public float verticalSpacing;
    private const float defaultVerticalSpacing = -0.1f;

    private const string menuKeyPrefix = "MENU.";
    private const string newGameKey = menuKeyPrefix + "NEWGAME";
    private const string resumeKey = menuKeyPrefix + "RESUME";
    private const string restartKey = menuKeyPrefix + "RESTART";
    public enum MainMenuScreen
    {
        SETTINGS,
        LEARNMOREOPTIONS,
        CONTROLS,
        LANGUAGES,
        SOUNDOPTIONS,
        DEFAULT
    }

    //_currentIndex == -1 means nothing is selected
    private int _currentIndex = -1;

    private bool isAnItemSelected()
    {
        // Debug.Log("MainMenuManager::isAnItemSelected");
        return _currentIndex >= 0 && _currentIndex < _items.Length;
    }

    public MainMenuItem getCurrentItem()
    {
        // Debug.Log("MainMenuManager::getCurrentItem");
        if (isAnItemSelected())
        {
            return _items[_currentIndex];
        }
        else
        {
            return null;
        }
    }

    private void deselect()
    {
        // Debug.Log("MainMenuManager::deselect");
        if (isAnItemSelected())
        {
            _items[_currentIndex].deselect();
            Logger.Log("MainMenuManager::deselected item " + _currentIndex, Logger.Level.DEBUG);
        }
        else
        {
            Logger.Log("MainMenuManager::no item selected: current index is " + _currentIndex, Logger.Level.WARN);
        }
        _currentIndex = -1;
    }

    public void playFeedback(Vector3 position)
    {
        feedback.burst(position + shift);
    }

    private bool selectItem(string name)
    {
        // Debug.Log(string.Format("selectItem({0})", name));
        if (isAnItemSelected() && name == _items[_currentIndex].itemName)
        {
            Logger.Log("MainMenuManager::item " + name + " was already selected", Logger.Level.DEBUG);
            return true;
        }
        else
        {
            for (int index = 0; index < _items.Length; index++)
            {
                if (name == _items[index].itemName)
                {
                    deselect();
                    _items[index].select();
                    _currentIndex = index;
                    Logger.Log("MainMenuManager::selected item " + index + " via its name '" + name + "'", Logger.Level.DEBUG);
                    return true;
                }
            }
            return false;
        }
    }

    private enum SelectionMode
    {
        NEXT,
        PREVIOUS,
        NONE
    }

    private bool selectItem(int index, SelectionMode mode = SelectionMode.NEXT)
    {
        // Debug.Log(string.Format("selectItem({0}, {1})", index, mode.ToString()));
        int previousIndex = index;
        int normalizedIndex;
        int remainingTries = _items.Length;

        while (remainingTries > 0)
        {
            normalizedIndex = previousIndex >= 0 ? previousIndex % _items.Length : (previousIndex % _items.Length) + _items.Length;
            if (null != _items[normalizedIndex] && _items[normalizedIndex].displayed)
            {
                deselect();
                _items[normalizedIndex].select();
                _currentIndex = normalizedIndex;
                // Debug.Log("MainMenuManager selected item " + normalizedIndex);
                return true;
            }

            switch (mode)
            {
                case SelectionMode.NEXT:
                    previousIndex = normalizedIndex + 1;
                    break;
                case SelectionMode.PREVIOUS:
                    previousIndex = normalizedIndex - 1;
                    break;
                case SelectionMode.NONE:
                default:
                    return false;
            }
            remainingTries--;
        }
        return false;
    }

    public bool selectNext()
    {
        // Debug.Log("MainMenuManager::selectNext");
        return selectItem(_currentIndex + 1, SelectionMode.NEXT);
    }

    public bool selectPrevious()
    {
        // Debug.Log("MainMenuManager::selectPrevious");
        return selectItem(_currentIndex - 1, SelectionMode.PREVIOUS);
    }

    public void onHover(MainMenuItem item)
    {
        // Debug.Log(string.Format("onHover({0})", item._itemName));
        Logger.Log(item.itemName + " onHover", Logger.Level.DEBUG);
        selectItem(item.itemName);
    }

    public static void replaceTextBy(string target, string replacement, MainMenuItem[] items, string debug = "")
    {
        // Debug.Log(string.Format("replaceTextBy({0}, {1}, {2}, {3})", target, replacement, MainMenuItemArray.ToString(items), debug));
        for (int index = 0; index < items.Length; index++)
        {
            if (items[index].itemName == target)
            {
                items[index].itemName = replacement;
                MainMenuManager.redraw(items);
                return;
            }
        }
        Debug.LogWarning("MainMenuManager::MainMenuItem::replaceTextBy static " + debug + " FAIL with target=" + target + " and replacement=" + replacement);
    }

    private void replaceTextBy(string target, string replacement, string debug = "")
    {
        // Debug.Log(string.Format("replaceTextBy({0}, {1}, {2})", target, replacement, debug));
        MainMenuManager.replaceTextBy(target, replacement, _items, debug);
    }

    public void setNewGame()
    {
        // Debug.Log("MainMenuManager::setNewGame");
        replaceTextBy(resumeKey, newGameKey, "setNewGame");
        setVisibility(restartKey, false);
    }

    public void setResume()
    {
        // Debug.Log("MainMenuManager::setResume");
        replaceTextBy(newGameKey, resumeKey, "setResume");
        setVisibility(restartKey, true);
    }

    private void setVisibility(string itemKey, bool isVisible)
    {
        // Debug.Log(string.Format("setVisibility({0},{1})", itemKey, isVisible.ToString()));
        setVisibility(_items, itemKey, isVisible, "MainMenuManager");
    }

    public static void setVisibility(MainMenuItem[] items, string itemKey, bool isVisible, string debug = null, float spacing = defaultVerticalSpacing)
    {
        // Debug.Log(string.Format("setVisibility({0},{1},{2},{3},{4})", MainMenuItemArray.ToString(items), itemKey, isVisible.ToString(), debug, spacing.ToString()));
        if (!string.IsNullOrEmpty(debug))
        {
            //Debug.Log("MainMenuManager::setVisibility(items, "+itemKey+", "+isVisible+", "+debug+", "+spacing);
        }
        for (int index = 0; index < items.Length; index++)
        {
            items[index].initializeIfNecessary();
            if (items[index].itemName == itemKey)
            {
                items[index].displayed = isVisible;
                if (!string.IsNullOrEmpty(debug))
                {
                    //Debug.Log("MainMenuManager::setVisibility "+debug+" found "+itemKey+" and set its visibility to "+isVisible);
                }
                break;
            }
            else if (!string.IsNullOrEmpty(debug))
            {
                //Debug.Log("MainMenuManager::setVisibility "+debug+": '"+itemKey+"'≠'"+items[index].itemName+"'");
            }
        }
        MainMenuManager.redraw(items, debug, spacing);
    }

    public static void redraw(MainMenuItem[] items, string debug = null, float spacing = defaultVerticalSpacing)
    {
        // Debug.Log(string.Format("redraw({0}, {1}, {2})", MainMenuItemArray.ToString(items), debug, spacing.ToString()));
        if (!string.IsNullOrEmpty(debug))
        {
            //Debug.Log("MainMenuManager::redraw "+debug);
        }
        if (items.Length != 0)
        {
            Vector2 nextRelativeOffset = items[0].anchor.relativeOffset;
            foreach (MainMenuItem item in items)
            {
                item.gameObject.SetActive(item.displayed);
                if (!string.IsNullOrEmpty(debug))
                {
                    //Debug.Log("MainMenuManager::redraw "+debug+" set "+item.itemName+" activity to "+item.displayed);
                }
                if (item.displayed)
                {
                    item.anchor.relativeOffset = nextRelativeOffset;
                    nextRelativeOffset = new Vector2(nextRelativeOffset.x, nextRelativeOffset.y + spacing);
                }
            }
        }
        else
        {
            Debug.LogWarning("MainMenuManager redraw static no item");
        }
    }

    private void redraw()
    {
        // Debug.Log("MainMenuManager::redraw");
        MainMenuManager.redraw(_items, null, verticalSpacing);
    }

    private MainMenuItemArray[] arrays;
    private bool _initializedArrays = false;
    private void initializeArrays()
    {
        if (!_initializedArrays)
        {
            arrays = new MainMenuItemArray[6] { mainMenuItems, controlItems, languageItems, soundItems, settingsItems, learnMoreItems };
            _initializedArrays = true;
        }
    }

    private void activateArray(MainMenuItemArray toActivate)
    {
        // Debug.Log("activateArray " + toActivate);
        initializeArrays();

        deselect();
        foreach (MainMenuItemArray array in arrays)
        {
            array.gameObject.SetActive(array == toActivate);
            // Debug.Log(array + " == " + toActivate + " = " + (array == toActivate));
        }
        copyItemsFrom(toActivate);
        selectItem(0);
    }

    public void switchTo(MainMenuScreen screen)
    {
        // Debug.Log(string.Format("switchTo({0})", screen.ToString()));
        MainMenuItemArray toActivate = mainMenuItems;
        switch (screen)
        {
            case MainMenuScreen.SETTINGS:
                // Debug.Log("toActivate = " + settingsItems);
                toActivate = settingsItems;
                break;
            case MainMenuScreen.LEARNMOREOPTIONS:
                toActivate = learnMoreItems;
                break;
            case MainMenuScreen.CONTROLS:
                toActivate = controlItems;
                break;
            case MainMenuScreen.LANGUAGES:
                toActivate = languageItems;
                break;
            case MainMenuScreen.SOUNDOPTIONS:
                toActivate = soundItems;
                break;
            case MainMenuScreen.DEFAULT:
            default:
                toActivate = mainMenuItems;
                break;
        }
        activateArray(toActivate);
    }

    private void copyItemsFrom(MainMenuItemArray array)
    {
        // Debug.Log(string.Format("copyItemsFrom({0})", array.ToString()));
        _items = new MainMenuItem[array._items.Length];
        for (int index = 0; index < array._items.Length; index++)
        {
            // Debug.Log(string.Format("_items[{0}]={1}", index.ToString(), array._items[index].ToString()));
            _items[index] = array._items[index];
        }
    }

    public bool escape()
    {
        // Debug.Log("MainMenuManager::escape");
        BackMainMenuItem bmmi;
        ResumeMainMenuItem rmmi;
        foreach (MainMenuItem item in _items)
        {
            bmmi = item as BackMainMenuItem;
            if (null != bmmi)
            {
                bmmi.click();
                return true;
            }
            else
            {
                rmmi = item as ResumeMainMenuItem;
                if (null != rmmi)
                {
                    if (resumeKey == rmmi.itemName)
                    {
                        rmmi.click();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        GameStateController.get().leaveMainMenu();
        return true;
    }

    public void open()
    {
        // Debug.Log("MainMenuManager::open");
        GUITransitioner.showGraphs(false, GUITransitioner.GRAPH_HIDER.MAINMENU);
        gameObject.SetActive(true);
        switchTo(MainMenuScreen.DEFAULT);
        cullingMaskHandler.showMainMenu(true);
    }

    public void close()
    {
        // Debug.Log("MainMenuManager::close");
        GUITransitioner.showGraphs(true, GUITransitioner.GRAPH_HIDER.MAINMENU);
        this.gameObject.SetActive(false);
        cullingMaskHandler.showMainMenu(false);
    }
}

using UnityEngine;


public class MainMenuManager : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "MainMenuManager";
    private static MainMenuManager _instance;
    [SerializeField]
    private bool _first = true;
    [SerializeField]
    private CullingMaskHandler cullingMaskHandler;

    public static MainMenuManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("MainMenuManager get was badly initialized");

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

    private MainMenuItemArray _current;

    [SerializeField]
    private MainMenuItemArray mainMenuItems;
    [SerializeField]
    private MainMenuItemArray settingsItems;
    [SerializeField]
    private MainMenuItemArray controlItems;
    [SerializeField]
    private MainMenuItemArray soundItems;
    [SerializeField]
	private MainMenuItemArray chapterSelectionItems;
    [SerializeField]
    private LearnMoreOptionsMainMenuItemArray learnMoreItems;

    [SerializeField]
    private ParticleSystemFeedback _feedback;
    [SerializeField]
    private ResumeMainMenuItem _rmmi;
    private Vector3 _shift = new Vector3(0, 0, -0.4f);

    private const float _verticalSpacing = -0.1f;
    private const float _defaultVerticalSpacing = -0.1f;

    private const string _menuKeyPrefix = "MENU.";
    public const string newGameKey = _menuKeyPrefix + "NEWGAME";
    private const string resumeKey = _menuKeyPrefix + "RESUME";
    private const string _restartKey = _menuKeyPrefix + "RESTART";
    public enum MainMenuScreen
    {
        SETTINGS,
        LEARNMOREOPTIONS,
        CONTROLS,
        SOUNDOPTIONS,
		CHAPTERSELECTION,
        DEFAULT
    }

    //_currentIndex == -1 means nothing is selected
    private int _currentIndex = -1;

    private bool isAnItemSelected()
    {
        // Debug.Log(this.GetType() + " isAnItemSelected");
		return _currentIndex >= 0 && _currentIndex < _current._items.Length;
    }

    public MainMenuItem getCurrentItem()
    {
        // Debug.Log(this.GetType() + " getCurrentItem");
        if (isAnItemSelected())
        {
			return _current._items[_currentIndex];
        }
        else
        {
            return null;
        }
    }

    private void deselect()
    {
        // Debug.Log(this.GetType() + " deselect");
        if (isAnItemSelected())
        {
			_current._items[_currentIndex].deselect();
            // Debug.Log(this.GetType() + " deselected item " + _currentIndex);
        }
        else
        {
            // Debug.LogWarning(this.GetType() + " no item selected: current index is " + _currentIndex);
        }
        _currentIndex = -1;
    }

    public void playFeedback(Vector3 position)
    {
        _feedback.burst(position + _shift);
    }

    private bool selectItem(string name)
    {
        // Debug.Log(string.Format("selectItem({0})", name));
		if (isAnItemSelected() && name == _current._items[_currentIndex].itemName)
        {
            // Debug.Log(this.GetType() + " item " + name + " was already selected");
            return true;
        }
        else
        {
			for (int index = 0; index < _current._items.Length; index++)
            {
                if (name == _current._items[index].itemName)
                {
                    deselect();
					_current._items[index].select();
                    _currentIndex = index;
                    // Debug.Log(this.GetType() + " selected item " + index + " via its name '" + name + "'");
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
        int remainingTries = _current ? _current._items.Length: 0;

        while (remainingTries > 0)
        {
			normalizedIndex = previousIndex >= 0 ?
				previousIndex % _current._items.Length
				: (previousIndex % _current._items.Length) + _current._items.Length;
            if (null != _current._items[normalizedIndex] && _current._items[normalizedIndex].isDisplayed)
            {
                deselect();
                _current._items[normalizedIndex].select();
                _currentIndex = normalizedIndex;
                // Debug.Log(this.GetType() + " selected item " + normalizedIndex);
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
        // Debug.Log(this.GetType() + " selectNext");
        return selectItem(_currentIndex + 1, SelectionMode.NEXT);
    }

    public bool selectPrevious()
    {
        // Debug.Log(this.GetType() + " selectPrevious");
        return selectItem(_currentIndex - 1, SelectionMode.PREVIOUS);
    }

    public void onHover(MainMenuItem item)
    {
        // Debug.Log(string.Format("onHover({0})", item._itemName));
        // Debug.Log(this.GetType() + " " + item.itemName + " onHover");
        selectItem(item.itemName);
    }

    public static void replaceTextBy(string target, string replacement, MainMenuItemArray current, string debug = "")
    {
        // Debug.Log(string.Format("replaceTextBy({0}, {1}, {2}, {3})", target, replacement, MainMenuItemArray.ToString(items), debug));
        for (int index = 0; index < current._items.Length; index++)
        {
			if (current._items[index].itemName == target)
            {
				current._items[index].itemName = replacement;
                MainMenuManager.redraw(current);
                return;
            }
        }
        Debug.LogWarning("MainMenuManager replaceTextBy static " + debug + " FAIL with target=" + target + " and replacement=" + replacement);
    }

    private void replaceTextBy(string target, string replacement, string debug = "")
    {
        // Debug.Log(string.Format("replaceTextBy({0}, {1}, {2})", target, replacement, debug));
        MainMenuManager.replaceTextBy(target, replacement, _current, debug);
    }

    public void setNewGame()
    {
        // Debug.Log(this.GetType() + " setNewGame");
        replaceTextBy(resumeKey, newGameKey, "setNewGame");
        setVisibility(_restartKey, false);
    }

    public void setResume()
    {
        // Debug.Log(this.GetType() + " setResume");
        replaceTextBy(newGameKey, resumeKey, "setResume");
        setVisibility(_restartKey, true);
    }

    private void setVisibility(string itemKey, bool isVisible)
    {
        // Debug.Log(string.Format("setVisibility({0},{1})", itemKey, isVisible.ToString()));
		setVisibility(_current, itemKey, isVisible, "MainMenuManager");
    }

    public static void setVisibility(MainMenuItemArray current, string itemKey, bool isVisible, string debug = null, float spacing = _defaultVerticalSpacing)
    {
        // Debug.Log(string.Format("setVisibility({0},{1},{2},{3},{4})", MainMenuItemArray.ToString(items), itemKey, isVisible.ToString(), debug, spacing.ToString()));
        if (!string.IsNullOrEmpty(debug))
        {
            // Debug.Log(this.GetType() + " setVisibility(items, "+itemKey+", "+isVisible+", "+debug+", "+spacing);
        }
		for (int index = 0; index < current._items.Length; index++)
        {
			current._items[index].initializeIfNecessary();
            if (current._items[index].itemName == itemKey)
            {
				current._items[index].isDisplayed = isVisible;
                if (!string.IsNullOrEmpty(debug))
                {
                    // Debug.Log(this.GetType() + " setVisibility "+debug+" found "+itemKey+" and set its visibility to "+isVisible);
                }
                break;
            }
            else if (!string.IsNullOrEmpty(debug))
            {
                // Debug.Log(this.GetType() + " setVisibility "+debug+": '"+itemKey+"'≠'"+items[index].itemName+"'");
            }
        }
        MainMenuManager.redraw(current, debug, spacing);
    }

	public static void redraw(MainMenuItemArray current, string debug = null, float spacing = _defaultVerticalSpacing)
    {
		if (current == MainMenuManager.get().chapterSelectionItems)
			return;
        // Debug.Log(string.Format("redraw({0}, {1}, {2})", MainMenuItemArray.ToString(items), debug, spacing.ToString()));
        if (!string.IsNullOrEmpty(debug))
        {
            // Debug.Log(this.GetType() + " redraw "+debug);
        }
		if (current._items.Length != 0)
        {
			Vector2 nextRelativeOffset = current._items[0].anchor.relativeOffset;
			foreach (MainMenuItem item in current._items)
            {
                item.gameObject.SetActive(item.isDisplayed);
                if (!string.IsNullOrEmpty(debug))
                {
                    // Debug.Log(this.GetType() + " redraw "+debug+" set "+item.itemName+" activity to "+item.displayed);
                }
                if (item.isDisplayed)
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
        // Debug.Log(this.GetType() + " redraw");
        MainMenuManager.redraw(_current, null, _verticalSpacing);
    }

    private MainMenuItemArray[] arrays;
    private bool _initializedArrays = false;
    private void initializeArrays()
    {
        if (!_initializedArrays)
        {
            arrays = new MainMenuItemArray[6] { mainMenuItems, controlItems, chapterSelectionItems, soundItems, settingsItems, learnMoreItems };
            _initializedArrays = true;
        }
    }

    private void activateArray(MainMenuItemArray toActivate)
    {
        // Debug.Log(this.GetType() + " activateArray " + toActivate);
        initializeArrays();

        deselect();
        foreach (MainMenuItemArray array in arrays)
        {
            array.gameObject.SetActive(array == toActivate);
//            Debug.Log(array + " == " + toActivate + " = " + (array == toActivate));
        }
		_current = toActivate;
        selectItem(toActivate.itemToActivateFirst);
    }

    public void switchTo(MainMenuScreen screen)
    {
        // Debug.Log(string.Format("switchTo({0})", screen.ToString()));
        MainMenuItemArray toActivate = mainMenuItems;
        switch (screen)
        {
            case MainMenuScreen.SETTINGS:
                // Debug.Log(this.GetType() + " toActivate = " + settingsItems);
                toActivate = settingsItems;
                break;
            case MainMenuScreen.LEARNMOREOPTIONS:
                toActivate = learnMoreItems;
                break;
            case MainMenuScreen.CONTROLS:
                toActivate = controlItems;
                break;
            case MainMenuScreen.SOUNDOPTIONS:
                toActivate = soundItems;
                break;
			case MainMenuScreen.CHAPTERSELECTION:
				toActivate = chapterSelectionItems;
				break;
            case MainMenuScreen.DEFAULT:
            default:
                toActivate = mainMenuItems;
                break;
        }
        activateArray(toActivate);
    }
	/*
    private void copyItemsFrom(MainMenuItemArray array)
    {
        // Debug.Log(string.Format("copyItemsFrom({0})", array.ToString()));
        _items = new MainMenuItem[array._items.Length];
        for (int index = 0; index < array._items.Length; index++)
        {
            // Debug.Log(string.Format("_items[{0}]={1}", index.ToString(), array._items[index].ToString()));
            _items[index] = array._items[index];
        }
    }*/

    public bool escape()
    {
        // Debug.Log(this.GetType() + " escape");
        BackMainMenuItem bmmi;
        foreach (MainMenuItem item in _current._items)
        {
            bmmi = item as BackMainMenuItem;
            if (null != bmmi)
            {
                bmmi.click();
                return true;
            }
        }
        
        if (null != _rmmi && _rmmi.gameObject.activeInHierarchy)
        {
            if (!_rmmi.isStart())
            {
                _rmmi.click();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        // Debug.Log(this.GetType() + " neither 'resume' nor 'back' button found");
        GameStateController.get().leaveMainMenu();
        return true;
    }

    public void open()
    {
        // Debug.Log(this.GetType() + " open");
        GUITransitioner.showGraphs(false, GUITransitioner.GRAPH_HIDER.MAINMENU);
        gameObject.SetActive(true);
		#if CHAPTER_SELECT_BUILD
		switchTo(MainMenuScreen.CHAPTERSELECTION);
		#else
        switchTo(MainMenuScreen.DEFAULT);
		#endif
        cullingMaskHandler.showMainMenu(true);
    }

    public void close()
    {
        // Debug.Log(this.GetType() + " close");
        GUITransitioner.showGraphs(true, GUITransitioner.GRAPH_HIDER.MAINMENU);
        this.gameObject.SetActive(false);
        cullingMaskHandler.showMainMenu(false);
    }
}

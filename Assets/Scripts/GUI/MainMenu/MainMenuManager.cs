using UnityEngine;
using System.Collections.Generic;

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
            // default: has "Science" and "Quit" options
            // Arcade: neither
            // WebGL: no "Quit"
#if UNITY_EDITOR
            // hide QUIT
            // mainMenuItems.hideIndexes(new List<int>{ 6 });
#elif ARCADE
            // hide Settings, Science and Quit
            mainMenuItems.hideIndexes(new List<int> { 4, 5, 6 });
#elif UNITY_WEBGL
            // hide QUIT
		    mainMenuItems.hideIndexes(new List<int>{ 6 });
#endif
            _initialized = true;
        }
    }

    // Use this for initialization
    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        if (null != _current)
        {
            selectItem(_current.itemToActivateFirst);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public delegate void BasicEvent();
    public static event BasicEvent onMenuSelectItem;
    private MainMenuItemArray _current;
    [SerializeField]
    private MainMenuItemArray mainMenuItems;
    [SerializeField]
    private ChapterSelectionMainMenuItemArray chapterSelectionItems;
    // settings submenu
    [SerializeField]
    private SettingsMainMenuItemArray settingsItems;
    [SerializeField]
    private GraphicsOptionsMainMenuItemArray graphicsItems;
    [SerializeField]
    private SoundOptionsMainMenuItemArray soundItems;
    [SerializeField]
    private ControlsMainMenuItemArray controlItems;
    // science submenu
    [SerializeField]
    private ScienceMainMenuItemArray scienceItems;
    [SerializeField]
    private ContributeMainMenuItemArray contributeItems;
    [SerializeField]
    private LearnMoreOptionsMainMenuItemArray learnMoreItems;
	// quit submenu
	[SerializeField]
	private QuitMainMenuItemArray quitItems;

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
        CHAPTERSELECTION,
        SETTINGS,
        CONTROLS,
        SOUNDOPTIONS,
        GRAPHICSOPTIONS,
        SCIENCE,
        CONTRIBUTE,
        LEARNMOREOPTIONS,
		QUIT,
        DEFAULT,
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

    private enum SelectionMode
    {
        NEXT,
        PREVIOUS,
        NONE
    }

    private bool innerSelectItem(int normalizedIndex)
    {
        // Debug.Log(this.GetType() + " innerSelectItem " + normalizedIndex);
        if (null != _current._items[normalizedIndex])
        {
            deselect();
            _current._items[normalizedIndex].select();
            _currentIndex = normalizedIndex;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool selectItem(MainMenuItem item)
    {
        if (null != item)
        {
            for (int index = 0; index < _current._items.Length; index++)
            {
                if (item == _current._items[index])
                {
                    return innerSelectItem(index);
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private bool selectItem(int index, SelectionMode mode = SelectionMode.NEXT)
    {
        // Debug.Log(string.Format("selectItem({0}, {1})", index, mode.ToString()));
        int previousIndex = index;
        int normalizedIndex;
        int remainingTries = _current ? _current._items.Length : 0;

        while (remainingTries > 0)
        {
            normalizedIndex = previousIndex >= 0 ?
                previousIndex % _current._items.Length
                : (previousIndex % _current._items.Length) + _current._items.Length;
            if (null != _current._items[normalizedIndex] && _current._items[normalizedIndex].isDisplayed)
            {
                // Debug.Log(this.GetType() + " selected item " + normalizedIndex);
                return innerSelectItem(normalizedIndex);
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
        onMenuSelectItem();
        return selectItem(_currentIndex + 1, SelectionMode.NEXT);
    }

    public bool selectPrevious()
    {
        // Debug.Log(this.GetType() + " selectPrevious");
        onMenuSelectItem();
        return selectItem(_currentIndex - 1, SelectionMode.PREVIOUS);
    }

    public void onHover(MainMenuItem item)
    {
        // Debug.Log(string.Format("onHover({0})", item._itemName));
        // Debug.Log(this.GetType() + " " + item.itemName + " onHover");
        selectItem(item);
    }

    public static void replaceTextBy(string target, string replacement, MainMenuItemArray current, bool all = false, string debug = "")
    {
        // Debug.Log(string.Format("replaceTextBy({0}, {1}, {2}, {3})", target, replacement, MainMenuItemArray.ToString(items), debug));
        bool found = false;
        for (int index = 0; index < current._items.Length; index++)
        {
            if (current._items[index].itemName == target)
            {
                found = true;
                current._items[index].itemName = replacement;
                MainMenuManager.redraw(current);
                if (!all)
                {
                    return;
                }
            }
        }
        if (!found)
        {
            Debug.LogWarning("MainMenuManager replaceTextBy static " + debug + " FAIL with target=" + target + " and replacement=" + replacement);
        }
    }

    private void replaceTextBy(string target, string replacement, bool all = false, string debug = "")
    {
        // Debug.Log(string.Format("replaceTextBy({0}, {1}, {2})", target, replacement, debug));
        MainMenuManager.replaceTextBy(target, replacement, _current, all, debug);
    }

    public void setNewGame()
    {
        // Debug.Log(this.GetType() + " setNewGame");
        replaceTextBy(resumeKey, newGameKey, false, "setNewGame");
        setVisibility(_restartKey, false);
    }

    public void setResume()
    {
        // Debug.Log(this.GetType() + " setResume");
        replaceTextBy(newGameKey, resumeKey, false, "setResume");
        setVisibility(_restartKey, true);
    }

    private void setVisibility(string itemKey, bool isVisible, bool all = false)
    {
        // Debug.Log(string.Format("setVisibility({0},{1})", itemKey, isVisible.ToString()));
        setVisibility(_current, itemKey, isVisible, all, "MainMenuManager");
    }

    public static void setVisibility(MainMenuItemArray current, string itemKey, bool isVisible, bool all = false, string debug = null, float spacing = _defaultVerticalSpacing)
    {
        // Debug.Log(string.Format("setVisibility({0},{1},{2},{3},{4})", MainMenuItemArray.ToString(items), itemKey, isVisible.ToString(), debug, spacing.ToString()));
        bool found = false;
        if (!string.IsNullOrEmpty(debug))
        {
            // Debug.Log(this.GetType() + " setVisibility(items, "+itemKey+", "+isVisible+", "+debug+", "+spacing);
        }
        for (int index = 0; index < current._items.Length; index++)
        {
            current._items[index].initializeIfNecessary();
            if (current._items[index].itemName == itemKey)
            {
                found = true;
                current._items[index].isDisplayed = isVisible;
                // if (!string.IsNullOrEmpty(debug))
                // {
                //     Debug.Log("MainMenuManager setVisibility "+debug+" found "+itemKey+" and set its visibility to "+isVisible);
                // }
                if (!all)
                {
                    break;
                }
            }
            // else if (!string.IsNullOrEmpty(debug))
            // {
            //     Debug.Log("MainMenuManager setVisibility "+debug+": '"+itemKey+"'≠'"+current._items[index].itemName+"'");
            // }
        }
        if (!found)
        {
            Debug.LogWarning("MainMenuManager setVisibility static " + debug + " FAIL with target=" + itemKey + " and isVisible=" + isVisible);
        }
        MainMenuManager.redraw(current, debug, spacing);
    }

    public static void redraw(MainMenuItemArray current, string debug = null, float spacing = _defaultVerticalSpacing)
    {
        if (current != MainMenuManager.get().chapterSelectionItems)
        {
            // Debug.Log(string.Format("redraw({0}, {1}, {2})", MainMenuItemArray.ToString(items), debug, spacing.ToString()));
            // if (!string.IsNullOrEmpty(debug))
            // {
            //     Debug.Log(" redraw " + debug);
            // }
            if (current._items.Length != 0)
            {
                if (current._items[0].anchor)
                {
                    Vector2 nextRelativeOffset = current._items[0].anchor.relativeOffset;
                    foreach (MainMenuItem item in current._items)
                    {
                        item.gameObject.SetActive(item.isDisplayed);
                        // if (!string.IsNullOrEmpty(debug))
                        // {
                        //     Debug.Log("redraw " + debug + " set " + item.itemName + " activity to " + item.displayed);
                        // }
                        if (item.isDisplayed)
                        {
                            item.anchor.relativeOffset = nextRelativeOffset;
                            nextRelativeOffset = new Vector2(nextRelativeOffset.x, nextRelativeOffset.y + spacing);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("MainMenuManager redraw static no item");
            }
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
            arrays = new MainMenuItemArray[10] {
                mainMenuItems,
                chapterSelectionItems,
                settingsItems,
                graphicsItems,
                soundItems,
                controlItems,
                scienceItems,
                contributeItems,
                learnMoreItems,
				quitItems,
                };
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
            // Debug.Log(array + " == " + toActivate + " = " + (array == toActivate));
        }
        _current = toActivate;
        selectItem(toActivate.itemToActivateFirst);
        // Debug.Log(this.GetType() + " activateArray selectItem " + toActivate.itemToActivateFirst);
    }

    public void switchTo(MainMenuScreen screen)
    {
        // Debug.Log(this.GetType() + " switchTo(" + screen.ToString() + ")");
        MainMenuItemArray toActivate = mainMenuItems;
        switch (screen)
        {
            case MainMenuScreen.CHAPTERSELECTION:
                toActivate = chapterSelectionItems;
                break;
            // settings submenu
            case MainMenuScreen.SETTINGS:
                toActivate = settingsItems;
                break;
            case MainMenuScreen.GRAPHICSOPTIONS:
                toActivate = graphicsItems;
                break;
            case MainMenuScreen.SOUNDOPTIONS:
                toActivate = soundItems;
                break;
            case MainMenuScreen.CONTROLS:
                toActivate = controlItems;
                break;
            // science submenu
            case MainMenuScreen.SCIENCE:
                toActivate = scienceItems;
                break;
            case MainMenuScreen.CONTRIBUTE:
                // Debug.Log(this.GetType() + " switchTo #contributeItems=" + contributeItems._items.Length);
                toActivate = contributeItems;
                break;
            case MainMenuScreen.LEARNMOREOPTIONS:
                toActivate = learnMoreItems;
                break;
			case MainMenuScreen.QUIT:
				toActivate = quitItems;
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
        ArcadeManager.instance.playAnimation(ArcadeManager.Animation.gui_tactile_start);
    }

    public void close()
    {
        ArcadeManager.instance.playAnimation(ArcadeManager.Animation.gui_tactile_end);
        // Debug.Log(this.GetType() + " close");
        GUITransitioner.showGraphs(true, GUITransitioner.GRAPH_HIDER.MAINMENU);
        this.gameObject.SetActive(false);
        cullingMaskHandler.showMainMenu(false);
    }
}
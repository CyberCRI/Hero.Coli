// #define QUICKTEST

using UnityEngine;
using System.Collections.Generic;

//TODO: merge with ModalManager
public class InfoWindowManager : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "InfoWindowManager";
    private static InfoWindowManager _instance;
    public static InfoWindowManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("InfoWindowManager get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<InfoWindowManager>();
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
            loadDataIntoDico(inputFiles, _loadedInfoWindows);
            _initialized = true;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public string[] inputFiles;

    public UILocalize titleLabel;
    public UILocalize subtitleLabel;
    public UILocalize explanationLabel;
    public UILocalize bottomLabel;
    public NextAction nextAction;

    public GameObject infoPanel;
    public UISprite infoSprite;

    public GameStateController gameStateController;

    private Dictionary<string, StandardInfoWindowInfo> _loadedInfoWindows = new Dictionary<string, StandardInfoWindowInfo>();
    private const string _genericPrefix = "INFO.";
    private const string _genericTitle = ".TITLE";
    private const string _genericSubtitle = ".SUBTITLE";
    private const string _genericExplanation = ".EXPLANATION";
    private const string _genericBottom = ".BOTTOM";

    public enum NextAction
    {
        GOTOWORLD,
        GOTOEQUIP,
        GOTOCRAFT,
        RBS2CRAFTTUTORIAL
    }

    private Dictionary<string, NextAction> _actions = new Dictionary<string, NextAction>(){
    {InfoWindowXMLTags.WORLD, NextAction.GOTOWORLD},
    {InfoWindowXMLTags.EQUIP, NextAction.GOTOEQUIP},
    {InfoWindowXMLTags.CRAFT, NextAction.GOTOCRAFT},
    {InfoWindowXMLTags.RBS2CRAFTTUTORIAL, NextAction.RBS2CRAFTTUTORIAL}
  };

    public static bool hasActivePanel()
    {
        return _instance.infoPanel.activeInHierarchy;
    }

    public static bool displayInfoWindow(string code)
    {
#if QUICKTEST
        return true;
#endif
        if (fillInFieldsFromCode(code))
        {
            ModalManager.setModal(_instance.infoPanel);
            return true;
        }
        else
        {
            Debug.LogWarning("InfoWindowManager displayInfoWindow(" + code + ") failed");
            return false;
        }
    }

    private static bool fillInFieldsFromCode(string code)
    {

        StandardInfoWindowInfo info = retrieveFromDico(code);

        if (null != info)
        {
            string generic = _genericPrefix + code.ToUpper();

            _instance.titleLabel.key = generic + _genericTitle;
            _instance.subtitleLabel.key = generic + _genericSubtitle;
            _instance.infoSprite.spriteName = info._texture;
            _instance.explanationLabel.key = generic + _genericExplanation;
            _instance.bottomLabel.key = generic + _genericBottom;
            _instance.nextAction = getFromString(info._next);

            return true;
        }
        else
        {
            return false;
        }

        // TODO manage onNext
    }

    private static StandardInfoWindowInfo retrieveFromDico(string code)
    {
        StandardInfoWindowInfo info;
        if (!_instance._loadedInfoWindows.TryGetValue(code, out info))
        {
            Debug.LogWarning("InfoWindowManager retrieveFromDico(" + code + ") failed");
            info = null;
        }
        return info;
    }

    private void loadDataIntoDico(string[] inputFiles, Dictionary<string, StandardInfoWindowInfo> dico)
    {

        InfoWindowLoader iwLoader = new InfoWindowLoader();

        string loadedFiles = "";

        foreach (string file in inputFiles)
        {
            foreach (StandardInfoWindowInfo info in iwLoader.loadInfoFromFile(file))
            {
                dico.Add(info._code, info);
            }
            if (!string.IsNullOrEmpty(loadedFiles))
            {
                loadedFiles += ", ";
            }
            loadedFiles += file;
        }

        // Debug.Log(this.GetType() + " loadDataIntoDico loaded "+loadedFiles);
    }

    public static void next()
    {
        // Debug.Log("InfoWindowManager next()");
        ModalManager.unsetModal();
        _instance.gameStateController.tryUnlockPause();

        switch (_instance.nextAction)
        {
            case NextAction.GOTOWORLD:
                // Debug.Log("InfoWindowManager next GOTOWORLD");
                break;
            case NextAction.GOTOEQUIP:
                // Debug.Log("InfoWindowManager next GOTOEQUIP");
                GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen2);
                break;
            case NextAction.GOTOCRAFT:
                // Debug.Log("InfoWindowManager next GOTOCRAFT");
                GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
                break;
            case NextAction.RBS2CRAFTTUTORIAL:
                // Debug.Log("InfoWindowManager next RBS2CRAFTTUTORIAL");
                Hero.get().gameObject.AddComponent<RBS2CraftHint>();
                break;
            default:
                // Debug.Log("InfoWindowManager next GOTOWORLD");
                break;
        }
    }

    public static NextAction getFromString(string next)
    {
        NextAction action;
        _instance._actions.TryGetValue(next, out action);
        return action;
    }

    public static GameStateTarget manageKeyPresses()
    {
        if (Input.GetKeyDown(KeyCode.Escape)
           || Input.GetKeyDown(KeyCode.Space)
           || Input.GetKeyDown(KeyCode.Return)
           || Input.GetKeyUp(KeyCode.KeypadEnter)
          )
        {
            // Debug.Log("InfoWindowManager manageKeyPresses() - key pressed");
            next();
            return GameStateTarget.NoTarget;
        }
        return GameStateTarget.NoAction;
    }
}


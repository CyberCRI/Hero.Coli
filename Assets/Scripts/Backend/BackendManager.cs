// #define FORCEADMIN
// #define FORCENOTADMIN

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if ARCADE
using System.Collections.Generic;
using System.IO.Ports;
#endif

// admin mode unlocks actions and logs to a test GUID
public class BackendManager : MonoBehaviour
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "BackendManager";
    private static BackendManager _instance;
    public static BackendManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("BackendManager get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<BackendManager>();
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
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    public void initializeIfNecessary(GameObject adminTools, GameObject genericAdminButton, Dropdown dropdown)
    {
        if (!_initialized)
        {
            _initialized = true;
            _adminTools = adminTools;
            _genericAdminButton = genericAdminButton;
            _debugDropdown = dropdown;
#if ARCADE
            _debugDropdown.onValueChanged.AddListener(delegate
            {
                switchPort();
            });
#endif
            showAdminTools(GameConfiguration.isAdmin);
        }
    }

#if ARCADE
    private void switchPort()
    {
        ArcadeManager.instance.switchPort(_dynamicPorts[_debugDropdown.value]);
    }
#endif

    void Start()
    {
#if FORCEADMIN
        setAdmin(true);
#elif FORCENOTADMIN
        setAdmin(false);
#endif
        // Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////


    [SerializeField]
    private bool _forceAdmin = false;
    [SerializeField]
    private int _layer;
    private GameObject _adminTools, _genericAdminButton;
    private int _depth = 0;
    private const float _duration = 2.0f;
    private string[] _dynamicPorts;
    [SerializeField]
    private static bool _isHurtLightEffectsOn = false;
    public static bool isHurtLightEffectsOn
    {
        get
        {
            return _isHurtLightEffectsOn;
        }
    }
    [SerializeField]
    private Dropdown _debugDropdown;

    private void showAdminTools(bool doShow)
    {
        if (null != _adminTools)
        {
            // Debug.Log(this.GetType() + " adminTools SetActive " + doShow);
            _adminTools.gameObject.SetActive(doShow);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " adminTools is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //logging mode: to test or to production RedMetrics version
        if (_forceAdmin)
        {
            setAdmin(true);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            switchAdmin();
        }
        else if (GameConfiguration.isAdmin)
        {
            // not managed here: search for 'GameConfiguration.isAdmin'
            // - teleportation
            // - chemicals' concentrations
            // - life and energy
            // - BlackLight (deprecated)

            // to instantly change the language
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                swapLanguage();
            }

            else if (Input.GetKeyDown(KeyCode.Comma))
            {
                // to be able to go over or under obstacles
                moveCharacterDown();
            }
            else if (
              (Input.GetKeyDown(KeyCode.Period) && I18n.getCurrentLanguage() == I18n.Language.English)
              ||
              (Input.GetKeyDown(KeyCode.Semicolon) && I18n.getCurrentLanguage() == I18n.Language.French)
            )
            {
                moveCharacterUp();
            }
            else if (
              (Input.GetKeyDown(KeyCode.Slash) && I18n.getCurrentLanguage() == I18n.Language.English)
              ||
              (Input.GetKeyDown(KeyCode.Exclaim) && I18n.getCurrentLanguage() == I18n.Language.French)
            )
            {
                resetCharacterVerticalPosition();
            }

            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
            {
                // unlock everything
                unlockAll();
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            {
                displayPorts();
            }

            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Backspace))
            {
                // to erase all info in PlayerPrefs
                resetPlayerPrefs();
            }

            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                // to end the game instantly, displaying stats and validating current chapter
                endGame();
            }
        }
    }

    private void switchAdmin()
    {
        setAdmin(!GameConfiguration.isAdmin);
    }

    private void setAdmin(bool _setAdmin)
    {
        Debug.Log(this.GetType() + " setAdmin(" + _setAdmin + ")");
        _forceAdmin = false;
        if (GameConfiguration.isAdmin != _setAdmin)
        {
            GameConfiguration.isAdmin = _setAdmin;
        }

        Debug.Log(this.GetType() + " setAdmin showAdminTools");
        showAdminTools(GameConfiguration.isAdmin);

        //display feedback for logging mode
        string msg = GameConfiguration.isAdmin ? "REDMETRICS ADMIN MODE" : "REDMETRICS DEFAULT MODE";
        StartCoroutine(waitAndDestroy(createMessage(msg)));
    }

    IEnumerator waitAndDestroy(GameObject go)
    {
        yield return new WaitForSeconds(_duration);
        GameObject.Destroy(go);
    }

    public GameObject createMessage(string msg)
    {
        GameObject go = new GameObject();
        GUIText guiText = go.AddComponent<GUIText>();
        go.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
        guiText.text = msg;
        return go;
    }

    private void unlockAll()
    {
        Debug.Log(this.GetType() + " unlockAll");
        GameStateController.unlockAll();
    }

    private void displayPorts()
    {
        Debug.Log(this.GetType() + " displayPorts");
#if ARCADE
        _debugDropdown.ClearOptions();
        _dynamicPorts = getPortNames();
        _debugDropdown.AddOptions(new List<string>(_dynamicPorts));
#endif
    }

#if ARCADE
    // cf http://answers.unity3d.com/questions/643078/serialportsgetportnames-error.html
    private string[] getPortNames()
    {
        Debug.Log(this.GetType() + " getPortNames");

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Debug.Log(this.GetType() + " getPortNames not unix");
        return SerialPort.GetPortNames();

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_STANDALONE_LINUX
        Debug.Log(this.GetType() + " getPortNames UNIX");
        List<string> serial_ports = new List<string>();
        string[] ttys = System.IO.Directory.GetFiles("/dev/", "tty*");
        return ttys;
#endif
    }
#endif

    private void resetPlayerPrefs()
    {
        Debug.Log(this.GetType() + " resetPlayerPrefs");
        // ?
        MemoryManager.get().configuration.furthestChapter = 0;
        GameStateController.get().resetPlayerPrefsAndRestart();
    }

    private void endGame()
    {
        Debug.Log(this.GetType() + " endGame");
        GameStateController.get().triggerEnd();
    }

    private void swapLanguage()
    {
        Debug.Log(this.GetType() + " swapLanguage");

        // language swap
        I18n.Language newLanguage = (I18n.Language.English == I18n.getCurrentLanguage()) ? I18n.Language.French : I18n.Language.English;
        if (I18n.changeLanguageTo(newLanguage))
        {
            RedMetricsManager.get().sendEvent(TrackingEvent.ADMINCONFIGURE, new CustomData(CustomDataTag.LANGUAGE, I18n.getCurrentLanguage().ToString()));
        }
    }

    private void logObjectsOfLayer(int layer = int.MinValue)
    {
        Debug.Log(this.GetType() + " logObjectsOfLayer");

        int usedLayer = int.MinValue == layer ? _layer : layer;

        // layer detection
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gos)
        {
            if (go.layer == usedLayer)
            {
                Debug.Log(go.name);
            }
        }
    }

    private void resetCharacterVerticalPosition()
    {
        Debug.Log(this.GetType() + " resetCharacterVerticalPosition");
        Vector3 position = Character.get().transform.position;
        Character.get().transform.position = new Vector3(position.x, position.y - _depth, position.z);
        _depth = 0;
    }

    private void moveCharacterUp()
    {
        Debug.Log(this.GetType() + " moveCharacterUp");
        Vector3 position = Character.get().transform.position;
        Character.get().transform.position = new Vector3(position.x, position.y - 1, position.z);
        _depth--;
    }

    private void moveCharacterDown()
    {
        Debug.Log(this.GetType() + " moveCharacterDown");
        Vector3 position = Character.get().transform.position;
        Character.get().transform.position = new Vector3(position.x, position.y + 1, position.z);
        _depth++;
    }

    private GameObject[] _testGameObjects;
    private Behaviour _listener;

    public void setTestObjects(GameObject[] testGameObjects, Behaviour listener)
    {
        _testGameObjects = testGameObjects;
        _listener = listener;

        foreach (GameObject tgo in _testGameObjects)
        {
            createGOBTestButton(tgo);
        }
        createGOBTestButton(null, listener);
        createAllSpecialButtons();
    }

    private int buttonIndex = 1;
    private void createGOBTestButton(GameObject toActivate, Behaviour behaviour = null)
    {
        GameObject go;
        Button button;
        createButton(out go, out button);
        GameObjectAndBehaviourActivator activator = go.AddComponent<GameObjectAndBehaviourActivator>();
        activator.goToActivate = toActivate;
        activator.behaviourToActivate = behaviour;
        activator.button = button;
        button.onClick.AddListener(() => { activator.click(); });
    }

    private void createButton(out GameObject go, out Button button, string label = null, OnClick onclickdelegate = null)
    {
        go = GameObject.Instantiate(_genericAdminButton);
        button = go.GetComponent<Button>();
        go.transform.SetParent(_adminTools.transform);
        Vector3 position = _genericAdminButton.transform.position;
        if (null != label)
        {
            Text buttonText = go.GetComponentInChildren<Text>();
            if (null != buttonText)
            {
                buttonText.text = label;
            }
        }
        if (null != onclickdelegate)
        {
            button.onClick.AddListener(() => { onclickdelegate(); });
        }
        go.transform.position = new Vector3(position.x, position.y - buttonIndex * 30f, position.z);
        buttonIndex++;
    }

    public delegate void OnClick();
    private void createAllSpecialButtons()
    {
        GameObject go;
        Button button;
        createButton(out go, out button, "unlockAll", unlockAll);
        createButton(out go, out button, "endGame", endGame);
        createButton(out go, out button, "resetPlayerPrefs", resetPlayerPrefs);
        createButton(out go, out button, "moveCharacterDown", moveCharacterDown);
        createButton(out go, out button, "moveCharacterUp", moveCharacterUp);
        createButton(out go, out button, "resetCharacterVerticalPosition", resetCharacterVerticalPosition);
        createButton(out go, out button, "swapLanguage", swapLanguage);

        /*
        swapLanguage();
        moveCharacterDown();
        moveCharacterUp();
        resetCharacterVerticalPosition();
        unlockAll();
        resetPlayerPrefs();
        endGame();
        */
    }
}

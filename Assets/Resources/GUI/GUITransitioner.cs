using UnityEngine;

public class GUITransitioner : MonoBehaviour
{


    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "GUITransitioner";
    private static GUITransitioner _instance;
    public static GUITransitioner get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("GUITransitioner was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<GUITransitioner>();
        }
        return _instance;
    }
    void Awake()
    {
        Debug.Log(this.GetType() + " Awake");
        if((_instance != null) && (_instance != this))
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
        Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    void Start()
    {
        Debug.Log(this.GetType() + " Start");
    }

    private bool _initialized = false;
    public void initialize()
    {
        if (!_initialized)
        {
            _reactionEngine = ReactionEngine.get();
            _devicesDisplayer = DevicesDisplayer.get();

            setScreen2(false);
            setScreen3(false);
            setScreen1(true);

            _devicesDisplayer.UpdateScreen();
            _currentScreen = GameScreen.screen1;

            _initialized = true;
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private ReactionEngine _reactionEngine;

    private bool _pauseTransition = false;
    private float _timeScale = 1.0f;

    //regulates at which speed the game will get back to its original speed when pause is switched off
    private float _unpauseAcceleration = 1f;

    public GameScreen _currentScreen = GameScreen.screen1;
    public enum GameScreen
    {
        screen1,
        screen2,
        screen3
    };


    //public GameObject _mainCameraObject;
    public BoundCamera mainBoundCamera;

    public GameObject worldScreen;
    public GameObject craftScreen;
    private DevicesDisplayer _devicesDisplayer;


    public VectrosityPanel celliaGraph;
    public VectrosityPanel roomGraph;

    public Hero hero;
    public CellControl control;

    private bool _screen1, _screen2, _screen3;

    private void setScreen1(bool active)
    {
        // Debug.Log(this.GetType() + " setScreen1(" + active + ") " + _screen1);
        _screen1 = active;
        if (null != worldScreen && null != craftScreen)
        {
            if (active)
            {
                ZoomOut();
            }
            worldScreen.SetActive(active);
            //_craftScreen.SetActive(!active);
        }
    }

    private void setScreen2(bool active)
    {
        // Debug.Log(this.GetType() + " setScreen2(" + active + ") " + _screen2);
        _screen2 = active;
        if (null != worldScreen && null != craftScreen)
        {
            if (active)
            {
                ZoomIn();
            }
            worldScreen.SetActive(active);
        }
    }

    private void setScreen3(bool active)
    {
        // Debug.Log(this.GetType() + " setScreen3(" + active + ") " + _screen3);
        _screen3 = active;
        if (null != worldScreen && null != craftScreen)
        {
            craftScreen.SetActive(active);
        }
    }

    /*
     * "Defensive programming" method
     * Cannot work during Awake
      private void checkCamera() {
          if(_mainBoundCamera == null) {
              _mainBoundCamera = GameObject.Find ("Main Camera").GetComponent<BoundCamera>() as BoundCamera;
          }
      }
   */

    private void ZoomIn()
    {
        if (null != mainBoundCamera)
        {
            mainBoundCamera.SetZoom(true);
        }
    }

    private void ZoomOut()
    {
        if (null != mainBoundCamera)
        {
            mainBoundCamera.SetZoom(false);
        }
    }


    public enum GRAPH_HIDER
    {
        MAINMENU = 0,
        CUTSCENE = 1,
        ENDGAME = 2,
        VECTROSITYPANEL = 3
    }
    private bool[] _hideGraph = new bool[System.Enum.GetValues(typeof(GRAPH_HIDER)).Length];

    public static void showGraphs(bool show, GRAPH_HIDER hider)
    {
        if (null != _instance)
        {
            // Debug.Log("showGraphs("+show+", "+hider+")");

            bool inactive = false;

            _instance._hideGraph[(int)hider] = !show;

            foreach (bool hide in _instance._hideGraph)
            {
                inactive = (inactive || hide);
            }

            if (null != _instance.roomGraph)
            {
                _instance.roomGraph.show(!inactive);
            }
            if (null != _instance.celliaGraph)
            {
                _instance.celliaGraph.show(!inactive);
            }
        }
    }

    public void Pause(bool pause)
    {
        _pauseTransition = !pause;
        if (_reactionEngine == null)
        {
            _reactionEngine = ReactionEngine.get();
        }
        _reactionEngine.Pause(pause);
        _timeScale = pause ? 0 : 1;
        if (pause)
        {
            Time.timeScale = 0;
        }

        if ((null != roomGraph) && (null != celliaGraph))
        {
            roomGraph.setPause(pause);
            celliaGraph.setPause(pause);
        }
        hero.Pause(pause);
        control.Pause(pause);
        EnemiesManager.Paused = pause;
    }

    public void GoToScreen(GameScreen destination)
    {
        // Debug.Log("GUITransitioner::GoToScreen(" + destination + ")");
        if (destination == GameScreen.screen1)
        {
            if (_currentScreen == GameScreen.screen2)
            {
                // Debug.Log("2->1");
                //2 -> 1
                //set zoom1
                //remove inventory device, deviceID
                //add graphs
                //move devices and potions?

            }
            else if (_currentScreen == GameScreen.screen3)
            {
                // Debug.Log("3->1");
                //3 -> 1
                //set zoom1
                //remove craft screen
                //add graphs
                //add potions
                //add devices
                //add life/energy
                //add medium info
                setScreen3(false);
                setScreen1(true);
            }
            GameStateController.get().tryUnlockPause();
            ZoomOut();
            _currentScreen = GameScreen.screen1;


        }
        else if (destination == GameScreen.screen2)
        {
            if (_currentScreen == GameScreen.screen1)
            {
                // Debug.Log("GUITransitioner::GoToScreen 1->2");
                //1 -> 2
                //set zoom2
                //add inventory device, deviceID
                //remove graphs
                //move devices and potions?
                GameStateController.get().tryLockPause();
            }
            else if (_currentScreen == GameScreen.screen3)
            {
                // Debug.Log("GUITransitioner::GoToScreen 3->2");
                //3 -> 2
                //set zoom2
                //remove craft screen
                //add inventory device, deviceID
                //add potions
                //add devices
                //add life/energy
                //add medium info
                setScreen3(false);
                setScreen2(true);
            }

            ZoomIn();
            _currentScreen = GameScreen.screen2;


        }
        else if (destination == GameScreen.screen3)
        {
            if (_currentScreen == GameScreen.screen1)
            {
                // Debug.Log("GUITransitioner::GoToScreen 1->3");
                //1 -> 3
                //remove everything
                //add device inventory, parameters
                //remove graphs
                //move devices and potions?
                setScreen1(false);
                setScreen3(true);
                GameStateController.get().tryLockPause();

            }
            else if (_currentScreen == GameScreen.screen2)
            {
                // Debug.Log("GUITransitioner::GoToScreen 2->3");
                //2 -> 3
                //remove everything
                //add craft screen
                setScreen2(false);
                setScreen3(true);

            }
            ZoomIn();
            _currentScreen = GameScreen.screen3;

        }
        else
        {
            Debug.LogError("GuiTransitioner::GoToScreen(" + destination + "): error: unmanaged destination");
        }

        _devicesDisplayer.UpdateScreen();
        TooltipManager.displayTooltip();
    }

    public void SwitchScreen(GameScreen alternate1, GameScreen alternate2)
    {
        // Debug.Log("GuiTransitioner::SwitchScreen(" + alternate1 + "," + alternate2 + ")");
        if (_currentScreen == alternate1)
        {
            GoToScreen(alternate2);
        }
        else if (_currentScreen == alternate2)
        {
            GoToScreen(alternate1);
        }
        else
        {
            Debug.LogError("GuiTransitioner::SwitchScreen(" + alternate1 + "," + alternate2 + "): error: unmanaged alternate");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyUp(KeyCode.Alpha1))
        // {
        //     GoToScreen(GameScreen.screen1);
        // }
        // else if (Input.GetKeyUp(KeyCode.Alpha2))
        // {
        //     GoToScreen(GameScreen.screen2);
        // }
        // else if (Input.GetKeyUp(KeyCode.Alpha3))
        // {
        //     GoToScreen(GameScreen.screen3);
        // }
    }

    void LateUpdate()
    {
        if (_pauseTransition)
        {
            if ((Time.timeScale > 0.99f))
            {
                Time.timeScale = 1;
                _pauseTransition = false;
            }
            else
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, _timeScale, Time.fixedDeltaTime * _unpauseAcceleration);
            }
        }
    }
}

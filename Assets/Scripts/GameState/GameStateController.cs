using UnityEngine;
using System.Collections;
using System;

public enum GameState{
    Start,
    Game,
    Pause,
    End,
    Default
}

//specifies which game state is to be set after a computation
//Start, Game, Pause, End: GameState states as targets
//NoTarget: no specific state is required
//NoAction: no computation happened and therefore no state change shoud happen
public enum GameStateTarget{
    Start,
    Game,
    Pause,
    End,
    NoTarget,
    NoAction
}

public class GameStateController : MonoBehaviour {

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "GameStateController";
    private static GameStateController _instance;
    public static GameStateController get() {
    	if (_instance == null)
    	{
    		Logger.Log("GameStateController::get was badly initialized", Logger.Level.ERROR);
    		_instance = GameObject.Find(gameObjectName).GetComponent<GameStateController>();
    	}

    	return _instance;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public static string _masterScene = "Master-Demo-0.1";
    public static string _adventureLevel1 = "World1.0";
    public static string _sandboxLevel1 = "Sandbox-0.1";
    public static string _interfaceScene = "Interface1.0";
    public static string _bacteriumScene = "Bacterium1.0";
    //TODO refactor use of this variable
    //replace by:
    //string storedLevel = null;
    //if(MemoryManager.get ().tryGetData(_currentLevelKey, out storedLevel))
    private string _currentLevel;
    //TODO
    //{ get; set;};// = _sandboxLevel1;

    private void setCurrentLevel(string currentLevel, string cause = "default")
    {
        if(!string.IsNullOrEmpty(cause))
        {
            Debug.LogError("GameStateController::setCurrentLevel by "+cause);
        }
        _currentLevel = currentLevel;
    }

    public string getCurrentLevel ()
    {
        Debug.LogError("GameStateController::getCurrentLevel...");
        if(null == _currentLevel)
        {
            Debug.LogError("GameStateController::getCurrentLevel null == _currentLevel...");
            //LevelInfo levelInfo = null;
            string currentLevelCode = null;
            //if(MemoryManager.get ().tryGetCurrentLevelInfo(out levelInfo))
            if(MemoryManager.get ().tryGetData(GameStateController._currentLevelKey, out currentLevelCode))
            {
                Debug.LogError("GameStateController::getCurrentLevel null == _currentLevel => set memorized level...");
                //_currentLevel = levelInfo.code;
                setCurrentLevel(currentLevelCode, "getCurrentLevel");
            }
            else
            {
                Debug.LogError("GameStateController::getCurrentLevel null == _currentLevel => default level...");
                //default level
                setAndSaveLevelName(_adventureLevel1);
            }
        }
        Debug.LogError("GameStateController::getCurrentLevel returns _currentLevel="+_currentLevel);
        return _currentLevel;
    }

    public static string _currentLevelKey = "GameStateController.currentLevel";

    public static string keyPrefix = "KEY.";
    public static string _inventoryKey = keyPrefix+"INVENTORY";
    public static string _craftingKey = keyPrefix+"CRAFTING";
    public static string _pauseKey = keyPrefix+"PAUSE";
    public static string _sandboxKey = keyPrefix+"SANDBOX";


    private GameState _gameState;
    public GUITransitioner gUITransitioner;
    public Fade fadeSprite;
    public GameObject intro, endWindow, pauseIndicator;
    public ContinueButton introContinueButton;
    public EndRestartButton endRestartButton;
    private static int _pausesStacked = 0;


    void Awake() {
        Debug.LogError("AWAKE with hashcode="+this.gameObject.GetHashCode());
        Debug.LogError("AWAKE 0 _currentLevel = "+getCurrentLevel());
        if(null != _instance)
        {
            Debug.LogError("GameStateController::Awake null != instance");
        }

        Debug.LogError("AWAKE 1");
        _instance = this;
        Debug.LogError("AWAKE 2");

        
        //get data from previous instance
        //other solution: make all fields static
        //other solution: fix initialization through LinkManagers
        //that automatically take new GameStateController object
        //and leave old GameStateController object with old dead links to destroyed objects
        string storedLevel = null;
        /*
        if(MemoryManager.get ().tryGetData(_currentLevelKey, out storedLevel))
        {
            Debug.LogError("GameStateController::Awake _instance._currentLevel:=storedLevel="+storedLevel);
            _instance._currentLevel = storedLevel;
        }
        else
        {
            Debug.LogError("GameStateController::Awake no storedLevel, default _instance._currentLevel="+_instance._currentLevel);
        }
        */
        Debug.LogError("AWAKE 3");
        loadLevels();
        Debug.LogError("AWAKE 4");



        Debug.LogError("AWAKE 5 final _currentLevel = "+getCurrentLevel());
    }

    // Use this for initialization
    void Start () {
        Debug.LogError("START with hashcode="+this.gameObject.GetHashCode());
        Debug.LogError("START 1");
        _gameState = GameState.Start;
        Debug.LogError("START 2");
        resetPauseStack();
        Debug.LogError("START 3");
        I18n.changeLanguageTo(I18n.Language.French);
        Debug.LogError("START 4");
        Logger.Log("GameStateController::Start game starts in "+Localization.Localize("MAIN.LANGUAGE"), Logger.Level.INFO);
        Debug.LogError("START 5");
        Debug.LogError("START _currentLevel = "+getCurrentLevel());
    }
    
    private void loadLevels()
    {
        Debug.LogError("LOADLEVELS _instance._currentLevel="+getCurrentLevel());
        //take into account order of loading to know which LinkManager shall ask which one
        Application.LoadLevelAdditive(_interfaceScene);
        Application.LoadLevelAdditive(_bacteriumScene);
        Application.LoadLevelAdditive(getCurrentLevel());
    }
    private static void resetPauseStack ()
    {
        _pausesStacked = 0;
    }
    public static int getPausesInStackCount ()
    {
        return _pausesStacked;
    }
    private int pushPauseInStack ()
    {
        _pausesStacked++;
        Logger.Log ("pushPauseInStack() returns " + _pausesStacked, Logger.Level.INFO);
        return _pausesStacked;
    }
    public int popPauseInStack ()
    {
        Logger.Log ("popPauseInStack() starts with _pausesStacked==" + _pausesStacked, Logger.Level.DEBUG);
        if (_pausesStacked > 0) {
            _pausesStacked--;
        } else {
            Logger.Log ("GameStateController::popPauseInStack tried to pop a pause from empty stack", Logger.Level.WARN);
            _pausesStacked = 0;
        }
        Logger.Log ("popPauseInStack() returns _pausesStacked==" + _pausesStacked, Logger.Level.INFO);
        return _pausesStacked;
    }
    public void tryUnlockPause ()
    {
        Logger.Log ("tryUnlockPause() with previous _pausesStacked=" + _pausesStacked, Logger.Level.DEBUG);
        if (0 == popPauseInStack ()) {
            changeState (GameState.Game);
        }
        Logger.Log ("tryUnlockPause() with final _pausesStacked=" + _pausesStacked, Logger.Level.INFO);
    }
    public void tryLockPause ()
    {
        Logger.Log ("tryLockPause() with previous _pausesStacked=" + _pausesStacked, Logger.Level.DEBUG);
        pushPauseInStack ();
        changeState (GameState.Pause);
        Logger.Log ("tryLockPause() with final _pausesStacked=" + _pausesStacked, Logger.Level.INFO);
    }

    //TODO optimize for frequent calls & refactor out of GameStateController
    public static KeyCode getKeyCode(string localizationKey)
    {
      return (KeyCode) Enum.Parse(typeof(KeyCode), Localization.Localize(localizationKey));
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKeyDown(string localizationKey)
    {
      return Input.GetKeyDown(getKeyCode(localizationKey));
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKey(string localizationKey)
    {
      return Input.GetKey(getKeyCode(localizationKey));
    }

    private static GameState getStateFromTarget(GameStateTarget target)
    {
        GameState result = GameState.Game;

        switch (target) {
            case GameStateTarget.Start:
                result = GameState.Start;
                break;
            case GameStateTarget.Game:
                result = GameState.Game;
                break;
            case GameStateTarget.Pause:
                result = GameState.Pause;
                break;
            case GameStateTarget.End:
                result = GameState.End;
                break;
            case GameStateTarget.NoTarget:
                result = GameState.Default;
                break;
            case GameStateTarget.NoAction:
                result = GameState.Default;
                break;
            default:
                Logger.Log("GameStateController::getStateFromTarget unknown target state "+target, Logger.Level.WARN);
                result = GameState.Default;
                break;
        }

        return result;
    }
	
	// Update is called once per frame
    void Update () {

        //TODO remove this
        if(Input.GetKeyDown(KeyCode.W))
        {
            Logger.Log("pressed shortcut to teleport Cellia to the end of the game", Logger.Level.INFO);
            GameObject.Find("Player").transform.position = new Vector3(-150, 0, 1110);
            GameObject perso = GameObject.Find("Perso");
            perso.transform.localPosition = Vector3.zero;
            //perso.transform.localRotation = new Quaternion(0, -65, 0, perso.transform.localRotation.w);
            perso.GetComponent<CellControl>().stopMovement();
        } else if(Input.GetKeyDown(KeyCode.X))
        {
            Logger.Log("pressed shortcut to teleport Cellia to the pursuit", Logger.Level.INFO);
            GameObject.Find("Player").transform.position = new Vector3(500, 0, 637);
            GameObject perso = GameObject.Find("Perso");
            perso.transform.localPosition = Vector3.zero;
            //perso.transform.localRotation = new Quaternion(0, -65, 0, perso.transform.localRotation.w);
            perso.GetComponent<CellControl>().stopMovement();
        }

        switch(_gameState){

            case GameState.Start:
                //TODO put this code into a separate implementation of a "Level" interface
                //with methods such as "OnStartState", "OnGameState(bool isPause)" and so on
                //so that those modals don't appear on every level
                //Also: put specific interface elements into level scene and then move them to interface hierarchy

                //TODO remove this temporary hack
                if(getCurrentLevel() == _adventureLevel1)
                {
                    fadeSprite.gameObject.SetActive(true);
                    ModalManager.setModal(intro, true, introContinueButton.gameObject, introContinueButton.GetType().Name);
                }
                else if(getCurrentLevel() == _sandboxLevel1)
                {
                    changeState(GameState.Game);
                }
                else
                {
                    Debug.LogError("unknown _currentLevel="+getCurrentLevel());
                }
                endWindow.SetActive(false);

                break;

            case GameState.Game:
                //pause
                if (Input.GetKeyDown(KeyCode.Escape) || isShortcutKeyDown(_pauseKey))
                {
                    Logger.Log("GameStateController::Update - Escape/Pause key pressed", Logger.Level.DEBUG);
                    ModalManager.setModal(pauseIndicator, false);
                    changeState(GameState.Pause);
                } 
                //inventory
                //TODO add DNA damage accumulation management when player equips/unequips too often
                else if(isShortcutKeyDown(_inventoryKey) && Inventory.isOpenable())
                {
                    Logger.Log("GameStateController::Update inventory key pressed", Logger.Level.INFO);
                    gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen2);
                }
                //crafting
                else if(isShortcutKeyDown(_craftingKey) && CraftZoneManager.isOpenable())
                {
                    Logger.Log("GameStateController::Update craft key pressed", Logger.Level.INFO);
                    gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen3);
                }
                else if(isShortcutKeyDown(_sandboxKey))
                {
                    Debug.LogError("PRESSED K => BEFORE _currentLevel = "+getCurrentLevel());
                    setAndSaveLevelName(_sandboxLevel1);
                    Debug.LogError("PRESSED K => AFTER _currentLevel = "+getCurrentLevel());
                    restart();
                }
                break;

            case GameState.Pause:
                GameStateTarget newState = ModalManager.manageKeyPresses();
                if(GameStateTarget.NoAction != newState)
                {
                    if(
                        GameStateTarget.NoTarget != newState 
                        && GameStateTarget.Pause != newState
                        )
                    {
                        changeState(getStateFromTarget(newState));
                    }
                }
                else
                {
                    switch(gUITransitioner._currentScreen)
                    {
                        case GUITransitioner.GameScreen.screen1:
                            break;
                        case GUITransitioner.GameScreen.screen2:
                            if(isShortcutKeyDown(_inventoryKey) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
                            {
                                Logger.Log("GameStateController::Update out of inventory key pressed", Logger.Level.INFO);
                                gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
                            }
                            else if(isShortcutKeyDown(_craftingKey) && CraftZoneManager.isOpenable())
                            {
                                Logger.Log("GameStateController::Update inventory to craft key pressed", Logger.Level.INFO);
                                gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen3);
                            }
                            break;
                        case GUITransitioner.GameScreen.screen3:
                            if(isShortcutKeyDown(_inventoryKey) && Inventory.isOpenable())
                            {
                                Logger.Log("GameStateController::Update craft to inventory key pressed", Logger.Level.INFO);
                                gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen2);
                            }
                            else if(isShortcutKeyDown(_craftingKey) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
                            {
                                Logger.Log("GameStateController::Update out of craft key pressed", Logger.Level.INFO);
                                gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
                            }
                            break;
                        default:
                            Logger.Log("GameStateController::Update unknown screen "+gUITransitioner._currentScreen, Logger.Level.WARN);
                            break;
                    }
                }
                break;
        	
            case GameState.End:
                break;	

            default:
                break;
        }
    }

    public void triggerEnd(EndGameCollider egc)
    {
        gUITransitioner.TerminateGraphs();

        //TODO merge fadeSprite with Modal background
        fadeSprite.gameObject.SetActive(true);
        fadeSprite.FadeIn();

        StartCoroutine (waitFade (2f, egc));

    }

    private IEnumerator waitFade (float waitTime, EndGameCollider egc)
    {
        // do stuff before waitTime
        yield return new WaitForSeconds (waitTime);
        egc.displayEndMessage();
    }
    
    public void changeState(GameState newState){
        _gameState = newState;
        Logger.Log("GameStateController::StateChange _gameState="+_gameState, Logger.Level.DEBUG);
		
        switch(_gameState){
            case GameState.Start:
                break;
			
            case GameState.Game:
                gUITransitioner.Pause(false);
                break;
			
            case GameState.Pause:
                gUITransitioner.Pause(true);
                break;
			
            case GameState.End:
                gUITransitioner.Pause(true);
                break;

            default:
                Logger.Log("GameStateController::changeState unexpected game state "+newState, Logger.Level.WARN);
                break;
        }
    }

    public void setAndSaveLevelName(string levelName)
    {
        if(_adventureLevel1 != levelName
           && _sandboxLevel1 != levelName)
        {
            Debug.LogError("GameStateController::setAndSaveLevelName bad level name="+levelName);
        }
        else
        {
            Debug.LogError("GameStateController::setAndSaveLevelName good level name="+levelName);
            //saving level name into MemoryManager
            //because GameStateController current instance will be destroyed during restart
            //whereas MemoryManager won't
            setCurrentLevel(levelName, "setAndSaveLevelName");
            MemoryManager.get ().addOrUpdateData(_currentLevelKey, levelName);
        }
    }

    public static void restart()
    {
        Debug.LogError("RESTART with hashcode="+_instance.gameObject.GetHashCode());
        Debug.LogError("RESTART before LoadLevel _instance._currentLevel="+_instance.getCurrentLevel());
        Logger.Log ("GameStateController::restart", Logger.Level.INFO);
        Application.LoadLevel(_masterScene);
        Debug.LogError("RESTART after LoadLevel _instance._currentLevel="+_instance.getCurrentLevel());

        //setAndSaveLevelName(_instance._currentLevel);
        Debug.LogError("RESTART DONE with hashcode="+_instance.gameObject.GetHashCode());
    }

    void OnDestroy() {
        Debug.LogError("GAMESTATECONTROLLER DESTROYED");
    }
}

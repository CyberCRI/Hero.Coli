using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    MainMenu,
    Game,
    Pause,
    End,
    Default
}

//specifies which game state is to be set after a computation
//Start, MainMenu, Game, Pause, End: GameState states as targets
//NoTarget: no specific state is required
//NoAction: no computation happened and therefore no state change shoud happen
public enum GameStateTarget
{
    Start,
    MainMenu,
    Game,
    Pause,
    End,
    NoTarget,
    NoAction
}

public class GameStateController : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "GameStateController";
    private static GameStateController _instance;
    public static GameStateController get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("GameStateController::get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<GameStateController>();
        }

        return _instance;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public static string _masterScene = "Master-Demo-0.1";
    public static string _interfaceScene = "Interface1.0";
    public static string _bacteriumScene = "Bacterium1.0";


    public static string keyPrefix = "KEY.";
    public static string _inventoryKey = keyPrefix + "INVENTORY";
    public static string _craftingKey = keyPrefix + "CRAFTING";
    public static string _pauseKey = keyPrefix + "PAUSE";
    public static string _sandboxKey = keyPrefix + "SANDBOX";
    public static string _forgetDevicesKey = keyPrefix + "FORGETDEVICES";

    private GameState _gameState;
    public GUITransitioner gUITransitioner;
    public Fade fadeSprite;
    public GameObject intro, endWindow, pauseIndicator;
    public ContinueButton introContinueButton;
    public EndMainMenuButton endMainMenuButton;
    public MainMenuManager mainMenu;
    private static int _pausesStacked = 0;
    private bool _isGameLevelPrepared = false;
    private GameState _stateBeforeMainMenu = GameState.Game;

    private bool _finishLoadLevels = false;
    private bool _finishedLoadLevels = false;

    private static bool _isAdminMode = false;
    public static bool isAdminMode
    {
        get
        {
            return _isAdminMode;
        }
    }

    void Awake()
    {
        Debug.Log("GameStateController::Awake");
        GameStateController.get();
        _gameState = GameState.Start;
        resetPauseStack();
        SceneManager.sceneLoaded += SceneLoaded;
        loadLevels();
        updateAdminStatus();
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("GameStateController::Start");
        Logger.Log("GameStateController::Start game starts in " + Localization.Localize("MAIN.LANGUAGE"), Logger.Level.INFO);
    }

    public static void updateAdminStatus()
    {
        _isAdminMode = Application.isEditor || MemoryManager.get("updateAdminStatus").configuration.isTestGUID();
    }

    bool isInterfaceLoaded = false;
    bool isPlayerLoaded = false;
    bool isWorldLoaded = false;

    private void reinitializeLoadingVariables()
    {
        isInterfaceLoaded = false;
        isPlayerLoaded = false;
        isWorldLoaded = false;
    }

    void SceneLoaded(Scene scene, LoadSceneMode m)
    {
        Debug.Log("GameStateController::SceneLoaded(" + scene + ", " + m + ")");

        int level = scene.buildIndex;

        isInterfaceLoaded = isInterfaceLoaded || (3 == level);
        isPlayerLoaded = isPlayerLoaded || (2 == level);
        isWorldLoaded = isWorldLoaded || (1 == level) || (4 == level) || (5 == level);

        if (isInterfaceLoaded && isPlayerLoaded && isWorldLoaded)
        {
            finishLoadLevels();
            //_finishLoadLevels = true;
        }
    }

    private void loadLevels()
    {
        Debug.Log("GameStateController::loadLevels");
        SceneManager.LoadScene(_interfaceScene, LoadSceneMode.Additive);
        SceneManager.LoadScene(_bacteriumScene, LoadSceneMode.Additive);
        SceneManager.LoadScene(MemoryManager.get("loadLevels").configuration.getSceneName(), LoadSceneMode.Additive);
    }

    private void finishLoadLevels()
    {
        Debug.Log("GameStateController::finishLoadLevels");

        // get the linkers
        InterfaceLinkManager ilm = InterfaceLinkManager.get();
        PlayerLinkManager blm = PlayerLinkManager.get();
        WorldLinkManager wlm = WorldLinkManager.get();

        Debug.Log("GameStateController initialization: ilm=" + ilm + ", blm=" + blm + ", wlm=" + wlm);
        ilm.initialize();
        blm.initialize();
        wlm.initialize();

        Debug.Log("finishInitialize");
        ilm.finishInitialize();
        blm.finishInitialize();
        wlm.finishInitialize();

        // open the Main Menu
        Debug.Log("finishLoadLevels => MainMenuManager.get().open()");

        _finishedLoadLevels = true;

        MainMenuManager.get().open();
    }

    public static bool isPause()
    {
        return (GameState.Pause == _instance._gameState);
    }

    private static void resetPauseStack()
    {
        _pausesStacked = 0;
    }
    public static int getPausesInStackCount()
    {
        return _pausesStacked;
    }
    private int pushPauseInStack()
    {
        _pausesStacked++;
        Logger.Log("pushPauseInStack() returns " + _pausesStacked, Logger.Level.INFO);
        return _pausesStacked;
    }
    public int popPauseInStack()
    {
        Logger.Log("popPauseInStack() starts with _pausesStacked==" + _pausesStacked, Logger.Level.DEBUG);
        if (_pausesStacked > 0)
        {
            _pausesStacked--;
        }
        else
        {
            Logger.Log("GameStateController::popPauseInStack tried to pop a pause from empty stack", Logger.Level.WARN);
            _pausesStacked = 0;
        }
        Logger.Log("popPauseInStack() returns _pausesStacked==" + _pausesStacked, Logger.Level.INFO);
        return _pausesStacked;
    }
    public void tryUnlockPause()
    {
        Logger.Log("tryUnlockPause() with previous _pausesStacked=" + _pausesStacked, Logger.Level.DEBUG);
        if (0 == popPauseInStack())
        {
            changeState(GameState.Game);
        }
        Logger.Log("tryUnlockPause() with final _pausesStacked=" + _pausesStacked, Logger.Level.INFO);
    }
    public void tryLockPause()
    {
        Logger.Log("tryLockPause() with previous _pausesStacked=" + _pausesStacked, Logger.Level.DEBUG);
        pushPauseInStack();
        changeState(GameState.Pause);
        Logger.Log("tryLockPause() with final _pausesStacked=" + _pausesStacked, Logger.Level.INFO);
    }

    //TODO optimize for frequent calls & refactor out of GameStateController
    public static KeyCode getKeyCode(string localizationKey)
    {
        return (KeyCode)Enum.Parse(typeof(KeyCode), Localization.Localize(localizationKey));
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKeyDown(KeyCode code, bool restricted = false)
    {
        return (!restricted || isAdminMode) && Input.GetKeyDown(code);
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKey(KeyCode code, bool restricted = false)
    {
        return (!restricted || isAdminMode) && Input.GetKey(code);
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKeyDown(string localizationKey, bool restricted = false)
    {
        return (!restricted || isAdminMode) && Input.GetKeyDown(getKeyCode(localizationKey));
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKey(string localizationKey, bool restricted = false)
    {
        return (!restricted || isAdminMode) && Input.GetKey(getKeyCode(localizationKey));
    }

    private static GameState getStateFromTarget(GameStateTarget target)
    {
        GameState result = GameState.Game;

        switch (target)
        {
            case GameStateTarget.Start:
                result = GameState.Start;
                break;
            case GameStateTarget.MainMenu:
                result = GameState.MainMenu;
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
                Logger.Log("GameStateController::getStateFromTarget unknown target state " + target, Logger.Level.WARN);
                result = GameState.Default;
                break;
        }

        return result;
    }

    public void endGame()
    {
        Logger.Log("endGame", Logger.Level.INFO);
        _isGameLevelPrepared = false;
        reinitializeLoadingVariables();
        mainMenu.setNewGame();
        goToMainMenuFrom(GameState.MainMenu);
    }

    private void prepareGameLevelIfNecessary()
    {
        if (!_isGameLevelPrepared)
        {
            //TODO put this code into a separate implementation of a "Level" interface
            //with methods such as "OnStartState", "OnGameState(bool isPause)" and so on
            //so that those modals don't appear on every level
            //Also: put specific interface elements into level scene and then move them to interface hierarchy

            mainMenu.setResume();

            //TODO remove this temporary hack
            switch (MemoryManager.get("prepareGameLevelIfNecessary").configuration.getMode())
            {
                case GameConfiguration.GameMode.ADVENTURE:
                    fadeSprite.gameObject.SetActive(true);
                    ModalManager.setModal(intro, true, introContinueButton.gameObject, introContinueButton.GetType().Name);
                    changeState(GameState.Pause);
                    break;
                case GameConfiguration.GameMode.SANDBOX:
                    break;
                default:
                    Logger.Log("GameStateController::Update unknown game mode=" + MemoryManager.get("prepareGameLevelIfNecessary 2").configuration.getMode(), Logger.Level.WARN);
                    break;
            }
            _isGameLevelPrepared = true;
        }
    }

    public void goToMainMenu()
    {
        Debug.Log("GameStateController::goToMainMenu");
        goToMainMenuFrom(_gameState);
    }

    public void goToMainMenuFrom(GameState state)
    {
        _stateBeforeMainMenu = state;
        Debug.Log("goToMainMenuFrom(" + state + ") with mainMenu=" + mainMenu);
        mainMenu.open();
        changeState(GameState.MainMenu);
    }

    public void leaveMainMenu()
    {
        Debug.Log("GameStateController::leaveMainMenu");

        //restart
        if (GameState.MainMenu == _stateBeforeMainMenu)
        {
            GameStateController.restart();
            //resume or new game
        }
        else
        {
            mainMenu.close();
            changeState(_stateBeforeMainMenu);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // if (_finishLoadLevels)
        // {
        //     _finishLoadLevels = false;
        //     finishLoadLevels();
        // }

        if (_finishedLoadLevels)
        {

            if (_isAdminMode)
            {
                if (isShortcutKeyDown(KeyCode.X))
                {
                    Logger.Log("pressed shortcut to teleport Cellia to the pursuit", Logger.Level.INFO);
                    CellControl.get().teleport(new Vector3(500, 0, 637));
                }
                else if (isShortcutKeyDown(KeyCode.V))
                {
                    Logger.Log("pressed shortcut to teleport Cellia to the GFP BioBrick", Logger.Level.INFO);
                    CellControl.get().teleport(new Vector3(168, 0, 724));

                }
                else if (isShortcutKeyDown(KeyCode.W))
                {
                    Logger.Log("pressed shortcut to teleport Cellia to the end of the game", Logger.Level.INFO);
                    CellControl.get().teleport(new Vector3(-150, 0, 1110));
                }
            }

            switch (_gameState)
            {

                case GameState.Start:

                    endWindow.SetActive(false);
                    mainMenu.setNewGame();
                    if (GameConfiguration.RestartBehavior.GAME == MemoryManager.get("Update").configuration.restartBehavior)
                    {
                        leaveMainMenu();
                    }
                    else
                    {
                        goToMainMenuFrom(GameState.Game);
                    }
                    break;

                case GameState.MainMenu:
                    if (Input.GetKeyUp(KeyCode.UpArrow))
                    {
                        mainMenu.selectPrevious();
                    }
                    else if (Input.GetKeyUp(KeyCode.DownArrow))
                    {
                        mainMenu.selectNext();
                    }
                    else if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
                    {
                        mainMenu.getCurrentItem().click();
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        mainMenu.escape();
                    }
                    break;

                case GameState.Game:

                    prepareGameLevelIfNecessary();

                    //pause
                    if (isShortcutKeyDown(_pauseKey))
                    {
                        Logger.Log("GameStateController::Update - Escape/Pause key pressed", Logger.Level.DEBUG);
                        ModalManager.setModal(pauseIndicator, false);
                        changeState(GameState.Pause);
                    }
                    //main menu
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        goToMainMenuFrom(GameState.Game);
                    }
                    //inventory
                    //TODO add DNA damage accumulation management when player equips/unequips too often
                    // else if(isShortcutKeyDown(_inventoryKey) && Inventory.isOpenable())
                    // {
                    //     Logger.Log("GameStateController::Update inventory key pressed", Logger.Level.INFO);
                    //     gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen2);
                    // }
                    //crafting
                    else if (isShortcutKeyDown(_craftingKey) && CraftZoneManager.isOpenable())
                    {
                        Logger.Log("GameStateController::Update craft key pressed", Logger.Level.INFO);
                        gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen3);
                    }
                    /*
                    else if(isShortcutKeyDown(_sandboxKey))
                    {
                        Logger.Log("GameStateController::Update sandbox key pressed from scene="+MemoryManager.get ().configuration.getSceneName(), Logger.Level.INFO);
                        goToOtherGameMode();
                    }*/
                    //TODO fix this feature
                    /*
                    else if(isShortcutKeyDown(_forgetDevicesKey))
                    {
                        Inventory.get ().switchDeviceKnowledge();
                    }*/
                    break;

                case GameState.Pause:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        goToMainMenuFrom(GameState.Pause);
                    }
                    else
                    {
                        GameStateTarget newState = ModalManager.manageKeyPresses();
                        if (GameStateTarget.NoAction != newState)
                        {
                            if (
                                GameStateTarget.NoTarget != newState
                                && GameStateTarget.Pause != newState
                                )
                            {
                                changeState(getStateFromTarget(newState));
                            }
                        }
                        else
                        {
                            switch (gUITransitioner._currentScreen)
                            {
                                case GUITransitioner.GameScreen.screen1:
                                    break;
                                case GUITransitioner.GameScreen.screen2:
                                    if (isShortcutKeyDown(_inventoryKey) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
                                    {
                                        Logger.Log("GameStateController::Update out of inventory key pressed", Logger.Level.INFO);
                                        gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
                                    }
                                    else if (isShortcutKeyDown(_craftingKey) && CraftZoneManager.isOpenable())
                                    {
                                        Logger.Log("GameStateController::Update inventory to craft key pressed", Logger.Level.INFO);
                                        gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen3);
                                    }
                                    break;
                                case GUITransitioner.GameScreen.screen3:
                                    if (isShortcutKeyDown(_inventoryKey) && Inventory.isOpenable())
                                    {
                                        Logger.Log("GameStateController::Update craft to inventory key pressed", Logger.Level.INFO);
                                        gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen2);
                                    }
                                    else if (isShortcutKeyDown(_craftingKey) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
                                    {
                                        Logger.Log("GameStateController::Update out of craft key pressed", Logger.Level.INFO);
                                        gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
                                    }
                                    break;
                                default:
                                    Logger.Log("GameStateController::Update unknown screen " + gUITransitioner._currentScreen, Logger.Level.WARN);
                                    break;
                            }
                        }
                    }
                    break;

                case GameState.End:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        goToMainMenuFrom(GameState.End);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    public void goToOtherGameMode()
    {
        Logger.Log("GameStateController::goToOtherGameMode", Logger.Level.INFO);
        GameConfiguration.GameMap destination =
            (GameConfiguration.getMode(MemoryManager.get("goToOtherGameMode").configuration.gameMap) == GameConfiguration.GameMode.ADVENTURE) ?
                GameConfiguration.GameMap.SANDBOX2 :
                GameConfiguration.GameMap.TUTORIAL1;

        setAndSaveLevelName(destination, "goToOtherGameMode");
        RedMetricsManager.get().sendEvent(TrackingEvent.SWITCH, new CustomData(CustomDataTag.GAMELEVEL, destination.ToString()));
        internalRestart();
    }

    public void triggerEnd(EndGameCollider egc)
    {
        GUITransitioner.showGraphs(false, GUITransitioner.GRAPH_HIDER.ENDGAME);
        MemoryManager.get().sendCompletionEvent();
        egc.displayEndMessage();

        /*
                gUITransitioner.showGraphs(false);

                //TODO merge fadeSprite with Modal background
                fadeSprite.gameObject.SetActive(true);
                fadeSprite.FadeIn(0.5f);

                StartCoroutine (waitFade (2f, egc));
                */
    }

    private IEnumerator waitFade(float waitTime, EndGameCollider egc)
    {
        // do stuff before waitTime
        yield return new WaitForSeconds(waitTime);
        egc.displayEndMessage();
    }

    public void changeState(GameState newState)
    {
        _gameState = newState;
        Logger.Log("GameStateController::StateChange _gameState=" + _gameState, Logger.Level.DEBUG);

        switch (_gameState)
        {
            case GameState.Start:
                break;

            case GameState.MainMenu:
                gUITransitioner.Pause(true);
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
                Logger.Log("GameStateController::changeState unexpected game state " + newState, Logger.Level.WARN);
                break;
        }
    }

    public void setAndSaveLevelName(GameConfiguration.GameMap newMap, string cause = null)
    {
        if (!string.IsNullOrEmpty(cause))
        {
            Logger.Log("GameStateController::setAndSaveLevelName by " + cause, Logger.Level.DEBUG);
        }
        else
        {
            Logger.Log("GameStateController::setAndSaveLevelName", Logger.Level.DEBUG);
        }

        switch (newMap)
        {
            case GameConfiguration.GameMap.ADVENTURE1:
            case GameConfiguration.GameMap.SANDBOX1:
            case GameConfiguration.GameMap.SANDBOX2:
            case GameConfiguration.GameMap.TUTORIAL1:
                //saving level name into MemoryManager
                //because GameStateController current instance will be destroyed during restart
                //whereas MemoryManager won't
                MemoryManager.get("setAndSaveLevelName").configuration.gameMap = newMap;
                break;

            default:
                Logger.Log("GameStateController::setAndSaveLevelName unmanaged level=" + newMap, Logger.Level.WARN);
                break;
        }
    }

    public GameState getState()
    {
        return _gameState;
    }

    public static void restart()
    {
        RedMetricsManager.get().sendEvent(TrackingEvent.RESTART);
        internalRestart();
    }

    private static void internalRestart()
    {
        Logger.Log("GameStateController::restart", Logger.Level.INFO);
        //TODO reload scene but reset all of its components without using MemoryManager
        //note: ways to transfer data from one scene to another
        //get data from previous instance
        //other solution: make all fields static
        //other solution: fix initialization through LinkManagers
        //that automatically take new GameStateController object
        //and leave old GameStateController object with old dead links to destroyed objects

        MemoryManager.get("internalRestart").configuration.restartBehavior = GameConfiguration.RestartBehavior.GAME;
        Application.LoadLevel(_masterScene);
    }

    void OnDestroy()
    {
        Logger.Log("GameStateController::OnDestroy", Logger.Level.DEBUG);
    }

    public void FadeScreen(bool fade, float speed)
    {
        fadeSprite.gameObject.SetActive(true);
        if (fade)
        {
            fadeSprite.FadeIn(speed);
        }
        else
        {
            fadeSprite.FadeOut(speed);
        }
    }
}

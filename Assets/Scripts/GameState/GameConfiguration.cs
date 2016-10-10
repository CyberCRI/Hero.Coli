using UnityEngine;
using System;

public class GameConfiguration {

    public GameConfiguration()
    {
        restartBehavior = RestartBehavior.MAINMENU;
        gameMap = GameMap.TUTORIAL1;
        language = I18n.Language.English;
        isAbsoluteWASD = true;
        isLeftClickToMove = true;
        
        //TODO send playerGUID to RedMetrics
        //TODO unlock Sandbox if Adventure was finished 
    }
    
    private const string _adventureLevel1 = "World1.0";
    private const string _tutorial = "Tutorial1";
    private const string _sandboxLevel1 = "Sandbox-0.1";
    private const string _sandboxLevel2 = "Sandbox-0.2";
    
    private string localPlayerGUIDPlayerPrefsKey = "localPlayerGUID";
    private string gameVersionGUIDPlayerPrefsKey = "gameVersionGUID";

    public enum RestartBehavior
    {
        MAINMENU,
        GAME
    }

    public enum GameMap
    {
        ADVENTURE1,
        SANDBOX1,
        SANDBOX2,
        TUTORIAL1
    }
    
    public enum TutorialMode
    {
        START1FLAGELLUM,
        START1FLAGELLUM4BRICKS,
        START1FLAGELLUMDEVICE,
        START0FLAGELLUM,
        START0FLAGELLUMHORIZONTALTRANSFER
    }

    public enum GameMode
    {
        ADVENTURE,
        SANDBOX
    }
    
    public enum CraftInterface
    {
        UNLIMITEDDEVICES,
        LIMITEDDEVICES
    }

    public RestartBehavior restartBehavior;
    public GameMap gameMap;
    public string getGameMapName() {
        return gameMap.ToString().ToLowerInvariant();
    }
    public I18n.Language language;    
    public bool isAbsoluteWASD;
    public bool isLeftClickToMove;
    //TODO manage sound configuration
    public bool isSoundOn;
    public bool isAdmin = false;
    public CraftInterface craftInterface = CraftInterface.LIMITEDDEVICES;
    public TutorialMode tutorialMode = TutorialMode.START0FLAGELLUMHORIZONTALTRANSFER;
    
    //////////////////////////////////////////////////////////////////////////////////////////////////
    ///REDMETRICS TRACKING ///////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////
    //test
    public const string testVersionGUID = "83f99dfa-bd87-43e1-940d-f28bbcea5b1d";    
    //v 1.0
    //private const string gameVersionGuid = "\"99a00e65-6039-41a3-a85b-360c4b30a466\"";
    //v 1.31
    //private const string gameVersionGuid = "\"5832732e-6bfb-4ac7-8df4-270c6f20b72a\"";
    //v 1.32
    //public const string labelledGameVersionGUID = "be209fe8-0ef3-4291-a5f4-c2b389f5d77d";
    //v 1.33
    //public const string labelledGameVersionGUID = "51b8a78a-8dd3-4a5e-9f41-01e6805e0f52";
    //v 1.40
    //public const string labelledGameVersionGUID = "81ee441f-6cbc-45ba-a306-160905c80b97";
    //v1.50
    public const string labelledGameVersionGUID = "fef94d5f-d99a-4212-9f21-87308293fb03";

    //public string defaultPlayer = "b5ab445a-56c9-4c5b-a6d0-86e8a286cd81";
        
    private string _playerGUID;
    public string playerGUID {
        get {
            if (string.IsNullOrEmpty(_playerGUID)) {
                //TODO make it work through different versions of the game,
                //     so that memory is not erased every time a new version of the game is published
                string storedGUID = PlayerPrefs.GetString(localPlayerGUIDPlayerPrefsKey);
                if(string.IsNullOrEmpty(storedGUID)) {
                    _playerGUID = Guid.NewGuid().ToString();
                    PlayerPrefs.SetString(localPlayerGUIDPlayerPrefsKey, _playerGUID);
                } else {
                    _playerGUID = storedGUID;
                }
            }
            return _playerGUID;
        }
    }
    
    private string _gameVersionGUID;
    public string gameVersionGUID {
        get {
            if (string.IsNullOrEmpty(_gameVersionGUID)) {
                string storedGUID = PlayerPrefs.GetString(gameVersionGUIDPlayerPrefsKey);
                if(string.IsNullOrEmpty(storedGUID)) {
                    
                    //if the game is launched in the editor,
                    // sets the localPlayerGUID to a test GUID 
                    // so that events are logged onto a test version
                    // instead of the regular game version
                    // to prevent data from being contaminated by tests
                    setMetricsDestination(!Application.isEditor);
                } else {
                    gameVersionGUID = storedGUID;
                }
            }
            return _gameVersionGUID;
        }
        set {
            _gameVersionGUID = value;
            PlayerPrefs.SetString(gameVersionGUIDPlayerPrefsKey, _gameVersionGUID);
            if(Application.platform == RuntimePlatform.WebGLPlayer) {
                RedMetricsManager.get().disconnect ();
            }
            RedMetricsManager.get ().setGameVersion(_gameVersionGUID);
            if(Application.platform == RuntimePlatform.WebGLPlayer) {
                RedMetricsManager.get().connect ();
            }
            GameStateController.updateAdminStatus();
        }
    }
    
    //sets the destination to which logs will be sent
    public void setMetricsDestination(bool wantToBecomeLabelledGameVersion) {
        if(wantToBecomeLabelledGameVersion) { //sets the default destination: a labelled game version
            gameVersionGUID = labelledGameVersionGUID;
        } else { //sets a test version destination
            gameVersionGUID = testVersionGUID;
        }
    }
        
    //switches the logging mode from test to normal and conversely
    //returns true if switched to normal
    public bool switchMetricsGameVersion() {
        //TODO
        //RedMetricsManager.get ().sendEvent(TrackingEvent.SWITCHFROMGAMEVERSION, RedMetricsManager.get().generateCustomDataForGuidInit());
        setMetricsDestination(isTestGUID());
        RedMetricsManager.get ().sendEvent(TrackingEvent.SWITCHTOGAMEVERSION, RedMetricsManager.get().generateCustomDataForGuidInit());
        return !isTestGUID();
    }
    
    public bool isTestGUID() {
        return testVersionGUID == gameVersionGUID;
    }

    public GameMode getMode() {
        return getMode (gameMap);
    }

    public static GameMode getMode(GameMap map) {
        switch(map)
        {
            case GameMap.ADVENTURE1:
            case GameMap.TUTORIAL1:
                return GameMode.ADVENTURE;
            case GameMap.SANDBOX1:
            case GameMap.SANDBOX2:
                return GameMode.SANDBOX;
            default:
                Logger.Log("unknown map "+map, Logger.Level.ERROR);
                return GameMode.ADVENTURE;
        }
    }

    public string getSceneName() {
        return getSceneName(gameMap);
    }

    public static string getSceneName(GameMap map) {
        switch(map)
        {
            case GameMap.ADVENTURE1:
                return _adventureLevel1;
            case GameMap.SANDBOX1:
                return _sandboxLevel1;
            case GameMap.SANDBOX2:
                return _sandboxLevel2;
            case GameMap.TUTORIAL1:
                return _tutorial;
            default:
                Logger.Log("unknown map "+map, Logger.Level.ERROR);
                return _adventureLevel1;
        }
    }

    public override string ToString ()
    {
      return string.Format ("[GameConfiguration: restartBehavior={0}, gameMap={1}]", restartBehavior, gameMap);
    }
}

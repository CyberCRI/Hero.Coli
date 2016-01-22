using UnityEngine;
using System.Collections.Generic;
using System;

public class GameConfiguration {

    public GameConfiguration()
    {
        restartBehavior = RestartBehavior.MAINMENU;
        gameMap = GameMap.ADVENTURE1;
        language = I18n.Language.English;
        isAbsoluteWASD = true;
        isLeftClickToMove = true;
        
        //TODO send playerGUID to RedMetrics
        //TODO unlock Sandbox if Adventure was finished 
    }
    
    private static string _adventureLevel1 = "World1.0";
    private static string _sandboxLevel1 = "Sandbox-0.1";
    private static string _sandboxLevel2 = "Sandbox-0.2";
    
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
        SANDBOX2
    }

    public enum GameMode
    {
        ADVENTURE,
        SANDBOX
    }

    public RestartBehavior restartBehavior;
    public GameMap gameMap;
    public I18n.Language language;    
    public bool isAbsoluteWASD;
    public bool isLeftClickToMove;
    //TODO manage sound configuration
    public bool isSoundOn;
    public bool isAdmin = false;
    
    //////////////////////////////////////////////////////////////////////////////////////////////////
    ///REDMETRICS TRACKING ///////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////
    //test
    public static string testVersionGUID = "83f99dfa-bd87-43e1-940d-f28bbcea5b1d";    
    //v1.0
    //private static string gameVersionGuid = "\"99a00e65-6039-41a3-a85b-360c4b30a466\"";
    //v1.31
    //private static string gameVersionGuid = "\"5832732e-6bfb-4ac7-8df4-270c6f20b72a\"";
    //v1.32
    public static string labelledGameVersionGUID = "be209fe8-0ef3-4291-a5f4-c2b389f5d77d";
    //v1.33
    //public string gameVersionGuid = "51b8a78a-8dd3-4a5e-9f41-01e6805e0f52";
    
    //public string defaultPlayer = "b5ab445a-56c9-4c5b-a6d0-86e8a286cd81";
        
    private string _playerGUID;
    public string playerGUID {
        get {
            if (!string.IsNullOrEmpty(_playerGUID)) {
                Debug.LogError("use playerGUID stored in GameConfiguration: "+_playerGUID);                
            } else {
                //TODO make it work through different versions of the game,
                //     so that memory is not erased every time a new version of the game is published
                string storedGUID = PlayerPrefs.GetString(localPlayerGUIDPlayerPrefsKey);
                if(string.IsNullOrEmpty(storedGUID)) {
                    _playerGUID = Guid.NewGuid().ToString();
                    PlayerPrefs.SetString(localPlayerGUIDPlayerPrefsKey, _playerGUID);
                    Debug.LogError("use new playerGUID: "+_playerGUID);
                } else {
                    _playerGUID = storedGUID;
                    Debug.LogError("use playerGUID stored in PlayerPrefs: "+_playerGUID);
                }
            }
            return _playerGUID;
        }
    }
    
    private string _gameVersionGUID;
    public string gameVersionGUID {
        get {
            if (!string.IsNullOrEmpty(_gameVersionGUID)) {
                Debug.LogError("use gameVersionGUID stored in GameConfiguration: "+_gameVersionGUID);                
            } else {
                string storedGUID = PlayerPrefs.GetString(gameVersionGUIDPlayerPrefsKey);
                if(string.IsNullOrEmpty(storedGUID)) {
                    gameVersionGUID = Guid.NewGuid().ToString();
                    Debug.LogError("use new gameVersionGUID: "+_gameVersionGUID);
                } else {
                    gameVersionGUID = storedGUID;
                    Debug.LogError("use gameVersionGUID stored in PlayerPrefs: "+_gameVersionGUID);
                }
            }
            return _gameVersionGUID;
        }
        set {
            Debug.LogError("set gameVersionGUID to "+value);
            _gameVersionGUID = value;
            PlayerPrefs.SetString(gameVersionGUIDPlayerPrefsKey, _gameVersionGUID);
            RedMetricsManager.get ().setGameVersion(_gameVersionGUID);
        }
    }
    
    //sets the destination to which logs will be sent
    public void setMetricLogDestination(bool isDefault) {
        if(isDefault) { //sets the default destination: a labelled game version
            gameVersionGUID = labelledGameVersionGUID;
        } else { //sets a test version destination
            gameVersionGUID = testVersionGUID;
        }
    }
        
    //switches the logging mode from test to normal and conversely
    //returns true if switched to normal
    public bool switchGUID() {
        setMetricLogDestination(!isTestGUID());
        //send event to signal playerGUID manual change
        RedMetricsManager.get ().sendEvent(TrackingEvent.SWITCHTESTGUID);
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

using UnityEngine;

public class GameConfiguration {
    
    private static string _adventureLevel1 = "World1.0";
    private static string _sandboxLevel1 = "Sandbox-0.1";
    private static string _sandboxLevel2 = "Sandbox-0.2";

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

    private RestartBehavior _restartBehavior;
    public RestartBehavior restartBehavior {
        get {
            Debug.LogError("restartBehavior accessed");
            return _restartBehavior;
        }
        set {
            _restartBehavior = value;
            Debug.LogError("restartBehavior set to "+value);
        }
    }
    
    private GameMap _gameMap;
    public GameMap gameMap {
        get {
            Debug.LogError("gameMap accessed");
            return _gameMap;
        }
        set {
            _gameMap = value;
            Debug.LogError("gameMap set to "+value);
        }
    }

    private I18n.Language _language;
    public I18n.Language language {
        get {
            Debug.LogError("language accessed");
            return _language;
        }
        set {
            _language = value;
            I18n.changeLanguageTo(value);
            Debug.LogError("language set to "+value);
        }
    }


    public GameConfiguration()
    {
        Debug.LogError("GameConfiguration()");
        restartBehavior = RestartBehavior.MAINMENU;
        gameMap = GameMap.ADVENTURE1;
        language = I18n.Language.English;
    }

    public GameMode getMode() {
        Debug.LogError("getMode");
        return getMode (gameMap);
    }

    public static GameMode getMode(GameMap map) {
        Debug.LogError("getMode("+map+")");
        switch(map)
        {
            case GameMap.ADVENTURE1:
                return GameMode.ADVENTURE;
            case GameMap.SANDBOX1:
            case GameMap.SANDBOX2:
                return GameMode.SANDBOX;
            default:
                Debug.LogError("unknown map "+map);
                return GameMode.ADVENTURE;
        }
    }

    public string getSceneName() {
        Debug.LogError("getSceneName");
        return getSceneName(gameMap);
    }

    public static string getSceneName(GameMap map) {
        Debug.LogError("getSceneName("+map+")");
        switch(map)
        {
            case GameMap.ADVENTURE1:
                return _adventureLevel1;
            case GameMap.SANDBOX1:
                return _sandboxLevel1;
            case GameMap.SANDBOX2:
                return _sandboxLevel2;
            default:
                Debug.LogError("unknown map "+map);
                return _adventureLevel1;
        }
    }

    public override string ToString ()
    {
      return string.Format ("[GameConfiguration: restartBehavior={0}, gameMap={1}]", restartBehavior, gameMap);
    }
}

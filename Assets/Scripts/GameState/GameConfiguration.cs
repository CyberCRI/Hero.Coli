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

    public RestartBehavior restartBehavior;
    public GameMap gameMap;
    public I18n.Language language;    
    public bool isAbsoluteWASD;
    public bool isLeftClickToMove;

    public GameConfiguration()
    {
        restartBehavior = RestartBehavior.MAINMENU;
        gameMap = GameMap.ADVENTURE1;
        language = I18n.Language.English;
        isAbsoluteWASD = true;
        isLeftClickToMove = true;
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
                Debug.LogError("unknown map "+map);
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
                Debug.LogError("unknown map "+map);
                return _adventureLevel1;
        }
    }

    public override string ToString ()
    {
      return string.Format ("[GameConfiguration: restartBehavior={0}, gameMap={1}]", restartBehavior, gameMap);
    }
}

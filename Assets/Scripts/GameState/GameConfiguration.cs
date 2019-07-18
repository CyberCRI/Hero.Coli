#define TUTORIAL2

using UnityEngine;
using System;

public class GameConfiguration
{

    public GameConfiguration()
    {
        // Debug.Log(this.GetType() + " ctor");

        _gameMap.initialize();

        //TODO send playerGUID to RedMetrics
        //TODO unlock Sandbox if Adventure was finished
    }

    private const string _adventureLevel1 = "World1.0";
#if !TUTORIAL2
    private const string _tutorial = "Tutorial1";
#else
    private const string _tutorial = "Tutorial2";
#endif
    private const string _sandboxLevel1 = "Sandbox-0.1";
    private const string _sandboxLevel2 = "Sandbox-0.2";

    private const string _localPlayerGUIDPlayerPrefsKey = "localPlayerGUID";

    public enum RestartBehavior
    {
        UNINITIALIZED,
        MAINMENU,
        GAME
    }

    public enum GameMap
    {
        UNINITIALIZED,
        // ADVENTURE1,
        // SANDBOX1,
        SANDBOX2,
#if !TUTORIAL2
        TUTORIAL1,
#else
        TUTORIAL2,
#endif
    }

    // public enum TutorialMode
    // {
    //     START1FLAGELLUM,
    //     START1FLAGELLUM4BRICKS,
    //     START1FLAGELLUMDEVICE,
    //     START0FLAGELLUM,
    //     START0FLAGELLUMHORIZONTALTRANSFER
    // }

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

    // restartBehavior = RestartBehavior.MAINMENU;
    // gameMap = GameMap.TUTORIAL1;
    // language = I18n.Language.English;
    // isAbsoluteWASD = true;
    // isLeftClickToMove = true;



    // _restartBehavior = new EnumConfigurationParameter<RestartBehavior>(startValue, defaultValue, uninitializedValue, key);

    private EnumConfigurationParameter<RestartBehavior> _restartBehavior
        = new EnumConfigurationParameter<RestartBehavior>(RestartBehavior.MAINMENU, RestartBehavior.MAINMENU, RestartBehavior.UNINITIALIZED, _restartBehaviorKey);
    private static EnumConfigurationParameter<GameMap> _gameMap
    #if !TUTORIAL2
        = new EnumConfigurationParameter<GameMap>(GameMap.UNINITIALIZED, GameMap.TUTORIAL1, GameMap.UNINITIALIZED, _restartBehaviorKey, onMapChanged);
    #else
        = new EnumConfigurationParameter<GameMap>(GameMap.UNINITIALIZED, GameMap.TUTORIAL2, GameMap.UNINITIALIZED, _restartBehaviorKey, onMapChanged);
    #endif
    private BoolConfigurationParameter _isAbsoluteWASD
        = new BoolConfigurationParameter(true, true, _isAbsoluteWASDKey);
    private BoolConfigurationParameter _isLeftClickToMove
        = new BoolConfigurationParameter(true, true, _isLeftClickToMoveKey);
    private static float _baseVolume
        = -1;
    private static BoolConfigurationParameter _isSoundOn
        = new BoolConfigurationParameter(true, true, _isSoundOnKey, onSoundChanged);
    private static IntConfigurationParameter _furthestChapterReached
        = new IntConfigurationParameter(0, 0, _furthestChapterReachedKey);
    private static FloatConfigurationParameter[] _bestTimes
        = null;

    private void initializeBestTimesIfNecessary()
    {
        // Debug.Log(this.GetType() + " initializeBestTimesIfNecessary");
        if (null == _bestTimes)
        {
            _bestTimes = new FloatConfigurationParameter[Scorekeeper.completionsCount];
            for (int index = 0; index < Scorekeeper.completionsCount; index++)
            {
                // Debug.Log(this.GetType() + " initializeBestTimes index = " + index);
                _bestTimes[index] = new FloatConfigurationParameter(Mathf.Infinity, Mathf.Infinity, _bestCompletionTimeChapterStem + index);
                _bestTimes[index].initialize();
            }
        }
    }


#if UNITY_EDITOR
    // if the game is launched in the editor,
    // sets the Game Version GUID to a test GUID 
    // so that events are logged onto a test version
    // instead of the regular game version
    // to prevent data from being contaminated by tests
    private const bool _adminStartValue = true, _adminDefaultValue = true;
#else
    private const bool _adminStartValue = false, _adminDefaultValue = false;
#endif
    private static BoolConfigurationParameter _isAdmin = new BoolConfigurationParameter(_adminStartValue, _adminDefaultValue, _isAdminKey);

    private const RestartBehavior _restartBehaviorDefaultValue = RestartBehavior.MAINMENU;
    public RestartBehavior restartBehavior
    {
        get
        {
            return _restartBehavior.val;
        }
        set
        {
            _restartBehavior.val = value;

        }
    }

    private static string _gameMapName;
    public static GameMap gameMap
    {
        get
        {
            // Debug.Log("GameConfiguration gameMap get returns " + _gameMap.val);
            return _gameMap.val;
        }
        set
        {
            _gameMap.val = value;
        }
    }
    private static void onMapChanged(GameMap newMap)
    {
        _gameMapName = newMap.ToString().ToLowerInvariant();
        // Debug.Log("GameConfiguration onMapChanged newMap = " + newMap + ", _gameMapName = " + _gameMapName);
    }
    public string getGameMapName()
    {
        return _gameMapName;
    }

    public bool isAbsoluteWASD
    {
        get
        {
            return _isAbsoluteWASD.val;
        }
        set
        {
            _isAbsoluteWASD.val = value;
        }
    }

    public bool isLeftClickToMove
    {
        get
        {
            return _isLeftClickToMove.val;
        }
        set
        {
            _isLeftClickToMove.val = value;

        }
    }

    public bool isSoundOn
    {
        get
        {
            // Debug.Log(this.GetType() + " getting sound = " + _isSoundOn.val);
            return _isSoundOn.val;
        }
        set
        {
            // Debug.Log(this.GetType() + " setting sound to " + value);
            _isSoundOn.val = value;
        }
    }

    private static void onSoundChanged(bool newSoundValue)
    {
        // Debug.Log("GameConfiguration onSoundChanged");
        if (_baseVolume < 0)
        {
            if (0 == AudioListener.volume)
            {
                _baseVolume = 1f;
            }
            else
            {
                _baseVolume = AudioListener.volume;
            }
        }
        AudioListener.volume = newSoundValue ? _baseVolume : 0f;
    }

    public int furthestChapter
    {
        get
        {
            // Debug.Log(this.GetType() + " getting furthestChapter = " + _furthestChapterReached.val);
            return _furthestChapterReached.val;
        }
        set
        {
            // Debug.Log(this.GetType() + " trying to set furthestChapter to " + value);
            // if (value > _furthestChapterReached.val)
            // {
            // Debug.Log(this.GetType() + " setting furthestChapter to " + value);
            _furthestChapterReached.val = value;
            // }
            // else
            // {
            //     Debug.LogWarning(this.GetType() + " tried to update furthestChapter to " + value + " <= " + _furthestChapterReached.val);
            // }
        }
    }

    public static bool isAdmin
    {
        get
        {
            return _isAdmin.val;
        }
        set // should only be called by BackEndManager
        {
            if (value != _isAdmin.val)
            {
                _isAdmin.val = value;
                RedMetricsManager.get().sendEvent(TrackingEvent.SWITCHFROMGAMEVERSION, RedMetricsManager.get().generateCustomDataForGuidInit());
                MemoryManager.get().configuration.setMetricsDestination(!value);
                RedMetricsManager.get().sendEvent(TrackingEvent.SWITCHTOGAMEVERSION, RedMetricsManager.get().generateCustomDataForGuidInit());
            }
        }
    }

    public float[] bestTimes
    {
        get
        {
            // Debug.Log(this.GetType() + " getting _bestTimes");
            initializeBestTimesIfNecessary();
            float[] result = new float[Scorekeeper.completionsCount];
            for (int index = 0; index < Scorekeeper.completionsCount; index++)
            {
                result[index] = _bestTimes[index].val;
            }
            return result;
        }
        set
        {
            // Debug.Log(this.GetType() + " trying to set _bestTimes...");
            initializeBestTimesIfNecessary();
            for (int index = 0; index < Scorekeeper.completionsCount; index++)
            {
                if (value[index] < _bestTimes[index].val)
                {
                    // Debug.Log(this.GetType() + " setting _bestTimes[" + index + "] to " + value[index] + "(previously " + _bestTimes[index].val + ")");
                    _bestTimes[index].val = value[index];
                }
            }
        }
    }

    public void setBestTime(int index, float time, bool force = false)
    {
        // Debug.Log(this.GetType() + " setBestTime(" + index + ", " + time + ", " + force + ")");
        if (force || time < _bestTimes[index].val)
        {
            // Debug.Log(this.GetType() + " setBestTime updates " + index + " to " + time);
            _bestTimes[index].val = time;
        }
    }

    private const string _restartBehaviorKey = "restartBehavior";
    private const string _gameMapKey = "gameMap";
    private const string _isAbsoluteWASDKey = "isAbsoluteWASD";
    private const string _isLeftClickToMoveKey = "isLeftClickToMove";
    private const string _isSoundOnKey = "isSoundOn";
    private const string _furthestChapterReachedKey = "furthestChapterReached";
    private const string _bestCompletionTimeChapterStem = "bestCompletionTimeChapter";
    private const string _isAdminKey = "isAdmin";

    public void load()
    {
        // Debug.Log(this.GetType() + " load");

        RedMetricsManager.get().localPlayerGUID = playerGUID;

        _isAdmin.initialize();
        initializeGameVersionGUID();
        _restartBehavior.initialize();
        // _gameMap.initialize();
        _isAbsoluteWASD.initialize();
        _isLeftClickToMove.initialize();
        _isSoundOn.initialize();
        _furthestChapterReached.initialize();

        // Debug.Log(this.GetType() + " load done");
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    ///REDMETRICS TRACKING ///////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////
    //test
    // public const string testVersionGUID = "83f99dfa-bd87-43e1-940d-f28bbcea5b1d";
    // redmetrics.io
    // public const string testVersionGUIDString = "83f99dfa-bd87-43e1-940d-f28bbcea5b1d";
    // http://redmetrics.crigamelab.org/ same as previous
    public const string testVersionGUIDString = "a98af628-e06e-46ec-98dd-275636dd11ae";    
    public System.Guid testVersionGUID = new System.Guid(testVersionGUIDString);
    // v 1.0
    // private const string gameVersionGuid = "\"99a00e65-6039-41a3-a85b-360c4b30a466\"";
    // v 1.31
    // private const string gameVersionGuid = "\"5832732e-6bfb-4ac7-8df4-270c6f20b72a\"";
    // v 1.32
    // public const string labelledGameVersionGUID = "be209fe8-0ef3-4291-a5f4-c2b389f5d77d";
    // v 1.33
    // public const string labelledGameVersionGUID = "51b8a78a-8dd3-4a5e-9f41-01e6805e0f52";
    // v 1.40
    // public const string labelledGameVersionGUID = "81ee441f-6cbc-45ba-a306-160905c80b97";
    // v1.50
    // public const string labelledGameVersionGUID = "fef94d5f-d99a-4212-9f21-87308293fb03";
    // v1.51
    // public System.Guid labelledGameVersionGUID = new System.Guid("043c1977-93bf-4991-804e-53366d2b718b");
    // v1.52
    // public const string labelledGameVersionGUIDString = "915953b4-f9e1-41ca-acc4-4e4e90667102";
    // v1.60 redmetrics.io
    // public const string labelledGameVersionGUIDString = "284308a6-3c3b-4355-aab3-ee0518f4c42b";
    // v1.61 http://redmetrics.crigamelab.org/ same as 1.60
    // moved to http://redmetrics.io/ but with same content
    public const string labelledGameVersionGUIDString = "ceee388e-aba7-4554-b4ea-a83210e00790";
    public System.Guid labelledGameVersionGUID = new System.Guid(labelledGameVersionGUIDString);

    //public string defaultPlayer = "b5ab445a-56c9-4c5b-a6d0-86e8a286cd81";

    public void reset()
    {
        Debug.Log(this.GetType() + " reset");
        furthestChapter = 0;
        setWebGUID(null);
#if UNITY_WEBGL
        Debug.Log(this.GetType() + " reset Application.ExternalCall(deleteLocalPlayerGUID);");
        Application.ExternalCall("deleteLocalPlayerGUID");
#endif
        _playerGUID = null;
        PlayerPrefs.DeleteAll ();
        load();
        RedMetricsManager.get().sendStartEvent(true);
    }

    private string _webGUID;
    public void setWebGUID(string webGUID)
    {
        // Debug.Log(this.GetType() + " setWebGUID(" + webGUID + ")");
        _webGUID = webGUID;
    }

    private string _playerGUID;
    public string playerGUID
    {
        get
        {
            // Debug.Log(this.GetType() + " playerGUID get");
            if (string.IsNullOrEmpty(_playerGUID))
            {
                // Debug.Log(this.GetType() + " playerGUID get string.IsNullOrEmpty(_playerGUID)");
                //TODO make it work through different versions of the game,
                //     so that memory is not erased every time a new version of the game is published
                string storedGUID = PlayerPrefs.GetString(_localPlayerGUIDPlayerPrefsKey);
                if (string.IsNullOrEmpty(storedGUID))
                {
#if UNITY_WEBGL
				    if(!string.IsNullOrEmpty(_webGUID))
                    {
                        // Debug.Log(this.GetType() + " playerGUID get !string.IsNullOrEmpty(_webGUID)");
                        _playerGUID = _webGUID;
                        PlayerPrefs.SetString(_localPlayerGUIDPlayerPrefsKey, _playerGUID);
                    }
                    else
                    {
#endif
                        // Debug.Log(this.GetType() + " playerGUID get string.IsNullOrEmpty(storedGUID)");
                        _playerGUID = Guid.NewGuid().ToString();
                        PlayerPrefs.SetString(_localPlayerGUIDPlayerPrefsKey, _playerGUID);
#if UNITY_WEBGL
                        Debug.Log(this.GetType() + " playerGUID get Application.ExternalCall(initializeGUIDState);");
                        Application.ExternalCall("initializeGUIDState");
                    }
#endif

                }
                else
                {
                    // Debug.Log(this.GetType() + " playerGUID get _playerGUID = storedGUID");
                    _playerGUID = storedGUID;
                }
            }
            // Debug.Log(this.GetType() + " playerGUID get returns " + _playerGUID);
            return _playerGUID;
        }
    }

    public void initializeGameVersionGUID()
    {
        // Debug.Log(this.GetType() + " initializeGameVersionGUID with RedMetricsManager.gameVersion=" + RedMetricsManager.get().getGameVersion());
        if (!RedMetricsManager.get().isGameVersionInitialized())
        {
            setMetricsDestination(!isAdmin);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " initializeGameVersionGUID RedMetricsManager GameVersion already initialized");
        }
        // Debug.Log(this.GetType() + " initializeGameVersionGUID done with"
        // + " guid=" + RedMetricsManager.get().getGameVersion()
        // + ", isAdmin=" + isAdmin
        // + ", isTestGUID=" + isTestGUID()
        // );
    }

    // use: check that stored guid is not from previous version
    private bool isGUIDCorrect(string guid)
    {
        // Debug.Log(this.GetType() + " initializeGameVersionGUID("+guid+")");
        return (!string.IsNullOrEmpty(guid)) && (guid == labelledGameVersionGUIDString || guid == testVersionGUIDString);
    }

    //sets the destination to which logs will be sent
    public void setMetricsDestination(bool wantToBecomeLabelledGameVersion)
    {
        // Debug.Log(this.GetType() + " setMetricsDestination(" + wantToBecomeLabelledGameVersion + ")");
        System.Guid guid = wantToBecomeLabelledGameVersion ? labelledGameVersionGUID : testVersionGUID;

        if (guid != RedMetricsManager.get().getGameVersion())
        {
            // Debug.Log(this.GetType() + " setMetricsDestination " + wantToBecomeLabelledGameVersion);
            
            // Debug.Log(this.GetType() + " gameVersionGUID set calls setGameVersion");
            setGameVersion(guid);
        }
        // Debug.Log(this.GetType() + " setMetricsDestination(" + wantToBecomeLabelledGameVersion + ") done");
    }

    private void setGameVersion(System.Guid guid)
    {
        // Debug.Log(this.GetType() + " setGameVersion " + guid);
        RedMetricsManager.get().setGameVersion(guid);
        if (isTestGUID() != isAdmin)
        {
            Debug.LogWarning(this.GetType() + " setGameVersion: incorrect status isTestGUID() != isAdmin");
        }
    }

    public bool isTestGUID()
    {
        // Debug.Log(this.GetType() + " isTestGUID");
        return testVersionGUID == RedMetricsManager.get().getGameVersion();
    }

    public GameMode getMode()
    {
        return getMode(gameMap);
    }

    public static GameMode getMode(GameMap map)
    {
        switch (map)
        {
            // case GameMap.ADVENTURE1:
#if !TUTORIAL2
            case GameMap.TUTORIAL1:
#else
            case GameMap.TUTORIAL2:
#endif
                return GameMode.ADVENTURE;
            // case GameMap.SANDBOX1:
            case GameMap.SANDBOX2:
                return GameMode.SANDBOX;
            default:
                Debug.LogError("unknown map " + map);
                return GameMode.ADVENTURE;
        }
    }

    public string getSceneName()
    {
        return getSceneName(gameMap);
    }

    public static string getSceneName(GameMap map)
    {
        switch (map)
        {
            // case GameMap.ADVENTURE1:
            //     return _adventureLevel1;
            // case GameMap.SANDBOX1:
            //     return _sandboxLevel1;
            case GameMap.SANDBOX2:
                return _sandboxLevel2;
#if !TUTORIAL2
            case GameMap.TUTORIAL1:
#else
            case GameMap.TUTORIAL2:
#endif
                return _tutorial;
            default:
                Debug.LogError("unknown map " + map);
                return _adventureLevel1;
        }
    }

    public override string ToString()
    {
        return string.Format("[GameConfiguration: restartBehavior={0}, gameMap={1}]", restartBehavior, gameMap);
    }
}

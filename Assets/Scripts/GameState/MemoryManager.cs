using UnityEngine;
using System.Collections.Generic;

public class MemoryManager : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "MemoryManager";
    private static MemoryManager _instance;
    public static MemoryManager get(string origin = "")
    {
        if (_instance == null)
        {
            // Debug.Log("MemoryManager::get badly initialized called by " + origin);
            _instance = GameObject.Find(gameObjectName).GetComponent<MemoryManager>();
            if (null != _instance)
            {
                _instance.initializeIfNecessary();
            }
            else
            {
                Debug.LogError("MemoryManager::get couldn't find game object");
            }
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log("MemoryManager::Awake");
        if (null != _instance && this != _instance)
        {
            Debug.LogWarning("MemoryManager::antiDuplicateInitialization self-destruction");
            Destroy(this.gameObject);
        }
        else
        {
            initializeIfNecessary();
        }
    }

    private bool _initialized = false;

    void Start()
    {
        // Debug.Log("MemoryManager::Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private GameConfiguration _configuration;
    public GameConfiguration configuration
    {
        get
        {
            if (null == _configuration)
            {
                _configuration = new GameConfiguration();
            }
            return _configuration;
        }
    }

    public Hero hero;
    public string[] inputFiles;
    private Dictionary<string, string> _savedData = new Dictionary<string, string>();
    private Dictionary<string, LevelInfo> _loadedLevelInfo = new Dictionary<string, LevelInfo>();
    private const string playerDataKey = "player";

    private void initializeIfNecessary(bool onlyIfEmpty = true)
    {
        if (!_initialized)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            Logger.Log("MemoryManager::initializeIfNecessary", Logger.Level.DEBUG);
            if (!onlyIfEmpty || 0 == _loadedLevelInfo.Count)
            {
                loadLevelData(inputFiles, _loadedLevelInfo);

                string playerGUID = configuration.playerGUID;
                Logger.Log(
                    "MemoryManager::initializeIfNecessary: playerGUID=" + playerGUID
                    + " & configuration.isTestGUID()=" + configuration.isTestGUID()
                    + " & Application.isEditor=" + Application.isEditor
                );

                //TODO manage RedMetricsManager's globalPlayerGUID
                RedMetricsManager.get().setLocalPlayerGUID(playerGUID);
                RedMetricsManager.get().sendStartEvent();
                Logger.Log(string.Format("MemoryManager::initializeIfNecessary initial game configuration={0}, labelledGameVersionGUID={1}, playerGUID={2}"
                                         , configuration, GameConfiguration.labelledGameVersionGUID, playerGUID)
                           , Logger.Level.INFO);
            }
            _initialized = true;
        }
    }

    public bool addData(string key, string value)
    {
        if (!_savedData.ContainsKey(key))
        {
            _savedData.Add(key, value);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool addOrUpdateData(string key, string value)
    {
        Logger.Log("MemoryManager::addOrUpdateData", Logger.Level.DEBUG);
        if (_savedData.ContainsKey(key))
        {
            _savedData.Remove(key);
        }
        return addData(key, value);
    }

    public bool tryGetData(string key, out string value)
    {
        Logger.Log("MemoryManager::tryGetData", Logger.Level.DEBUG);
        return _savedData.TryGetValue(key, out value);
    }

    private void loadLevelData(string[] inputFiles, Dictionary<string, LevelInfo> dico)
    {
        Logger.Log("MemoryManager::loadLevelData", Logger.Level.DEBUG);
        FileLoader loader = new FileLoader();

        foreach (string file in inputFiles)
        {
            LinkedList<LevelInfo> lis = loader.loadObjectsFromFile<LevelInfo>(file, LevelInfoXMLTags.INFO);
            if (null != lis)
            {
                foreach (LevelInfo li in lis)
                {
                    dico.Add(li.code, li);
                }
            }

        }
    }

    private LevelInfo retrieveFromDico(string code)
    {
        Logger.Log("MemoryManager::retrieveFromDico", Logger.Level.DEBUG);
        LevelInfo info;
        //TODO set case-insensitive
        if (!_loadedLevelInfo.TryGetValue(code, out info))
        {
            Logger.Log("MemoryManager::retrieveFromDico(" + code + ") failed", Logger.Level.WARN);
            info = null;
        }
        return info;
    }

    public bool tryGetCurrentLevelInfo(out LevelInfo levelInfo)
    {
        Logger.Log("MemoryManager::tryGetCurrentLevelInfo", Logger.Level.DEBUG);
        levelInfo = null;
        return _loadedLevelInfo.TryGetValue(MemoryManager.get().configuration.getSceneName(), out levelInfo);
    }

    void OnDestroy()
    {
        Logger.Log("MemoryManager::OnDestroy", Logger.Level.DEBUG);
    }

    public void sendCompletionEvent()
    {
        RedMetricsManager.get().sendEvent(TrackingEvent.COMPLETE);
    }
}

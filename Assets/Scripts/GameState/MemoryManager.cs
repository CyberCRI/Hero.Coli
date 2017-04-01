using UnityEngine;
using System.Collections.Generic;

public class MemoryManager : MonoBehaviour
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "MemoryManager";
    private static MemoryManager _instance;
    public static MemoryManager get(string origin = "")
    {
        if (_instance == null)
        {
            // Debug.Log("MemoryManager get badly initialized called by " + origin);
            _instance = GameObject.Find(gameObjectName).GetComponent<MemoryManager>();
            if (null == _instance)
            {
                Debug.LogError("MemoryManager get couldn't find game object");
            }
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            // Debug.LogWarning(this.GetType() + " anti duplication self-destruction");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    private void initializeIfNecessary(bool onlyIfEmpty = true)
    {
        if (!_initialized)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Debug.Log(this.GetType() + " initializeIfNecessary");

            if (!onlyIfEmpty || 0 == _loadedLevelInfo.Count)
            {
                loadLevelData(inputFiles, _loadedLevelInfo);
            }
            _initialized = true;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");

        //TODO manage RedMetricsManager's globalPlayerGUID

        // Debug.Log(this.GetType() + " configuration.load()");

        configuration.load();

        // Debug.Log(this.GetType() + "configuration.gameVersionGUID = " + configuration.gameVersionGUID);

        RedMetricsManager.get().sendStartEvent();

        // Debug.Log(string.Format(this.GetType() + " Start initial game configuration={0}, labelledGameVersionGUID={1}, playerGUID={2}", configuration, GameConfiguration.labelledGameVersionGUID, playerGUID));
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

    public string[] inputFiles;
    [HideInInspector]
	public int checkpointIndex = 0;
    private Dictionary<string, string> _savedData = new Dictionary<string, string>();
    private Dictionary<string, LevelInfo> _loadedLevelInfo = new Dictionary<string, LevelInfo>();
    private const string playerDataKey = "player";

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
        // Debug.Log(this.GetType() + " addOrUpdateData");
        if (_savedData.ContainsKey(key))
        {
            _savedData.Remove(key);
        }
        return addData(key, value);
    }

    public bool tryGetData(string key, out string value)
    {
        // Debug.Log(this.GetType() + " tryGetData");
        return _savedData.TryGetValue(key, out value);
    }

    private void loadLevelData(string[] inputFiles, Dictionary<string, LevelInfo> dico)
    {
        // Debug.Log(this.GetType() + " loadLevelData");
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
        // Debug.Log(this.GetType() + " retrieveFromDico");
        LevelInfo info;
        //TODO set case-insensitive
        if (!_loadedLevelInfo.TryGetValue(code, out info))
        {
            Debug.LogWarning(this.GetType() + " retrieveFromDico(" + code + ") failed");
            info = null;
        }
        return info;
    }

    public bool tryGetCurrentLevelInfo(out LevelInfo levelInfo)
    {
        // Debug.Log(this.GetType() + " tryGetCurrentLevelInfo");
        levelInfo = null;
        return _loadedLevelInfo.TryGetValue(MemoryManager.get().configuration.getSceneName(), out levelInfo);
    }

    public void sendCompletionEvent()
    {
        RedMetricsManager.get().sendRichEvent(TrackingEvent.COMPLETE);
    }
}

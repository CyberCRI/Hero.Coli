using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoryManager : MonoBehaviour {
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "MemoryManager";
    private static MemoryManager _instance;
    public static MemoryManager get() {
        if (_instance == null)
        {
            Logger.Log("MemoryManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<MemoryManager>();
            if(null != _instance)
            {
                DontDestroyOnLoad(_instance.gameObject);
                _instance.initializeIfNecessary();
            }
            else
            {
                Logger.Log("MemoryManager::get couldn't find game object", Logger.Level.ERROR);
            }
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("MemoryManager::Awake", Logger.Level.INFO);
        antiDuplicateInitialization();
    }

    void Start ()
    {
        Logger.Log("MemoryManager::Start", Logger.Level.INFO);
    }
    
    void antiDuplicateInitialization()
    {
        MemoryManager.get ();
        Logger.Log("MemoryManager::antiDuplicateInitialization with hashcode="+this.GetHashCode()+" and _instance.hashcode="+_instance.GetHashCode(), Logger.Level.INFO);
        if(this != _instance) {
            Logger.Log("MemoryManager::antiDuplicateInitialization self-destruction", Logger.Level.INFO);
            Destroy(this.gameObject);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private GameConfiguration _configuration;
    public GameConfiguration configuration {
        get {
            if(null == _configuration)
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
    public string playerDataKey = "player";

    private void initializeIfNecessary(bool onlyIfEmpty = true)
    {
        Logger.Log("MemoryManager::initializeIfNecessary", Logger.Level.DEBUG);
        if(!onlyIfEmpty || 0 == _loadedLevelInfo.Count)
        {
            loadLevelData(inputFiles, _loadedLevelInfo);
            
            setTestGUID();
            
            string playerGUID = configuration.playerGUID;
            Debug.LogError(
                "MemoryManager::initializeIfNecessary: playerGUID="+playerGUID
                +" & testGUIDs.contains(playerGUID)="+configuration.isTestGUID()
                +" & Application.isEditor="+Application.isEditor
            );
            
            //TODO manage RedMetricsManager's globalPlayerGUID
            RedMetricsManager.get ().setLocalPlayerGUID(playerGUID);
            RedMetricsManager.get ().sendStartEvent();
            Logger.Log(string.Format("MemoryManager::initializeIfNecessary initial game configuration={0}, labelledGameVersionGUID={1}, playerGUID={2}"
                                     , configuration, GameConfiguration.labelledGameVersionGUID, playerGUID)
                       , Logger.Level.INFO);
        }
    }
    
    
    //if the game is launched in the editor,
    // sets the localPlayerGUID to a test GUID 
    // so that events are logged onto a test version
    // instead of the regular game version
    // to prevent data from being contaminated by tests
    private void setTestGUID() {
        if (Application.isEditor) {
            MemoryManager.get().configuration.setMetricLogDestination(true);
        }
    }

    public bool addData(string key, string value)
    {
        if(!_savedData.ContainsKey(key))
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
        if(_savedData.ContainsKey(key))
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
            LinkedList<LevelInfo> lis = loader.loadObjectsFromFile<LevelInfo>(file,LevelInfoXMLTags.INFO);
            if(null != lis)
            {
                foreach( LevelInfo li in lis)
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
        if(!_loadedLevelInfo.TryGetValue(code, out info))
        {
            Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
            info = null;
        }
        return info;
    }

    public bool tryGetCurrentLevelInfo(out LevelInfo levelInfo)
    {
        Logger.Log("MemoryManager::tryGetCurrentLevelInfo", Logger.Level.DEBUG);
        levelInfo = null;;
        return _loadedLevelInfo.TryGetValue(MemoryManager.get ().configuration.getSceneName (), out levelInfo);
    }

    void OnDestroy()
    {
        Logger.Log("MemoryManager::OnDestroy", Logger.Level.DEBUG);
    }
   
    public void sendCompletionEvent()
    {
        RedMetricsManager.get ().sendEvent(TrackingEvent.COMPLETE);
    }
}

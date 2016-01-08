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
            
            string playerGUID = MemoryManager.get().configuration.playerGUID;
            Debug.LogError(
                "MemoryManager::initializeIfNecessary: playerGUID="+playerGUID
                +" & testGUIDs.contains(playerGUID)="+testGUIDs.Contains(playerGUID)
                +" & Application.isEditor="+Application.isEditor
            );
            
            //if(Application.isEditor || testGUIDs.Contains(playerGUID) ) {
            if(testGUIDs.Contains(playerGUID) ) {
                RedMetricsManager.get ().setGameVersion(testVersionGuid);
            } else {
                RedMetricsManager.get ().setGameVersion(gameVersionGuid);
            }

            //TODO manage RedMetricsManager's globalPlayerGUID
            RedMetricsManager.get ().setLocalPlayerGUID(playerGUID);
            RedMetricsManager.get ().sendStartEvent();
            Logger.Log(string.Format("MemoryManager::initializeIfNecessary initial game configuration={0}, gameVersionGuid={1}, playerGUID={2}"
                                     , configuration, gameVersionGuid, playerGUID)
                       , Logger.Level.INFO);
        }
    }
    
    
    //if the game is launched in the editor,
    // sets the localPlayerGUID to a test GUID 
    // so that events are logged onto a test version
    // instead of the regular game version
    // to prevent data from being contaminated by tests
    private void setTestGUID() {
        if(Application.platform == RuntimePlatform.WindowsEditor) {
            MemoryManager.get().configuration.playerGUID = MemoryManager.testGUIDWindows;
        } else if (Application.platform == RuntimePlatform.OSXEditor) {
            MemoryManager.get().configuration.playerGUID = MemoryManager.testGUIDMacOS;
        } else if (Application.isEditor) {
            MemoryManager.get().configuration.playerGUID = MemoryManager.testGUIDOther;
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



    //////////////////////////////////////////////////////////////////////////////////////////////////
    ///REDMETRICS TRACKING ///////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////
    //test
    public string testVersionGuid = "83f99dfa-bd87-43e1-940d-f28bbcea5b1d";    
    //v1.0
    //private static string gameVersionGuid = "\"99a00e65-6039-41a3-a85b-360c4b30a466\"";
    //v1.31
    //private static string gameVersionGuid = "\"5832732e-6bfb-4ac7-8df4-270c6f20b72a\"";
    //v1.32
    public string gameVersionGuid = "be209fe8-0ef3-4291-a5f4-c2b389f5d77d";
    //v1.33
    //public string gameVersionGuid = "51b8a78a-8dd3-4a5e-9f41-01e6805e0f52";
    
    //public string defaultPlayer = "b5ab445a-56c9-4c5b-a6d0-86e8a286cd81";
    
    
    // GUIDs used for tests - should send events to the 'Hero.Coli - test' section
    public static string testGUIDMacOS   = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
    public static string testGUIDWindows = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";
    public static string testGUIDOther   = "cccccccc-cccc-cccc-cccc-cccccccccccc";
    public List<string> testGUIDs = new List<string>(){testGUIDMacOS, testGUIDWindows,testGUIDOther};
  
    public void sendCompletionEvent()
    {
        RedMetricsManager.get ().sendEvent(TrackingEvent.COMPLETE);
    }
}

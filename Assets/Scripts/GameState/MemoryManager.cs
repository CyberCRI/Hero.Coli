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
        Logger.Log("MemoryManager::Awake", Logger.Level.DEBUG);
        MemoryManager.get ();
    }

    void Start ()
    {
        createPlayer (www => trackStart(www));
    }
    ////////////////////////////////////////////////////////////////////////////////////////////
    
    public string[] inputFiles;
    private Dictionary<string, string> _savedData = new Dictionary<string, string>();
    private Dictionary<string, LevelInfo> _loadedLevelInfo = new Dictionary<string, LevelInfo>();

    private void initializeIfNecessary(bool onlyIfEmpty = true)
    {
        Logger.Log("MemoryManager::initializeIfNecessary", Logger.Level.DEBUG);
        if(!onlyIfEmpty || 0 == _loadedLevelInfo.Count)
        {
            loadLevelData(inputFiles, _loadedLevelInfo);
            GameStateController.get ().setAndSaveLevelName(GameStateController._adventureLevel1);
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
    
    private static LevelInfo retrieveFromDico(string code)
    {
        Logger.Log("MemoryManager::retrieveFromDico", Logger.Level.DEBUG);
        LevelInfo info;
        //TODO set case-insensitive
        if(!_instance._loadedLevelInfo.TryGetValue(code, out info))
        {
            Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
            info = null;
        }
        return info;
    }

    public bool tryGetCurrentLevelInfo(out LevelInfo levelInfo)
    {
        Logger.Log("MemoryManager::tryGetCurrentLevelInfo", Logger.Level.DEBUG);
        levelInfo = null;
        string currentLevelCode;
        if(tryGetData(GameStateController._currentLevelKey, out currentLevelCode))
        {
            return _loadedLevelInfo.TryGetValue(currentLevelCode, out levelInfo);
        }
        else
        {
            //defensive code
            GameStateController.get ().setAndSaveLevelName(GameStateController._adventureLevel1);

            return false;
        }
    }

    void OnDestroy()
    {
        Logger.Log("MemoryManager::OnDestroy", Logger.Level.DEBUG);
    }



    //////////////////////////////////////////////////////////////////////////////////////////////////
    ///REDMETRICS TRACKING ///////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////

    private string redMetricsURL = "https://api.redmetrics.io/v1/";
    private string redMetricsEvent = "event";
    private string redMetricsPlayer = "player";
    private string gameVersion = "\"99a00e65-6039-41a3-a85b-360c4b30a466\"";
    private string playerID = "\"b5ab445a-56c9-4c5b-a6d0-86e8a286cd81\"";
    
    private string createPlayerEventType = "\"newPlayer\"";
    private string startEventType = "\"start\"";
    
    void setPlayerID (string pID)
    {
        Debug.Log("setPlayerID("+pID+")");
        playerID = pID;
    }
    
    private void createEvent (string eventType)
    {
        string url = redMetricsURL + redMetricsEvent;
        Dictionary<string, string> headers = new Dictionary<string, string> ();
        headers.Add ("Content-Type", "application/json");
        
        string ourPostData = "{\"gameVersion\":" + gameVersion + "," +
            "\"player\":" + playerID + "," +
                "\"type\":"+eventType+"}";
        byte[] pData = System.Text.Encoding.ASCII.GetBytes (ourPostData.ToCharArray ());
        Debug.Log("StartCoroutine...");
        StartCoroutine (RedMetricsManager.POST (url, pData, headers, value => wwwLogger(value)));
    }
    
    private void createPlayer (System.Action<WWW> callback)
    {
        string url = redMetricsURL + redMetricsPlayer;
        Dictionary<string, string> headers = new Dictionary<string, string> ();
        headers.Add ("Content-Type", "application/json");
        string ourPostData = "{\"type\":"+createPlayerEventType+"}";
        byte[] pData = System.Text.Encoding.ASCII.GetBytes (ourPostData.ToCharArray ());
        Debug.Log("StartCoroutine...");
        StartCoroutine (RedMetricsManager.POST (url, pData, headers, callback));
    }
    
    private void wwwLogger (WWW www)
    {
        if (www.error == null) {
            Debug.LogError ("Success: " + www.text);
        } else {
            Debug.LogError ("Error: " + www.error);
        } 
    }
    
    private string extractPID(WWW www)
    {
        string result = null;
        wwwLogger(www);
        string trimmed = www.text.Trim ();
        string[] split1 = trimmed.Split('\n');
        foreach(string s1 in split1)
        {
            Debug.Log(s1);
            if(s1.Length > 5)
            {
                string[] split2 = s1.Trim ().Split(':');
                foreach(string s2 in split2)
                {
                    if(!s2.Equals("id")){
                        Debug.Log ("id =? "+s2);
                        result = s2;
                    }
                }
            }
        }
        return result;
    }
    
    private void trackStart(WWW www)
    {
        string pID = extractPID(www);
        setPlayerID(pID);
        createEvent(startEventType);
    }
}

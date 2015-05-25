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
    ////////////////////////////////////////////////////////////////////////////////////////////

    void antiDuplicateInitialization()
    {
        MemoryManager.get ();
        Logger.Log("MemoryManager::antiDuplicateInitialization with hashcode="+this.GetHashCode()+" and _instance.hashcode="+_instance.GetHashCode(), Logger.Level.INFO);
        _instance.sendStartEvent(this != _instance);
        if(this != _instance) {
            Logger.Log("MemoryManager::antiDuplicateInitialization self-destruction", Logger.Level.INFO);
            Destroy(this.gameObject);
        }
    }

    public Hero hero;
    public string[] inputFiles;
    private Dictionary<string, string> _savedData = new Dictionary<string, string>();
    private Dictionary<string, LevelInfo> _loadedLevelInfo = new Dictionary<string, LevelInfo>();
    private string _playerDataKey = "player";

    private void sendStartEvent(bool switched)
    {
        Logger.Log ("MemoryManager::sendStartEvent", Logger.Level.INFO);
        
        MemoryManager.get ();

        // management of game start for webplayer
        if(switched) {
            //event is sent from somewhere else
            /*
          string currentLevelName = "?";
          LevelInfo levelInfo;
          bool success = tryGetCurrentLevelInfo(out levelInfo);
          if(success && null != levelInfo)
          {
              currentLevelName = levelInfo.code;
          }
          sendEvent(TrackingEvent.SWITCH, currentLevelName);
          */
        } else {
            connect ();
            StartCoroutine(waitAndSendStart());
        }

        // management of game start for standalone
        /*
        string pID = null;
        bool tryGetPID = tryGetData(_playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Logger.Log ("MemoryManager::sendStartEvent player already identified - pID="+pID, Logger.Level.INFO);
            string currentLevelName = "?";
            LevelInfo levelInfo;
            bool success = tryGetCurrentLevelInfo(out levelInfo);
            if(success && null != levelInfo)
            {
                currentLevelName = levelInfo.code;
            }
            sendEvent(TrackingEvent.SWITCH,currentLevelName);
        } else {
            createPlayer (www => trackStart(www));
            //testGet (www => trackStart(www));
        }
        */

    }

    private IEnumerator waitAndSendStart() {
        yield return new WaitForSeconds(5.0f);
        sendEvent(TrackingEvent.START);
    }

    private string innerCreateJsonForRedMetrics(string eventCode, string customData, string section, string coordinates)
    {
        string eventCodePart = "", customDataPart = "", sectionPart = "", coordinatesPart = "";
        
        eventCodePart = "\"type\":\"";
        if(string.IsNullOrEmpty(eventCode)) {
            eventCodePart+="unknown";
        } else {
            eventCodePart+=eventCode;
        }
        eventCodePart+="\"";
        
        if(!string.IsNullOrEmpty(customData)) {
            customDataPart = ",\"customData\":\""+customData+"\"";
        }
        
        if(!string.IsNullOrEmpty(section)) {
            sectionPart = ",\"section\":\""+section+"\"";
        } else {
            if(null != hero && !string.IsNullOrEmpty(hero.getLastCheckpointName())) {
                sectionPart = ",\"section\":\""+hero.getLastCheckpointName()+"\"";
            }
        }
        
        if(!string.IsNullOrEmpty(coordinates)) {
            coordinatesPart = ",\"coordinates\":\""+coordinates+"\"";
        }
        
        return eventCodePart+customDataPart+sectionPart+coordinatesPart+"}";
    }
    
    private string createJsonForRedMetrics(string eventCode, string customData, string section, string coordinates)
    {
        string jsonPrefix = "{\"gameVersion\":" + gameVersion + "," +
            "\"player\":";
        string jsonSuffix = innerCreateJsonForRedMetrics(eventCode, customData, section, coordinates);
        
        string pID = null;
        bool tryGetPID = tryGetData(_playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Logger.Log ("MemoryManager::sendEvent player already identified - pID="+pID, Logger.Level.INFO);

        } else {
            Logger.Log ("MemoryManager::sendEvent no registered player!", Logger.Level.ERROR);
            pID = defaultPlayerID;
        }
        return jsonPrefix+pID+","+jsonSuffix;
        //sendData(redMetricsEvent, ourPostData, value => wwwLogger(value, "sendEvent("+eventCode+")"));
    }
    
    private string createJsonForRedMetricsJS(string eventCode, string customData, string section, string coordinates)
    {
        return "{"+innerCreateJsonForRedMetrics(eventCode, customData, section, coordinates);
    }
    
    // see github.com/CyberCri/RedMetrics.js
    // with type -> eventCode
    public void sendEvent(TrackingEvent trackingEvent, string customData = null, string section = null, string coordinates = null)
    {
        //TODO test on build type:
        // if webplayer, then use Application.ExternalCall("rmConnect", json);
        // else if standalone, then use WWW
        // else ... ?

        string json = createJsonForRedMetricsJS(trackingEvent.ToString().ToLower(), customData, section, coordinates);
        Logger.Log ("MemoryManager::sendEvent will rmPostEvent json="+json, Logger.Level.INFO);
        Application.ExternalCall("rmPostEvent", json);

        /*
        string pID = null;
        bool tryGetPID = tryGetData(_playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Logger.Log ("MemoryManager::sendEvent player already identified - pID="+pID, Logger.Level.INFO);
            string ourPostData = "{\"gameVersion\":" + gameVersion + "," +
                "\"player\":" + playerID + "," +
                    "\"type\":\""+eventCode+"\"}";
            sendData(redMetricsEvent, ourPostData, value => wwwLogger(value, "sendEvent("+eventCode+")"));
        } else {
            Logger.Log ("MemoryManager::sendEvent no registered player!", Logger.Level.ERROR);
        }
        */
    }

    public void sendCompletionEvent()
    {
        sendEvent(TrackingEvent.COMPLETE);
    }

    public static void connect()
    {
        string json = "{\"gameVersionId\": "+gameVersion+"}";
        Logger.Log ("MemoryManager::connect will rmConnect json="+json, Logger.Level.ERROR);
        Application.ExternalCall("rmConnect", json);
    }

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
    private string redMetricsPlayer = "player";
    //private static string gameVersion = "\"99a00e65-6039-41a3-a85b-360c4b30a466\"";
    private static string gameVersion = "\"5832732e-6bfb-4ac7-8df4-270c6f20b72a\"";
    private static string defaultPlayerID = "\"b5ab445a-56c9-4c5b-a6d0-86e8a286cd81\"";
    private string playerID = defaultPlayerID;
        
    private string createPlayerEventType = "newplayer";
    
    void setPlayerID (string pID)
    {
        Logger.Log("setPlayerID("+pID+")", Logger.Level.INFO);
        playerID = pID;
    }

    private void sendData(string urlSuffix, string pDataString, System.Action<WWW> callback)
    {
        string url = redMetricsURL + urlSuffix;
        Dictionary<string, string> headers = new Dictionary<string, string> ();
        headers.Add ("Content-Type", "application/json");
        byte[] pData = System.Text.Encoding.ASCII.GetBytes (pDataString.ToCharArray ());
        Logger.Log("MemoryManager::sendData StartCoroutine POST with data="+pDataString+" ...", Logger.Level.INFO);
        StartCoroutine (RedMetricsManager.POST (url, pData, headers, callback));
    }
    
    private void createPlayer (System.Action<WWW> callback)
    {
        string ourPostData = "{\"type\":"+createPlayerEventType+"}";
        sendData(redMetricsPlayer, ourPostData, callback);
    }

    private void testGet(System.Action<WWW> callback)
    {
        Logger.Log("testGet", Logger.Level.INFO);
        string url = redMetricsURL + redMetricsPlayer;
        StartCoroutine (RedMetricsManager.GET (url, callback));
    }
    
    private void wwwLogger (WWW www, string origin = "default")
    {
        if(null == www) {
            Logger.Log("MemoryManager::wwwLogger null == www from "+origin, Logger.Level.ERROR);
        } else {
          if (www.error == null) {
                Logger.Log("MemoryManager::wwwLogger Success: " + www.text + " from "+origin, Logger.Level.ERROR);
          } else {
                Logger.Log("MemoryManager::wwwLogger Error: " + www.error + " from "+origin, Logger.Level.ERROR);
          } 
        }
    }
    
    private string extractPID(WWW www)
    {
        string result = null;
        wwwLogger(www, "extractPID");
        string trimmed = www.text.Trim ();
        string[] split1 = trimmed.Split('\n');
        foreach(string s1 in split1)
        {
            //Debug.Log(s1);
            if(s1.Length > 5)
            {
                string[] split2 = s1.Trim ().Split(':');
                foreach(string s2 in split2)
                {
                    if(!s2.Equals("id")){
                        //Debug.Log ("id =? "+s2);
                        result = s2;
                    }
                }
            }
        }
        return result;
    }
    
    private void trackStart(WWW www)
    {
        Logger.Log("trackStart: www =? null:"+(null == www), Logger.Level.INFO);
        string pID = extractPID(www);
        setPlayerID(pID);
        addData(_playerDataKey, pID);
        sendEvent(TrackingEvent.START);
    }
}

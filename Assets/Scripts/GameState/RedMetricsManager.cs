using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedMetricsManager : MonoBehaviour
{    
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    /// 
    /// TODO make sure its game object is not destroyed during reload
    /// either add component RedMetricsManager to MemoryManager game object
    /// or create own RedMetricsManager game object that is not destroyed
    public static string gameObjectName = "RedMetricsManager";
    private static RedMetricsManager _instance;
    public static RedMetricsManager get() {
        if (_instance == null)
        {
            _instance = GameObject.Find(gameObjectName).GetComponent<RedMetricsManager>();
            if(null != _instance)
            {
                _instance.initializeIfNecessary();
            }
            else
            {
                Logger.Log("RedMetricsManager::get couldn't find game object", Logger.Level.ERROR);
            }
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("RedMetricsManager::Awake", Logger.Level.INFO);
        RedMetricsManager.get();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private void initializeIfNecessary() {}

    private string redMetricsURL = "https://api.redmetrics.io/v1/";
    private string redMetricsPlayer = "player";
    
    private string playerID;
    private string gameVersion;    
    private string createPlayerEventType = "newplayer";
    
    public void setPlayerID (string pID)
    {
        Logger.Log("setPlayerID("+pID+")", Logger.Level.INFO);
        playerID = pID;
    }
    
    public void setGameVersion (string gVersion)
    {
        Logger.Log("setGameVersion("+gVersion+")", Logger.Level.INFO);
        gameVersion = gVersion;
    }


    //////////////////////////////////////////////////
    /// 

    public static IEnumerator GET (string url, System.Action<WWW> callback)
    {
        Logger.Log("GET", Logger.Level.INFO);
        WWW www = new WWW (url);
        return waitForWWW (www, callback);
    }

    public static IEnumerator POST (string url, Dictionary<string,string> post, System.Action<WWW> callback)
    {
        Logger.Log("POST", Logger.Level.INFO);
        WWWForm form = new WWWForm ();
        foreach (KeyValuePair<string,string> post_arg in post) {
            form.AddField (post_arg.Key, post_arg.Value);
        }
            
        WWW www = new WWW (url, form);
        return waitForWWW (www, callback);
    }
        
    public static IEnumerator POST (string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
    {
        Logger.Log ("POST url:"+url, Logger.Level.INFO);
        WWW www = new WWW (url, post, headers);
        return waitForWWW (www, callback);
    }
        
    private static IEnumerator waitForWWW (WWW www, System.Action<WWW> callback)
    {
        Logger.Log ("waitForWWW", Logger.Level.INFO);
        float elapsedTime = 0.0f;
            
        if(null == www)
        {
            Logger.Log("waitForWWW: null www", Logger.Level.ERROR);
            yield return null;
        }

        while (!www.isDone) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 30.0f)
            {
                Logger.Log("waitForWWW: TimeOut!", Logger.Level.ERROR);
                break;
            }
            yield return null;
        }
            
        if (!www.isDone || !string.IsNullOrEmpty (www.error)) {
            string errmsg = string.IsNullOrEmpty (www.error) ? "timeout" : www.error;
            Logger.Log("RedMetricsManager::waitForWWW Error: Load Failed: " + errmsg, Logger.Level.ERROR);
            callback (null);    // Pass null result.
            yield break;
        }
            
        Logger.Log("waitForWWW: www good to ship!", Logger.Level.ERROR);
        callback (www); // Pass retrieved result.
    }

    ////////////////////////////////////////
    /// helpers
    /// 
    
    private void sendData(string urlSuffix, string pDataString, System.Action<WWW> callback)
    {
        string url = redMetricsURL + urlSuffix;
        Dictionary<string, string> headers = new Dictionary<string, string> ();
        headers.Add ("Content-Type", "application/json");
        byte[] pData = System.Text.Encoding.ASCII.GetBytes (pDataString.ToCharArray ());
        Logger.Log("RedMetricsManager::sendData StartCoroutine POST with data="+pDataString+" ...", Logger.Level.INFO);
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
            Logger.Log("RedMetricsManager::wwwLogger null == www from "+origin, Logger.Level.ERROR);
        } else {
            if (www.error == null) {
                Logger.Log("RedMetricsManager::wwwLogger Success: " + www.text + " from "+origin, Logger.Level.ERROR);
            } else {
                Logger.Log("RedMetricsManager::wwwLogger Error: " + www.error + " from "+origin, Logger.Level.ERROR);
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
        MemoryManager.get ().addData(MemoryManager.get ().playerDataKey, pID);
        sendEvent(TrackingEvent.START);
    }
    
    public void sendStartEvent(bool switched)
    {
        Logger.Log ("RedMetricsManager::sendStartEvent", Logger.Level.INFO);
        
        MemoryManager.get ();
        
        // management of game start for webplayer
        if(!switched) {
            //switch event is sent from somewhere else
            connect ();
            StartCoroutine(waitAndSendStart());
        }
        
        //TODO management of game start for standalone
        //WARNING: switch event is sent from somewhere else
        /*
        string pID = null;
        bool tryGetPID = tryGetData(playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Logger.Log ("RedMetricsManager::sendStartEvent player already identified - pID="+pID, Logger.Level.INFO);
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
    
    public void connect()
    {
        string json = "{\"gameVersionId\": "+gameVersion+"}";
        Logger.Log ("RedMetricsManager::connect will rmConnect json="+json, Logger.Level.ERROR);
        Application.ExternalCall("rmConnect", json);
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
            if(!string.IsNullOrEmpty(Hero.get ().getLastCheckpointName())) {
                sectionPart = ",\"section\":\""+Hero.get ().getLastCheckpointName()+"\"";
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
        bool tryGetPID = MemoryManager.get ().tryGetData(MemoryManager.get ().playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Logger.Log ("RedMetricsManager::sendEvent player already identified - pID="+pID, Logger.Level.INFO);            
        } else {
            Logger.Log ("RedMetricsManager::sendEvent no registered player!", Logger.Level.ERROR);
            pID = MemoryManager.get ().defaultPlayerID;
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
        Logger.Log ("RedMetricsManager::sendEvent will rmPostEvent json="+json, Logger.Level.INFO);
        Application.ExternalCall("rmPostEvent", json);
        
        /*
        //TODO management of game start for standalone
        string pID = null;
        bool tryGetPID = tryGetData(playerDataKey, out pID);
        if(tryGetPID && !string.IsNullOrEmpty(pID))
        {
            Logger.Log ("RedMetricsManager::sendEvent player already identified - pID="+pID, Logger.Level.INFO);
            string ourPostData = "{\"gameVersion\":" + gameVersion + "," +
                "\"player\":" + playerID + "," +
                    "\"type\":\""+eventCode+"\"}";
            sendData(redMetricsEvent, ourPostData, value => wwwLogger(value, "sendEvent("+eventCode+")"));
        } else {
            Logger.Log ("RedMetricsManager::sendEvent no registered player!", Logger.Level.ERROR);
        }
        */
    }

    public override string ToString ()
    {
        return string.Format ("[RedMetricsManager playerID:{0}, gameVersion:{1}, redMetricsURL:{2}]",
                              playerID, gameVersion, redMetricsURL);
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

// author 
//    Raphael Goujet
//    Center for Research and Interdisciplinarity
//    raphael.goujet@cri-paris.org
public class RedMetricsManager : MonoBehaviour
{    
  
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "RedMetricsManager";
    private static RedMetricsManager _instance;

    public static RedMetricsManager get ()
    {
        if (_instance == null) {
            _instance = GameObject.Find (gameObjectName).GetComponent<RedMetricsManager> ();
            if (null == _instance) {
                _instance = GameObject.FindObjectOfType<RedMetricsManager> ();
            }
            if (null != _instance) {
                //RedMetricsManager object is not destroyed when game restarts
                DontDestroyOnLoad (_instance.gameObject);
                _instance.initializeIfNecessary ();
            } else {
                logMessage (MessageLevel.ERROR, "get couldn't find game object");
            }
        }
        return _instance;
    }

    void Awake ()
    {
        antiDuplicateInitialization ();
    }
  
    void antiDuplicateInitialization ()
    {
        RedMetricsManager.get ();
        if (this != _instance) {
            Destroy (this.gameObject);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////
  
    //TODO interface to automatize data extraction for data gathering through sendEvent
    //eg: section, position

    private void initializeIfNecessary ()
    {
    }
  
    private string redMetricsURL = "https://api.redmetrics.io/v1/";
    private string redMetricsPlayer = "player";
    private string redMetricsEvent = "event";

    //Redmetrics-Unity's test game version
    private static string defaultGameVersion = "0bcc8bbb-b557-4b58-b133-761861df633b";
    private static System.Guid defaultGameVersionGuid = new System.Guid (defaultGameVersion);
    private System.Guid gameVersionGuid = new System.Guid (defaultGameVersionGuid.ToByteArray());

    private static string defaultGameSession = "b5ab445a-56c9-4c5b-a6d0-86e8a286cd81";
    private static System.Guid defaultGameSessionGUID = new System.Guid (defaultGameSession);
    private System.Guid gameSessionGUID = new System.Guid (defaultGameSessionGUID.ToByteArray());

    private string localPlayerGUID; //player guid stored on local computer, in PlayerPrefs 
    private string globalPlayerGUID; //TODO login system

    private bool isGameSessionGUIDCreated = false;
    private bool isStartEventSent = false;

    // list of events to be stacked while the player guid is not created yet, ie rmConnect's callback has not been called yet and isGameSessionGUIDCreated is still false
    private LinkedList<TrackingEventDataWithoutIDs> waitingList = new LinkedList<TrackingEventDataWithoutIDs>();
    
    public void setGameSessionGUID(string _gameSessionGUID) {
        gameSessionGUID = new System.Guid (_gameSessionGUID);
    }
    
    public void setLocalPlayerGUID(string _localPlayerGUID) {     
        localPlayerGUID = _localPlayerGUID;
    }
    
    public void setGlobalPlayerGUID(string _globalPlayerGUID) {
        globalPlayerGUID = _globalPlayerGUID;
    }
  
    public void setGameVersion (string gVersion)
    {
        gameVersionGuid = new System.Guid (gVersion);
    }

    private static void logMessage (MessageLevel level, string formattedMessage, params System.Object[] args) {
        logMessage(level, string.Format(formattedMessage, args));
    }

    private static void logMessage (MessageLevel level, string formattedMessage, IFormatProvider format, System.Object[] args) {
        logMessage(level, string.Format(format, formattedMessage, args));
    }
    
    private static void logMessage (string message) {
        logMessage(MessageLevel.DEFAULT, message);
    }

    private static void logMessage (MessageLevel level, string message)
    {
        //if the game is played using a web player
        if (Application.isWebPlayer) {
            Application.ExternalCall ("DebugFromWebPlayerToBrowser", message);

            //if the game is played inside the editor or as a standalone
        } else {
            switch (level) {
                case MessageLevel.DEFAULT:
                    //Debug.Log (message);
                    break;
                case MessageLevel.WARNING:
                    Debug.LogWarning (message);
                    break;
                case MessageLevel.ERROR:
                    Debug.LogError (message);
                    break;
                default:
                    Debug.Log (message);
                    break;
            }
        }
    }
  
    //////////////////////////////////////////////////
    /// standalone methods
  
    public static IEnumerator GET (string url, System.Action<WWW> callback)
    {
        //logMessage ("GET");
        WWW www = new WWW (url);
        return waitForWWW (www, callback);
    }
  
    public static IEnumerator POST (string url, Dictionary<string,string> post, System.Action<WWW> callback)
    {
        //logMessage ("POST");
        WWWForm form = new WWWForm ();
        foreach (KeyValuePair<string,string> post_arg in post) {
            form.AddField (post_arg.Key, post_arg.Value);
        }
    
        WWW www = new WWW (url, form);
        return waitForWWW (www, callback);
    }
  
    public static IEnumerator POST (string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
    {
        //logMessage (MessageLevel.DEFAULT, "POST url: {0}", url);
        WWW www = new WWW (url, post, headers);
        return waitForWWW (www, callback);
    }
  
    private static IEnumerator waitForWWW (WWW www, System.Action<WWW> callback)
    {
        //logMessage ("waitForWWW");
        float elapsedTime = 0.0f;
    
        if (null == www) {
            logMessage (MessageLevel.ERROR, "waitForWWW: null www");
            yield return null;
        }
    
        while (!www.isDone) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 30.0f) {
                logMessage (MessageLevel.ERROR, "waitForWWW: TimeOut!");
                break;
            }
            yield return null;
        }
    
        if (!www.isDone || !string.IsNullOrEmpty (www.error)) {
            string errmsg = string.IsNullOrEmpty (www.error) ? "timeout" : www.error;
            logMessage (MessageLevel.ERROR, "waitForWWW Error: Load Failed: {0}", errmsg);
            callback (null);    // Pass null result.
            yield break;
        }
    
        //logMessage (MessageLevel.DEFAULT, "waitForWWW: message successfully transmitted");
        callback (www); // Pass retrieved result.
    }
  
    ////////////////////////////////////////
    /// helpers for standalone
    /// 
  
    private void sendDataStandalone (string urlSuffix, string pDataString, System.Action<WWW> callback)
    {
        //logMessage("RedMetricsManager::sendDataStandalone");
        string url = redMetricsURL + urlSuffix;
        Dictionary<string, string> headers = new Dictionary<string, string> ();
        headers.Add ("Content-Type", "application/json");
        byte[] pData = System.Text.Encoding.ASCII.GetBytes (pDataString.ToCharArray ());
        //logMessage(MessageLevel.DEFAULT, "RedMetricsManager::sendDataStandalone StartCoroutine POST with data={0} ...", pDataString);
        StartCoroutine (RedMetricsManager.POST (url, pData, headers, callback));
    }
  
    private void createPlayer (System.Action<WWW> callback)
    {
        //logMessage("RedMetricsManager::createPlayer");
        CreatePlayerData data = new CreatePlayerData ();
        string json = getJsonString (data);
        sendDataStandalone (redMetricsPlayer, json, callback);
    }
  
    private void testGet (System.Action<WWW> callback)
    {
        //logMessage("RedMetricsManager::testGet");
        string url = redMetricsURL + redMetricsPlayer;
        StartCoroutine (RedMetricsManager.GET (url, callback));
    }
  
    private void wwwLogger (WWW www, string origin = "default")
    {
        if (null == www) {
            logMessage (MessageLevel.ERROR, "wwwLogger null == www from {0}", origin);
        } else {
            if (www.error == null) {
                logMessage (MessageLevel.DEFAULT, "RedMetricsManager::wwwLogger Success: {0} from {1}", www.text, origin);
            } else {
                logMessage (MessageLevel.ERROR, "wwwLogger Error: {0} from {1}", www.error, origin);
            } 
        }
    }
  
    private string extractPID (WWW www)
    {
        //logMessage("RedMetricsManager::extractPID");
        string result = null;
        wwwLogger (www, "extractPID");
        if(www != null && www.text != null)
        {
            string trimmed = www.text.Trim ();
            string[] split1 = trimmed.Split ('\n');
            foreach (string s1 in split1) {
                //logMessage(s1);
                if (s1.Length > 5) {
                    string[] split2 = s1.Trim ().Split (':');
                    foreach (string s2 in split2) {
                        if (!s2.Equals ("\"id\"") && !string.IsNullOrEmpty (s2)) {
                            string[] split3 = s2.Trim ().Split ('"');
                            foreach (string s3 in split3) {

                                if (!s3.Equals ("\"") && !string.IsNullOrEmpty (s3)) {
                                    result = s3;
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
  
    private void trackStart (WWW www) {
        //logMessage(MessageLevel.DEFAULT, "RedMetricsManager::trackStart: www =? null:{0}", (null == www));
        string pID = extractPID (www);
        if(null != pID)
        {
            setGameSessionGUID (pID);
            sendStartEventWithPlayerGUID();
        }
    }
    //////////////////////////////////////////////////
  
    private void sendStartEventWithPlayerGUID() {
        if(string.IsNullOrEmpty(localPlayerGUID)) {
            sendEvent (TrackingEvent.START);
        } else {
            CustomData guidCD = generateCustomDataForGuidInit();
            sendEvent (TrackingEvent.START, guidCD);
        }
    }

    public CustomData generateCustomDataForGuidInit() {
        //TODO manage GLOBALPLAYERGUID
        CustomData guidCD = new CustomData (CustomDataTag.LOCALPLAYERGUID, localPlayerGUID);
        guidCD.Add(CustomDataTag.PLATFORM, Application.platform.ToString().ToLowerInvariant());
        //logMessage(MessageLevel.DEFAULT, "generated guidCD={0}", guidCD);
        return guidCD;        
    } 

    //////////////////////////////////////////////////
    // filtering is done on Application.isWebPlayer
    // but could be done on Application.platform for better accuracy
    // Should be called only after localPlayerGUID is set
    public void sendStartEvent ()
    {
        //logMessage("RedMetricsManager::sendStartEvent");
        if (!isStartEventSent) {
            
            // all web players
            // management of game start for webplayer
            if (Application.isWebPlayer) {
                    connect ();
                    StartCoroutine (waitAndSendStart ());
            
            // other players + editor
            } else { 
                    //gameSessionGUID hasn't been initialized
                    //logMessage("RedMetricsManager::sendStartEvent other players/editor: createPlayer");
                    createPlayer (www => trackStart (www));
            }   
            isStartEventSent = true;
        }
    }

    //called by the bowser when connection is established
    public void ConfirmWebplayerConnection() {
        isGameSessionGUIDCreated = true;
        executeAndClearAllWaitingEvents();
    }

    private void addEventToSendLater(TrackingEventDataWithoutIDs data) {
        waitingList.AddLast(data);
    }

    private void executeAndClearAllWaitingEvents() {
        foreach(TrackingEventDataWithoutIDs data in waitingList) {
            sendEvent(data);
        }
        waitingList.Clear();
    }
  
    private IEnumerator waitAndSendStart ()
    {
        sendStartEventWithPlayerGUID();
        
        yield return new WaitForSeconds (5.0f);
        //all waiting events are flushed so that if the players disconnects, at least there's a trace that a player started the game
        executeAndClearAllWaitingEvents();

        //TODO: what if connection fails? Here all those events will be sent but the next, later ones won't
    }


    //webplayer
    public void connect ()
    {
        if (Application.isWebPlayer) {
            ConnectionData data = new ConnectionData (gameVersionGuid);
            string json = getJsonString (data);
            //logMessage(MessageLevel.DEFAULT, "RedMetricsManager::connect will rmConnect json={0}", json);
            Application.ExternalCall ("rmConnect", json);
        }
        //TODO treat answer by RedMetrics server by executeAndClearAllWaitingEvents so that all waiting events can be sent now that there's a user id
    }
    
    public void disconnect ()
    {
        if (Application.isWebPlayer) {
            Application.ExternalCall ("rmDisconnect");
            resetConnectionVariables();
        }
    }
    
    private void resetConnectionVariables() {
        isGameSessionGUIDCreated = false;
    } 
    
    
    public string getJsonString (object obj)
    {
        //serialization
        JsonWriter writer = new JsonWriter ();
        writer.PrettyPrint = true;    
        JsonMapper.ToJson (obj, writer);    
    
        string json = writer.ToString ();
        return json;
    }
  
    public void sendEvent (TrackingEvent trackingEvent, Vector2 coordinates, CustomData customData = null, string section = null)
    {
        int[] _coordinates = {
            Mathf.RoundToInt (coordinates.x),
            Mathf.RoundToInt (coordinates.y)
        };
        sendEvent (trackingEvent, customData, section, _coordinates);
    }
  
    public void sendEvent (TrackingEvent trackingEvent, Vector3 coordinates, CustomData customData = null, string section = null)
    {
        int[] _coordinates = {
            Mathf.RoundToInt (coordinates.x),
            Mathf.RoundToInt (coordinates.y),
            Mathf.RoundToInt (coordinates.z)
        };
        sendEvent (trackingEvent, customData, section, _coordinates);
    }

    public void sendEvent( TrackingEventDataWithoutIDs data) {
        sendEvent(data.getTrackingEvent(), data.customData, data.section, data.coordinates, data.userTime);
    }

    public void sendEvent (TrackingEvent trackingEvent, CustomData customData = null, string section = null, int[] coordinates = null, string userTime = null)
    {
        string checkedSection = section;

        //TODO remove dependency to Hero class
        if (string.IsNullOrEmpty (section) && (null != Hero.get ())) {
            checkedSection = Hero.get ().getLastCheckpointName ();
        }

        int[] checkedCoordinates = null;
        if (null != coordinates) {
            checkedCoordinates = new int[coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++) {
                checkedCoordinates [i] = coordinates [i];
            }
        } else {
            if (null != Hero.get ()) {
                Vector3 position = Hero.get ().gameObject.transform.position;
                checkedCoordinates = new int[2] {
                    Mathf.FloorToInt (position.x),
                    Mathf.FloorToInt (position.z)
                };
            }
        }
             
        //logMessage(MessageLevel.DEFAULT, "RedMetricsManager::sendEvent({0})", trackingEvent.ToString());
        if (Application.isWebPlayer) {
            TrackingEventDataWithoutIDs data = new TrackingEventDataWithoutIDs (trackingEvent, customData, checkedSection, checkedCoordinates, userTime);   
            if(isGameSessionGUIDCreated) {
                string json = getJsonString (data);
                Application.ExternalCall ("rmPostEvent", json);
            } else {
                addEventToSendLater(data);
                //TODO: what if connection fails, or even fails permanently? Should retry connection at different intervals
            }
        } else {
            //TODO wait on gameSessionGUID using an IEnumerator
            if (defaultGameSessionGUID != gameSessionGUID) {
            } else {
                logMessage (MessageLevel.ERROR, "sendEvent default player guid: no registered player!");
            }

            TrackingEventDataWithIDs data = new TrackingEventDataWithIDs (gameSessionGUID, gameVersionGuid, trackingEvent, customData, checkedSection, checkedCoordinates);
            string json = getJsonString (data);
            //logMessage (MessageLevel.DEFAULT, string.Format ("RedMetricsManager::sendEvent - gameSessionGUID={0}, gameVersionGuid={1}, json={2}", gameSessionGUID, gameVersionGuid, json));
            sendDataStandalone (redMetricsEvent, json, value => wwwLogger (value, "sendEvent(" + trackingEvent + ")"));
            //TODO pass data as parameter to sendDataStandalone so that it's serialized inside
        }
    }
  
    public override string ToString ()
    {
        return string.Format ("[RedMetricsManager gameSessionGUID:{0}, gameVersionGuid:{1}, redMetricsURL:{2}]",
                          gameSessionGUID, gameVersionGuid, redMetricsURL);
    }
  
}

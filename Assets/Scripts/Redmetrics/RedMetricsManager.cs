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
    private const string gameObjectName = "RedMetricsManager";
    private static RedMetricsManager _instance;

    public static RedMetricsManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("RedMetricsManager badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<RedMetricsManager>();
            if (null == _instance)
            {
                _instance = GameObject.FindObjectOfType<RedMetricsManager>();
            }
            if (null != _instance)
            {
                //RedMetricsManager object is not destroyed when game restarts
                // Debug.Log("RedMetricsManager get DontDestroyOnLoad");
                _instance.initializeIfNecessary();
            }
            else
            {
                Debug.LogError("RedMetricsManager get couldn't find game object");
            }
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake with gameVersionGuid=" + gameVersionGuid);
        if ((_instance != null) && (_instance != this))
        {
            // Debug.LogWarning(this.GetType() + " has two running instances: anti duplicate initialization");
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
        if (this == _instance)
        {
            _instance = null;
        }
    }

    // does not work on iOS, Windows Store Apps and Windows Phone 8.1
    // see details on https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html
    void OnApplicationQuit()
    {
        CustomData guidCD = generateCustomDataForGuidInit();
        sendEvent(TrackingEvent.END, guidCD);
    }

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        // Debug.Log(this.GetType() + " initializeIfNecessary _initialized=" + _initialized);
        if (!_initialized)
        {
            DontDestroyOnLoad(_instance.gameObject);
            _initialized = true;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start with gameVersionGuid=" + gameVersionGuid);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    //TODO interface to automatize data extraction for data gathering through sendEvent
    //eg: section, position

    private const string _redMetricsURL = "https://api.redmetrics.io/v1/";
    private const string _redMetricsPlayer = "player";
    private const string _redMetricsEvent = "event";

    //Redmetrics-Unity's test game version
    private const string _defaultGameVersion = "0bcc8bbb-b557-4b58-b133-761861df633b";
    private static System.Guid defaultGameVersionGuid = new System.Guid(_defaultGameVersion);
    private System.Guid gameVersionGuid = new System.Guid(defaultGameVersionGuid.ToByteArray());

    private const string _defaultGameSession = "b5ab445a-56c9-4c5b-a6d0-86e8a286cd81";
    private static System.Guid defaultGameSessionGUID = new System.Guid(_defaultGameSession);
    private System.Guid gameSessionGUID = new System.Guid(defaultGameSessionGUID.ToByteArray());

    private string localPlayerGUID; //player guid stored on local computer, in PlayerPrefs 
    private string globalPlayerGUID; //TODO login system

    private bool isGameSessionGUIDCreated = false;
    private bool _isStartEventSent = false;
    public bool isStartEventSent
    {
        get
        {
            return _isStartEventSent;
        }
    }

    // list of events to be stacked while the player guid is not created yet, ie rmConnect's callback has not been called yet and isGameSessionGUIDCreated is still false
    private LinkedList<TrackingEventDataWithoutIDs> waitingList = new LinkedList<TrackingEventDataWithoutIDs>();

    public void setGameSessionGUID(string _gameSessionGUID)
    {
        // Debug.Log(this.GetType() + " setGameSessionGUID " + _gameSessionGUID);
        gameSessionGUID = new System.Guid(_gameSessionGUID);
    }

    public void setLocalPlayerGUID(string _localPlayerGUID)
    {
        // Debug.Log(this.GetType() + " setLocalPlayerGUID " + _localPlayerGUID);
        localPlayerGUID = _localPlayerGUID;
    }

    public void setGlobalPlayerGUID(string _globalPlayerGUID)
    {
        // Debug.Log(this.GetType() + " setGlobalPlayerGUID " + _globalPlayerGUID);
        globalPlayerGUID = _globalPlayerGUID;
    }

    public void setGameVersion(string gVersion)
    {
        // Debug.Log(this.GetType() + " setGameVersion str " + gameVersionGuid + " to " + gVersion);
        gameVersionGuid = new System.Guid(gVersion);
    }

    public void setGameVersion(System.Guid gVersion)
    {
        // Debug.Log(this.GetType() + " setGameVersion Guid " + gameVersionGuid + " to " + gVersion);
        gameVersionGuid = gVersion;
    }

    public Guid getGameVersion()
    {
        return gameVersionGuid;
    }

    public bool isGameVersionInitialized()
    {
        // Debug.Log(this.GetType() + " isGameVersionInitialized");
        return defaultGameVersionGuid != gameVersionGuid;
    }

    //////////////////////////////////////////////////
    /// standalone methods

    public static IEnumerator GET(string url, System.Action<WWW> callback)
    {
        // Debug.Log("RedMetricsManager GET");
        WWW www = new WWW(url);
        return waitForWWW(www, callback);
    }

    public static IEnumerator POST(string url, Dictionary<string, string> post, System.Action<WWW> callback)
    {
        // Debug.Log("RedMetricsManager POST");
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }

        WWW www = new WWW(url, form);
        return waitForWWW(www, callback);
    }

    public static IEnumerator POST(string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
    {
        // Debug.Log("RedMetricsManager POST url: {0}", url);
        WWW www = new WWW(url, post, headers);
        return waitForWWW(www, callback);
    }

    private static IEnumerator waitForWWW(WWW www, System.Action<WWW> callback)
    {
        // Debug.Log("RedMetricsManager waitForWWW");
        float elapsedTime = 0.0f;

        if (null == www)
        {
            Debug.LogError("RedMetricsManager waitForWWW: null www");
            yield return null;
        }

        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 30.0f)
            {
                Debug.LogError("RedMetricsManager waitForWWW: TimeOut!");
                break;
            }
            yield return null;
        }

        if (!www.isDone || !string.IsNullOrEmpty(www.error))
        {
            string errmsg = string.IsNullOrEmpty(www.error) ? "timeout" : www.error;
            // Debug.LogError(string.Format("RedMetricsManager waitForWWW Error: Load Failed: {0}", errmsg));
            callback(null);    // Pass null result.
            yield break;
        }

        // Debug.Log("RedMetricsManager waitForWWW: message successfully transmitted");
        callback(www); // Pass retrieved result.
    }

    ////////////////////////////////////////
    /// helpers for standalone
    /// 

    private void sendDataStandalone(string urlSuffix, string pDataString, System.Action<WWW> callback)
    {
        // Debug.Log(this.GetType() + " sendDataStandalone");
        string url = _redMetricsURL + urlSuffix;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        byte[] pData = System.Text.Encoding.ASCII.GetBytes(pDataString.ToCharArray());
        // Debug.Log(this.GetType() + " sendDataStandalone StartCoroutine POST with data={0} ...", pDataString);
        StartCoroutine(RedMetricsManager.POST(url, pData, headers, callback));
    }

    private void createPlayer(System.Action<WWW> callback)
    {
        // Debug.Log(this.GetType() + " createPlayer");
        CreatePlayerData data = new CreatePlayerData();
        string json = getJsonString(data);
        sendDataStandalone(_redMetricsPlayer, json, callback);
    }

    private void testGet(System.Action<WWW> callback)
    {
        // Debug.Log(this.GetType() + " testGet");
        string url = _redMetricsURL + _redMetricsPlayer;
        StartCoroutine(RedMetricsManager.GET(url, callback));
    }

    private void wwwLogger(WWW www, string origin = "default")
    {
        if (null == www)
        {
            // Debug.LogError(this.GetType() + " wwwLogger null == www from " + origin);
        }
        else
        {
            if (www.error == null)
            {
                // Debug.Log(string.Format("{0} wwwLogger Success: {1} from {2}", this.GetType(), www.text, origin));
            }
            else
            {
                Debug.LogError(string.Format("{0} wwwLogger Error: {1} from {2}", this.GetType(), www.error, origin));
            }
        }
    }

    private string extractPID(WWW www)
    {
        // Debug.Log(this.GetType() + " extractPID");
        string result = null;
        wwwLogger(www, "extractPID");
        if (www != null && www.text != null)
        {
            string trimmed = www.text.Trim();
            string[] split1 = trimmed.Split('\n');
            foreach (string s1 in split1)
            {
                // Debug.Log(s1);
                if (s1.Length > 5)
                {
                    string[] split2 = s1.Trim().Split(':');
                    foreach (string s2 in split2)
                    {
                        if (!s2.Equals("\"id\"") && !string.IsNullOrEmpty(s2))
                        {
                            string[] split3 = s2.Trim().Split('"');
                            foreach (string s3 in split3)
                            {

                                if (!s3.Equals("\"") && !string.IsNullOrEmpty(s3))
                                {
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

    private void trackStart(WWW www)
    {
        // Debug.Log(this.GetType() + " trackStart: www =? null:" + (null == www));
        string pID = extractPID(www);
        if (null != pID)
        {
            setGameSessionGUID(pID);
            sendStartEventWithPlayerGUID();
        }
    }
    //////////////////////////////////////////////////

    private void sendStartEventWithPlayerGUID()
    {
        // Debug.Log(this.GetType() + " sendStartEventWithPlayerGUID");
        if (string.IsNullOrEmpty(localPlayerGUID))
        {
            sendEvent(TrackingEvent.START);
        }
        else
        {
            CustomData guidCD = generateCustomDataForGuidInit();
            sendEvent(TrackingEvent.START, guidCD);
        }
    }

    public CustomData generateCustomDataForGuidInit()
    {
        //TODO manage GLOBALPLAYERGUID
        CustomData guidCD = new CustomData(CustomDataTag.LOCALPLAYERGUID, localPlayerGUID);
        guidCD.Add(CustomDataTag.PLATFORM, Application.platform.ToString().ToLowerInvariant());
        // Debug.Log(this.GetType() + " generated guidCD={0}", guidCD);
        return guidCD;
    }

    //////////////////////////////////////////////////
    // Should be called only after localPlayerGUID is set
    public void sendStartEvent()
    {
        // Debug.Log(this.GetType() + " sendStartEvent");
        if (!_isStartEventSent)
        {
            // Debug.Log(this.GetType() + " sendStartEvent !isStartEventSent");

#if UNITY_WEBPLAYER
            // all web players
            // management of game start for webglplayer
            // Debug.Log(this.GetType() + " sendStartEvent calls connect");
            connect();
            sendStartEventWithPlayerGUID();

#else
            // other players + editor
            // Debug.Log(this.GetType() + " sendStartEvent isStartEventSent => createPlayer & trackStart");
            //gameSessionGUID hasn't been initialized
            // Debug.Log(this.GetType() + " sendStartEvent other players/editor: createPlayer");
            createPlayer(www => trackStart(www));
#endif
            _isStartEventSent = true;
        }
    }

    //called by the browser when connection is established
    public void ConfirmWebplayerConnection()
    {
        // Debug.Log(this.GetType() + " ConfirmWebplayerConnection");
        isGameSessionGUIDCreated = true;
        executeAndClearAllWaitingEvents();
    }

    private void addEventToSendLater(TrackingEventDataWithoutIDs data)
    {
        // Debug.Log(this.GetType() + " addEventToSendLater " + data);
        waitingList.AddLast(data);
    }

    private void executeAndClearAllWaitingEvents()
    {
        // Debug.Log(this.GetType() + " executeAndClearAllWaitingEvents");
        foreach (TrackingEventDataWithoutIDs data in waitingList)
        {
            sendEvent(data);
        }
        waitingList.Clear();
    }

    //webplayer
    public void connect()
    {
        // Debug.Log(this.GetType() + " connect");
#if UNITY_WEBPLAYER
            // Debug.Log(this.GetType() + " Unity connect");
            // force to wait for MemoryManager
            ConnectionData data = new ConnectionData(gameVersionGuid);
            string json = getJsonString(data);
            // Debug.Log(this.GetType() + " connect will rmConnect json={0}", json);
            Application.ExternalCall("rmConnect", json);
#else
            Debug.LogWarning(this.GetType() + " called connect, but not from WebGLPlayer");
#endif
    }

    public void disconnect()
    {
#if UNITY_WEBPLAYER
            // Debug.Log(this.GetType() + " Unity disconnect");
            Application.ExternalCall("rmDisconnect");
            resetConnectionVariables();
#endif
    }

    private void resetConnectionVariables()
    {
        isGameSessionGUIDCreated = false;
    }


    public string getJsonString(object obj)
    {
        //serialization
        JsonWriter writer = new JsonWriter();
        writer.PrettyPrint = true;
        JsonMapper.ToJson(obj, writer);

        string json = writer.ToString();
        return json;
    }

    public void sendEvent(TrackingEvent trackingEvent, Vector2 coordinates, CustomData customData = null, string section = null)
    {
        int[] _coordinates = {
            Mathf.RoundToInt (coordinates.x),
            Mathf.RoundToInt (coordinates.y)
        };
        sendEvent(trackingEvent, customData, section, _coordinates);
    }

    public void sendEvent(TrackingEvent trackingEvent, Vector3 coordinates, CustomData customData = null, string section = null)
    {
        int[] _coordinates = {
            Mathf.RoundToInt (coordinates.x),
            Mathf.RoundToInt (coordinates.y),
            Mathf.RoundToInt (coordinates.z)
        };
        sendEvent(trackingEvent, customData, section, _coordinates);
    }

    public void sendEvent(TrackingEventDataWithoutIDs data)
    {
        sendEvent(data.getTrackingEvent(), data.customData, data.section, data.coordinates, data.userTime);
    }

    public void sendRichEvent(TrackingEvent trackingEvent, CustomData customData = null, string section = null, int[] coordinates = null, string userTime = null)
    {
        // string customDataString = null == customData ? "" : ", " + customData;
        // Debug.Log(this.GetType() + " sendRichEvent(" + trackingEvent + customDataString);

        CustomData context = Character.get().getEventContext();
        if (customData != null)
        {
            // Debug.Log(this.GetType() + " merging from trackingEvent " + trackingEvent);
            context.merge(customData);
        }
        sendEvent(trackingEvent, context, section, coordinates, userTime);
    }

    public void sendEvent(TrackingEvent trackingEvent, CustomData customData = null, string section = null, int[] coordinates = null, string userTime = null)
    {
        // test Application.internetReachability
        // Debug.Log(this.GetType() + " sendEvent " + trackingEvent + " " + customData);
        string checkedSection = section;

        //TODO remove dependency to Character class
        if (string.IsNullOrEmpty(section) && (null != Character.get()))
        {
            checkedSection = Character.get().getLastCheckpointName();
        }

        int[] checkedCoordinates = null;
        if (null != coordinates)
        {
            checkedCoordinates = new int[coordinates.Length];
            for (int i = 0; i < coordinates.Length; i++)
            {
                checkedCoordinates[i] = coordinates[i];
            }
        }
        else
        {
            if (null != Character.get())
            {
                Vector3 position = Character.get().gameObject.transform.position;
                checkedCoordinates = new int[2] {
                    Mathf.FloorToInt (position.x),
                    Mathf.FloorToInt (position.z)
                };
            }
        }

        // Debug.Log(this.GetType() + " sendEvent({0})", trackingEvent.ToString());
#if UNITY_WEBPLAYER
            TrackingEventDataWithoutIDs data = new TrackingEventDataWithoutIDs(trackingEvent, customData, checkedSection, checkedCoordinates, userTime);
            if (isGameSessionGUIDCreated)
            {
                string json = getJsonString(data);
                // Debug.Log(this.GetType() + " sendEvent("+json+")");
                Application.ExternalCall("rmPostEvent", json);
            }
            else
            {
                addEventToSendLater(data);
                //TODO: what if connection fails, or even fails permanently? Should retry connection at different intervals
            }
#else
            // TODO wait on gameSessionGUID using an IEnumerator
            // if (defaultGameSessionGUID != gameSessionGUID)
            // {
            // }
            // else
            // {
            //     Debug.LogError(this.GetType() + " sendEvent default player guid: no registered player!");
            // }

            TrackingEventDataWithIDs data = new TrackingEventDataWithIDs(gameSessionGUID, gameVersionGuid, trackingEvent, customData, checkedSection, checkedCoordinates);
            string json = getJsonString(data);
            // Debug.Log(string.Format (this.GetType() + " sendEvent - gameSessionGUID={0}, gameVersionGuid={1}, json={2}", gameSessionGUID, gameVersionGuid, json));
            sendDataStandalone(_redMetricsEvent, json, value => wwwLogger(value, "sendEvent(" + trackingEvent + ")"));
            //TODO pass data as parameter to sendDataStandalone so that it's serialized inside
#endif
    }

    public override string ToString()
    {
        return string.Format("[RedMetricsManager gameSessionGUID:{0}, gameVersionGuid:{1}, redMetricsURL:{2}]",
                          gameSessionGUID, gameVersionGuid, _redMetricsURL);
    }

}

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
            Logger.Log("MemoryManager::get was badly initialized", Logger.Level.ERROR);
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
    ////////////////////////////////////////////////////////////////////////////////////////////
    
    public string[] inputFiles;
    private Dictionary<string, string> _savedData = new Dictionary<string, string>();
    private Dictionary<string, LevelInfo> _loadedLevelInfo = new Dictionary<string, LevelInfo>();

    private void initializeIfNecessary(bool onlyIfEmpty = true)
    {
        Debug.LogError("MemoryManager::Awake loadLevelData before with onlyIfEmpty="+onlyIfEmpty+" & _loadedLevelInfo.Count="+_loadedLevelInfo.Count); //_loadedLevelInfo.Count==0
        if(!onlyIfEmpty || 0 == _loadedLevelInfo.Count)
        {
            Debug.LogError("MemoryManager::Awake loadLevelData BEFORE _loadedLevelInfo.Count="+_loadedLevelInfo.Count);
            loadLevelData(inputFiles, _loadedLevelInfo);
            Debug.LogError("MemoryManager::Awake loadLevelData AFTER _loadedLevelInfo.Count="+_loadedLevelInfo.Count);
            //TODO manage everything here
            GameStateController.get ().setAndSaveLevelName(GameStateController._adventureLevel1);
        }
        Debug.LogError("MemoryManager::Awake loadLevelData after");
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
        Debug.LogError("MemoryManager::addOrUpdateData("+key+", "+value+")");
        if(_savedData.ContainsKey(key))
        {
            _savedData.Remove(key);
        }
        return addData(key, value);
    }

    public bool tryGetData(string key, out string value)
    {
        Debug.LogError("MemoryManager::tryGetData("+key+", out value)");
        return _savedData.TryGetValue(key, out value);
    }

    private void loadLevelData(string[] inputFiles, Dictionary<string, LevelInfo> dico)
    {      
        Debug.LogError("MemoryManager::loadLevelData starts with dico.Count="+dico.Count);
        FileLoader loader = new FileLoader();
    
        foreach (string file in inputFiles)
        {
            Debug.LogError("MemoryManager::loadLevelData processing file="+file);
            LinkedList<LevelInfo> lis = loader.loadObjectsFromFile<LevelInfo>(file,LevelInfoXMLTags.INFO);
            if(null != lis)
            {
                Debug.LogError("MemoryManager::loadLevelData null != lis");
                foreach( LevelInfo li in lis)
                {
                    Debug.LogError("MemoryManager::loadLevelData adding li="+li);
                    dico.Add(li.code, li);
                }
            }

        }
        
        Logger.Log("ModalManager::loadLevelData loaded ", Logger.Level.DEBUG);
        Debug.LogError("MemoryManager::loadLevelData ends with dico.Count="+dico.Count);
    }
    
    private static LevelInfo retrieveFromDico(string code)
    {
        LevelInfo info;
        //TODO set case-insensitive
        Debug.LogError("MemoryManager::retrieveFromDico("+code+") with _instance._loadedLevelInfo.Count="+_instance._loadedLevelInfo.Count);
        if(!_instance._loadedLevelInfo.TryGetValue(code, out info))
        {
            Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
            info = null;
        }
        return info;
    }

    public bool tryGetCurrentLevelInfo(out LevelInfo levelInfo)
    {
        levelInfo = null;
        string currentLevelCode;
        if(tryGetData(GameStateController._currentLevelKey, out currentLevelCode))
        {
            Debug.LogError("MemoryManager::tryGetCurrentLevelInfo currentLevelCode="+currentLevelCode);
            return _loadedLevelInfo.TryGetValue(currentLevelCode, out levelInfo);
        }
        else
        {
            Logger.Log("MemoryManager::tryGetCurrentLevelInfo failed to provide data; GameStateController._currentLevelKey="+GameStateController._currentLevelKey, Logger.Level.WARN);

            //defensive code
            Debug.LogError("NO CURRENT LEVEL INFO; WILL INSERT DEFAULT");
            GameStateController.get ().setAndSaveLevelName(GameStateController._adventureLevel1);

            return false;
        }
    }



    void Start()
    {
        Debug.LogError("MemoryManager::Start");
    }

    void OnDestroy()
    {
        Debug.LogError("MemoryManager::ONDESTROY");
    }
}

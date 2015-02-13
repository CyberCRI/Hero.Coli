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
}

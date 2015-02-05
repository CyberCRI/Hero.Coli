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
        }
        
        return _instance;
    }
    void Awake()
    {
        Logger.Log("MemoryManager::Awake", Logger.Level.DEBUG);
        _instance = this;
        loadLevelData(inputFiles, _loadedLevelInfo);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////
    
    public string[] inputFiles;
    private Dictionary<string, string> _savedData = new Dictionary<string, string>();
    private Dictionary<string, LevelInfo> _loadedLevelInfo = new Dictionary<string, LevelInfo>();

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

    public bool tryGetData(string key, out string value)
    {
        return _savedData.TryGetValue(key, out value);
    }

    private void loadLevelData(string[] inputFiles, Dictionary<string, LevelInfo> dico)
    {      
        FileLoader loader = new FileLoader();
    
        foreach (string file in inputFiles)
        {
            LinkedList<LevelInfo> lis = loader.loadObjectsFromFile<LevelInfo>(file,LevelInfoXMLTags.INFOLIST);
            if(null != lis)
            {
                foreach( LevelInfo li in lis)
                {
                    dico.Add(li.code, li);
                }
            }

        }
        
        Logger.Log("ModalManager::loadDataIntoDico loaded ", Logger.Level.DEBUG);
    }
    
    private static LevelInfo retrieveFromDico(string code)
    {
        LevelInfo info;
        //TODO set case-insensitive
        if(!_instance._loadedLevelInfo.TryGetValue(code, out info))
        {
            Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
            info = null;
        }
        return info;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

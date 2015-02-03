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
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private Dictionary<string, string> _savedData = new Dictionary<string, string>();

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

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

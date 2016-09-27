using UnityEngine;

public abstract class Singleton<T>
{
    protected static string _gameObjectName;
    public static string gameObjectName
    {
        get
        {
            return _gameObjectName;
        }
    }

    protected static T _instance;
    public static T instance
    {
        get
        {
            initializeInstanceIfNecessary();
            return _instance;
        }
    }

    protected static void initializeInstanceIfNecessary()
    {
        if (null == _instance)
        {
            GameObject go = GameObject.Find(gameObjectName);
            if (go)
            {
                _instance = go.GetComponent<T>();
            }
        }
    }

    void Awake()
    {
        initializeInstanceIfNecessary();
    }
}
using UnityEngine;
using System;

public abstract class ConfigurationParameter<K>
{
    public ConfigurationParameter(K startValue, K defaultValue, string key)
    {
        _value = startValue;
        _defaultValue = defaultValue;
        _key = key;
    }

    public ConfigurationParameter(K startValue, K defaultValue, string key, Callback onValueChanged)
    : this(startValue, defaultValue, key)
    {
        _onValueChanged = onValueChanged;
    }

    protected K _defaultValue;
    protected string _key;

    public abstract string kToString(K k);
    public abstract K stringToK(string s);

    public abstract bool isInitializationNeeded();

    public delegate void Callback(K k);
    protected Callback _onValueChanged = voidMethod;
    protected static void voidMethod(K k) { }

    protected K _value;
    public K val
    {
        get
        {
            initialize();
            return _value;
        }

        set
        {
            PlayerPrefs.SetString(_key, kToString(value));
            _value = value;
            // Debug.Log(this.GetType() + " _onValueChanged: set " + _key + " to " + kToString(value));
            _onValueChanged(value);
        }
    }

    public virtual bool initialize()
    {
        if (isInitializationNeeded())
        {
            Debug.Log(this.GetType() + " initialize isInitializationNeeded for key = " + _key);
            string storedValue = PlayerPrefs.GetString(_key);
            if (!string.IsNullOrEmpty(storedValue))
            {
                try
                {
                    K _storedValue = stringToK(storedValue);
                    val = _storedValue;
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError(this.GetType() + " initialization failed: " + e);
                    return false;
                }
            }
            else
            {
                // Debug.Log(this.GetType() + " initialize: couldn't read value, found _key as " + storedValue);
                val = _defaultValue;
                return true;
            }
        }
        return false;
    }
}
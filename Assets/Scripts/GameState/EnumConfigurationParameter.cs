using UnityEngine;
using System;

public class EnumConfigurationParameter<K> : ConfigurationParameter<K>
where K : IConvertible
{
    public EnumConfigurationParameter(K startValue, K defaultValue, K uninitializedValue, string key)
    : base(startValue, defaultValue, key)
    {
        _uninitializedValue = uninitializedValue;
    }

    public EnumConfigurationParameter(K startValue, K defaultValue, K uninitializedValue, string key, Callback onValueChanged)
    : base(startValue, defaultValue, key, onValueChanged)
    {
        _uninitializedValue = uninitializedValue;
    }

    private K _uninitializedValue;

    public override string kToString(K k)
    {
        return (Convert.ToInt32(k)).ToString();
    }
    public override K stringToK(string s)
    {
        int i = int.Parse(s);
        // K k = (K) Enum.Parse(typeof(K), s);
        K k = (K)Enum.ToObject(typeof(K), i);
        if (Enum.IsDefined(typeof(K), k))
        {
            return k;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " failed to convert");
            return _defaultValue;
        }
    }

    public override bool isInitializationNeeded()
    {
        return _uninitializedValue.Equals(_value);
    }
}
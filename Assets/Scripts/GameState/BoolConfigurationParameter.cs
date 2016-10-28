using UnityEngine;

public class BoolConfigurationParameter : ConfigurationParameter<bool>
{
    public BoolConfigurationParameter(bool startValue, bool defaultValue, string key)
    : base(startValue, defaultValue, key)
    {
    }

    public BoolConfigurationParameter(bool startValue, bool defaultValue, string key, Callback onValueChanged)
    : base(startValue, defaultValue, key, onValueChanged)
    {
    }

    public override string kToString(bool b)
    {
        return b.ToString();
    }
    public override bool stringToK(string s)
    {
        return bool.Parse(s);
    }

    private bool _isInitialized = false;
    public override bool isInitializationNeeded()
    {
        return !_isInitialized;
    }
    public override bool initialize()
    {
        if (base.initialize())
        {
            // Debug.Log(this.GetType() + "  initialize to _value = " + _value);
            _isInitialized = true;
            return true;
        }
        return false;
    }
}
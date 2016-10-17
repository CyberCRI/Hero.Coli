public class FloatConfigurationParameter : ConfigurationParameter<float>
{
    public FloatConfigurationParameter(float startValue, float defaultValue, string key)
    : base(startValue, defaultValue, key)
    {
    }

    public override string kToString(float b)
    {
        return b.ToString();
    }
    public override float stringToK(string s)
    {
        return float.Parse(s);
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
            _isInitialized = true;
            return true;
        }
        return false;
    }
}
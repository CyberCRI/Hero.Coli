public class IntConfigurationParameter : ConfigurationParameter<int>
{
    public IntConfigurationParameter(int startValue, int defaultValue, string key)
    : base(startValue, defaultValue, key)
    {
    }

    public override string kToString(int b)
    {
        return b.ToString();
    }
    public override int stringToK(string s)
    {
        return int.Parse(s);
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
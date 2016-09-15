public class FakeStepByStepTutorial : StepByStepTutorial
{
    private const string _textKeyPrefix = _genericTextKeyPrefix;
    protected override string textKeyPrefix
    {
        get
        {
            return _textKeyPrefix;
        }
    }
    private const int _stepCount = 0;
    protected override int stepCount {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] {};
    protected override string[] focusObjects 
    {
        get
        {
            return _focusObjects;
        }
    }
}

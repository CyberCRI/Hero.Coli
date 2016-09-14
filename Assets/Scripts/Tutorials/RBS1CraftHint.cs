public class RBS1CraftHint : StepByStepTutorial
{
    private bool prepared = false;

    private const string _textKeyPrefix = _genericTextKeyPrefix + "RBS1CRAFT.";
    protected override string textKeyPrefix
    {
        get
        {
            return _textKeyPrefix;
        }
    }
    private const int _stepCount = 1;
    protected override int stepCount
    {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] { 
        _craftButton
    };
    protected override string[] focusObjects
    {
        get
        {
            return _focusObjects;
        }
    }
}

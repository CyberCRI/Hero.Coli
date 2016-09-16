// #define QUICKTEST

#if QUICKTEST
public class RBS2CraftHint : FakeStepByStepTutorial { }
#else
public class RBS2CraftHint : StepByStepTutorial
{
    private const string _textKeyPrefix = _genericTextKeyPrefix + "RBS2CRAFT.";
    protected override string textKeyPrefix
    {
        get
        {
            return _textKeyPrefix;
        }
    }
    private const int _stepCount = 7;
    protected override int stepCount
    {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] {
        _craftButton,
        _craftResultPrefix + _moveDevice1 + _backgroundSuffix,
        _craftSlot1,
        _RBS2brick,
        _craftSlot1,
        _craftResultPrefix + _moveDevice2 + _backgroundSuffix,
        _exitCross
        };
    protected override string[] focusObjects
    {
        get
        {
            return _focusObjects;
        }
    }
}
#endif

// #define QUICKTEST

#if QUICKTEST

public class InducedPromoterHint : FakeStepByStepTutorial { }

#else

public class InducedPromoterHint : StepByStepTutorial
{
    private const string _textKeyPrefix = _genericTextKeyPrefix + "INDUCEDPROMOTER.";
    protected override string textKeyPrefix
    {
        get
        {
            return _textKeyPrefix;
        }
    }
    private const int _stepCount = 5;
    protected override int stepCount
    {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] {
        _craftButton, 
        _PCONSBrickBackground,
        _PBAD3BrickBackground,
        _PBAD3BrickBackground,
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
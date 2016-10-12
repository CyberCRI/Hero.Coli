// #define QUICKTEST

#if QUICKTEST
public class PlasmidHint : FakeStepByStepTutorial { }
#else
public class PlasmidHint : StepByStepTutorial
{
    private const string _textKeyPrefix = _genericTextKeyPrefix + "PLASMID.";
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
        _craftSlot2,
        _RBS2brick,  // rbs
        _GFPbrick,  // gfp
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
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
    private const int _stepCount = 9;
    protected override int stepCount
    {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] {
        _craftButton,
        _craftSlot1,
        _craftSlot2,
        _PBAD3Brick,

        // the player can't use both in their movement device
        _RBS2brick,  // rbs2
        _RBS1brick,  // rbs1

        _GFPbrick,  // gfp
        _terminatorBrick,
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
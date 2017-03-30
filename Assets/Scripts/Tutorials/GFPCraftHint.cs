// #define QUICKTEST

#if QUICKTEST

public class GFPCraftHint : FakeStepByStepTutorial { }

#else

public class GFPCraftHint : StepByStepTutorial
{
    private const string _textKeyPrefix = _genericTextKeyPrefix + "GFPCRAFT.";
    protected override string textKeyPrefix
    {
        get
        {
            return _textKeyPrefix;
        }
    }
    private const int _stepCount = 6;
    protected override int stepCount
    {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] {
        _craftButton,
        _listedPrefix + moveDevice1,
        _MOVbrick,
        _GFPbrick,
        _craftResultPrefix + _GFPdevice1 + _backgroundSuffix,
        _exitCross
        };
    protected override string[] focusObjects
    {
        get
        {
            return _focusObjects;
        }
    }


    private bool _isMoveDevice1, _isMoveDevice2;
    protected override bool skipStep(int step)
    {
        return (1 == step) && (_isMoveDevice1 || _isMoveDevice2);
    }
    protected override void prepareStep(int step)
    {
        if (0 == step)
        {
            _isMoveDevice1 = CraftFinalizer.get().isEquipped(moveDevice1);
            _isMoveDevice2 = CraftFinalizer.get().isEquipped(moveDevice2);
        }
        else if (2 == step && _isMoveDevice2)
        {
            _focusObjects[4] = _craftResultPrefix + _GFPdevice2 + _backgroundSuffix;
        }
    }
}
#endif
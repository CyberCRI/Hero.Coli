public class RBS2CraftHint : StepByStepTutorial
{
    
    private const string _device = "PRCONS:RBS3:MOV:DTER";
    private const string _brick = _availableDisplayedPrefix + "RBS2";

    private const string _textKeyPrefix = _genericTextKeyPrefix + "GFPCRAFT.";
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
        _listedPrefix + _device,
        _brick,
        _craftWindow,
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

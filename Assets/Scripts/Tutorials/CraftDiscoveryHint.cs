// #define QUICKTEST

#if QUICKTEST
public class CraftDiscoveryHint : FakeStepByStepTutorial { }
#else
using UnityEngine;
public class CraftDiscoveryHint : StepByStepTutorial
{
    private const string _cellPanelEquippedDeviceBackground = _equippedPrefix + _moveDevice1 + _backgroundSuffix;
    private const string _craftResultDevice = _craftResultPrefix + _moveDevice1;
    private const string _craftResultDeviceBackground = _craftResultDevice + _backgroundSuffix;

    private const string _textKeyPrefix = _genericTextKeyPrefix + "CRAFTDISCOVERY.";
    protected override string textKeyPrefix
    {
        get
        {
            return _textKeyPrefix;
        }
    }
    private const int _stepCount = 12;
    protected override int stepCount {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] { 
        _bacterium,
        _cellPanelEquippedDeviceBackground,
        _craftButton,
        _craftWindow,
        _craftResultDeviceBackground,
        _craftSlot1,
        _craftResultDevice,
        _exitCross,
        _bacterium,
        _craftButton,
        _listedPrefix + _moveDevice1,
        _exitCross
        };
    protected override string[] focusObjects 
    {
        get
        {
            return _focusObjects;
        }
    }

    protected override void end()
    {
        Hero.get().gameObject.AddComponent<MovementHint>();
        base.end();
    }
}
#endif

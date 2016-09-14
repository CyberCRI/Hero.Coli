using UnityEngine;

public class CraftDiscoveryHint : StepByStepTutorial
{

    private const string _slowMoveDevice = "PRCONS:RBS3:MOV:DTER";
    private const string _cellPanelEquippedDevice = _equippedPrefix + _slowMoveDevice;
    private const string _craftResultDevice = _craftResultPrefix + _slowMoveDevice;
    private const string _listedDevice = _listedPrefix + _slowMoveDevice;
    private const string _craftResultDeviceBackground = _craftResultDevice + _backgroundSuffix;    
    private const string _craftSlot = "slot0SelectionSprite";

    private Vector3 manualScale = new Vector3(440, 77, 1);


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
        _cellPanelEquippedDevice,
        _craftButton,
        _craftWindow,
        _craftResultDeviceBackground,
        _craftSlot,
        _craftResultDevice,
        _exitCross,
        _bacterium,
        _craftButton,
        _listedDevice,
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

// #define QUICKTEST

#if QUICKTEST

public class GFPCraftHint : FakeStepByStepTutorial { }

#else

using UnityEngine;

// TODO inherit StepByStepTutorial
public class GFPCraftHint : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;

    private const string _craftButton = "CraftButton";
    private const string _listedPrefix = "Listed";
    private const string _device1 = "PRCONS:RBS3:MOV:DTER";
    private const string _device2 = "PRCONS:RBS2:MOV:DTER";
    private const string _GFPdevice = "PRCONS:RBS2:FLUO1:DTER";
    private const string _MOVbrick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "MOV";
    private const string _GFPbrick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "FLUO1";
    private const string _craftWindow = "CraftPanelSprite";
    private const string _exitCross = "CraftCloseButton";
    private const string _textKeyPrefix = "HINT.GFPCRAFT.";
    private const string _craftResultPrefix = "c_";
    private const string _backgroundSuffix = "Background";

    private const int _stepCount = 6;
    private string[] focusObjects = new string[_stepCount] { _craftButton, _listedPrefix + _device1, _MOVbrick, _GFPbrick, _craftResultPrefix + _GFPdevice + _backgroundSuffix, _exitCross };

    private string[] textHints = new string[_stepCount];

    public void next()
    {
        prepared = false;
        step++;
    }
    
    void Awake()
    {
        for(int index = 0; index < textHints.Length; index++)
        {
            textHints[index] = _textKeyPrefix+index;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (step < focusObjects.Length)
        {
            if (!prepared)
            {
                if ((1 == step) && (CraftFinalizer.get().isEquiped(_device1) || CraftFinalizer.get().isEquiped(_device2)))
                {
                    next();
                }
                else
                {
                    GameObject go = GameObject.Find(focusObjects[step]);
                    if (null == go)
                    {
                        Debug.LogError("GFPCraftHint: GameObject not found: "+focusObjects[step]);
                        next();
                    }
                    else
                    {
                        ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                        if(null != target)
                        {
                            FocusMaskManager.get().focusOn(target, next, textHints[step]);
                        }
                        else
                        {
                            FocusMaskManager.get().focusOn(go, next, textHints[step], true);
                        }
                        prepared = true;
                        }
                }
            }
        }
        else
        {
            FocusMaskManager.get().stopFocusOn();
            Destroy(this);
        }
    }
}
#endif

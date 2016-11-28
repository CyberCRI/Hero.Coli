// #define QUICKTEST

#if QUICKTEST

public class CraftHint : FakeStepByStepTutorial
{
    public int bricks = 0;
}

#else

// TODO inherit StepByStepTutorial
using UnityEngine;

public class CraftHint : MonoBehaviour
{

    private int step = 0;
    public int bricks = 0;
    private bool prepared = false;

    private const string _craftButton = "CraftButton";
    private const string _craftWindow = "CraftPanelSprite";
    private const string _brick1 = AvailableDisplayedBioBrick._availableDisplayedPrefix + "PRCONS",
    _brick2 = AvailableDisplayedBioBrick._availableDisplayedPrefix + "RBS2",
    _brick3 = AvailableDisplayedBioBrick._availableDisplayedPrefix + "MOV",
    _brick4 = AvailableDisplayedBioBrick._availableDisplayedPrefix + "DTER";
    private const string _exitCross = "CraftCloseButton";
    private const string _textKeyPrefix = "HINT.CRAFT.";

    private const int _stepCount = 8;
    private string[] focusObjects = new string[_stepCount] { _craftButton, _craftWindow, _brick1, _brick2, _brick3, _brick4, _craftWindow, _exitCross };
    private string[] textHints = new string[_stepCount];

    public void next()
    {
        prepared = false;
        step++;
    }

    void Awake()
    {
        for (int index = 0; index < textHints.Length; index++)
        {
            textHints[index] = _textKeyPrefix + index;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (4 == bricks)
        {
            if (step < focusObjects.Length)
            {
                if (!prepared)
                {
                    GameObject go = GameObject.Find(focusObjects[step]);
                    if (go == null)
                    {
                        Debug.LogError("couldn't find " + focusObjects[step]);
                    }
                    else
                    {
                        ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                        if (null != target)
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
            else
            {
                FocusMaskManager.get().stopFocusOn();
                Destroy(this);
            }
        }
    }
}
#endif

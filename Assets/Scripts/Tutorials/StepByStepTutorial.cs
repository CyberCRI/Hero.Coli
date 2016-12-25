// #define DEV
using UnityEngine;

public abstract class StepByStepTutorial : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;
    private float waited = 0;
    private const float waitedThreshold = 0f;

    protected const string _bacterium = Hero.gameObjectName;
    protected const string _craftWindow = "CraftPanelSprite";
    protected const string _craftButton = "CraftButton";
    protected const string _backgroundSuffix = "Background";
    protected const string _bioBrickIconBackgroundSuffix = "BioBrickIconBackground";
    protected const string _exitCross = "CraftCloseButton";

    protected const string _availableDisplayedPrefix = AvailableDisplayedBioBrick._availableDisplayedPrefix;
    protected const string _listedPrefix = "l_";
    protected const string _equippedPrefix = "e_";
    protected const string _craftResultPrefix = "c_";
    protected const string _moveDevice1 = "PRCONS:RBS3:MOV:DTER";
    protected const string _moveDevice2 = "PRCONS:RBS2:MOV:DTER";
    protected const string _moveDevice3 = "PRCONS:RBS1:MOV:DTER";
    protected const string _slotBaseName = "slot";
    protected const string _slotSelectionSpriteSuffix = "SelectionSprite";
    protected const string _craftSlot1 = _slotBaseName + "0" + _slotSelectionSpriteSuffix;
    protected const string _craftSlot2 = _slotBaseName + "1" + _slotSelectionSpriteSuffix;
    protected const string _PBAD3Brick = _availableDisplayedPrefix + "PRBAD3";
    protected const string _RBS2brick = _availableDisplayedPrefix + "RBS2";
    protected const string _RBS1brick = _availableDisplayedPrefix + "RBS1";
    protected const string _GFPbrick = _availableDisplayedPrefix + "FLUO1";
    protected const string _terminatorBrick = _availableDisplayedPrefix + "DTER";
    protected const string _PCONSBrickBackground = _availableDisplayedPrefix + "PRCONS" + _bioBrickIconBackgroundSuffix;
    protected const string _PBAD3BrickBackground = _availableDisplayedPrefix + "PRBAD3" + _bioBrickIconBackgroundSuffix;

    protected const string _genericTextKeyPrefix = "HINT.";
    private string[] textHints;

    protected abstract string textKeyPrefix { get; }
    protected abstract int stepCount { get; }
    protected abstract string[] focusObjects { get; }

    private Vector3 manualScale = new Vector3(440, 77, 1);
    private static FocusMaskManager focusMaskManager;

    private static bool _isPlaying = false;
    public static bool isPlaying()
    {
        return _isPlaying;
    }
    public static void reset()
    {
        _isPlaying = false;
    }

    public void next()
    {
        // Debug.Log(this.GetType() + " next");
        prepared = false;
        waited = 0f;
        step++;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake " + this.GetType()
        // + " step=" + stepCount
        // + " prepared=" + prepared
        // + " waited=" + waited
        // + " textHints=" + textHints
        // + " textKeyPrefix=" + textKeyPrefix
        // + " stepCount=" + stepCount
        // + " focusObjects=" + focusObjects
        // );
        StepByStepTutorial._isPlaying = true;
        textHints = new string[stepCount];
        for (int index = 0; index < textHints.Length; index++)
        {
            textHints[index] = textKeyPrefix + index;
        }
    }

    void Start()
    {
        if (null == focusMaskManager)
        {
            focusMaskManager = FocusMaskManager.get();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (step < focusObjects.Length)
        {
            if (waited >= waitedThreshold)
            {
                if (!prepared)
                {
                    // Debug.Log(this.GetType() + " preparing step " + step + " searching for " + focusObjects[step]);
                    GameObject go = GameObject.Find(focusObjects[step]);
                    if (go == null)
                    {
                        Debug.LogError(this.GetType() + " couldn't find " + focusObjects[step]);
                        next();
                    }
                    else
                    {
                        // Debug.Log(this.GetType() + " go != null at step=" + step + ", go.transform.position=" + go.transform.position + " & go.transform.localPosition=" + go.transform.localPosition);
                        ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                        if (null != target)
                        {
                            // Debug.Log(this.GetType() + " target != null at step=" + step);
                            focusMaskManager.focusOn(target, next, textHints[step]);
                        }
                        else
                        {
                            // Debug.Log(this.GetType() + " target == null at step=" + step);
                            focusMaskManager.focusOn(go, next, textHints[step], true);
                        }
                        // Debug.Log(this.GetType() + " prepared step=" + step);
                        prepared = true;
                    }
                }
#if DEV
                else
                {
                    if(Input.GetKeyUp(KeyCode.Space))
                    {
                        next();
                    }
                }
#endif
            }
            waited += Time.fixedDeltaTime;
        }
        else
        {
            end();
        }
    }

    protected virtual void end()
    {
        focusMaskManager.stopFocusOn();
        StepByStepTutorial._isPlaying = false;
        Destroy(this);
    }
}

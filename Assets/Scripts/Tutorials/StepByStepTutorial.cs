// #define DEV
using UnityEngine;

public abstract class StepByStepTutorial : MonoBehaviour
{
    public const string moveDevice1 = "PRCONS:RBS3:MOV:DTER";
    public const string moveDevice2 = "PRCONS:RBS2:MOV:DTER";
    public const string moveDevice3 = "PRCONS:RBS1:MOV:DTER";

    private int _step = 0;
    private bool prepared = false;
    private float waited = 0;
    private const float waitedThreshold = 0f;

    protected const string _bacterium = Character.gameObjectName;
    protected const string _craftWindow = "CraftPanelSprite";
    protected const string _craftButton = "CraftButton";
    protected const string _backgroundSuffix = "Background";
    protected const string _bioBrickIconBackgroundSuffix = "BioBrickIconBackground";
    protected const string _exitCross = "CraftCloseButton";

    protected const string _listedPrefix = "l_";
    protected const string _equippedPrefix = "e_";
    protected const string _craftResultPrefix = "c_";
    protected const string _GFPdevice1 = "PRCONS:RBS3:FLUO1:DTER";
    protected const string _GFPdevice2 = "PRCONS:RBS2:FLUO1:DTER";
    protected const string _craftSlotBrickBaseName = CraftZoneManager._brickNameRoot;
    protected const string _slotBaseName = CraftZoneManager._slotNameRoot;
    protected const string _slotSelectionSpriteSuffix = "SelectionSprite";
    
    protected const string _craftSlot1 = _slotBaseName + "0" + _slotSelectionSpriteSuffix;
    protected const string _craftSlot2 = _slotBaseName + "1" + _slotSelectionSpriteSuffix;
    protected const string _craftSlot1Brick1 = _craftSlot1 + _craftSlotBrickBaseName + "0";
    protected const string _PBAD3Brick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "PRBAD3";
    protected const string _RBS2brick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "RBS2";
    protected const string _RBS1brick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "RBS1";
    protected const string _MOVbrick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "MOV";
    protected const string _GFPbrick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "FLUO1";
    protected const string _terminatorBrick = AvailableDisplayedBioBrick._availableDisplayedPrefix + "DTER";
    protected const string _PCONSBrickBackground = AvailableDisplayedBioBrick._availableDisplayedPrefix + "PRCONS" + _bioBrickIconBackgroundSuffix;
    protected const string _PBAD3BrickBackground = AvailableDisplayedBioBrick._availableDisplayedPrefix + "PRBAD3" + _bioBrickIconBackgroundSuffix;

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
        _step++;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + "Awake");

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
        // Debug.Log(this.GetType() + "Start");
        focusMaskManager = FocusMaskManager.get();
    }

    protected virtual bool skipStep(int step)
    {
        return false;
    }

    protected virtual void prepareStep(int step) {}

    // Update is called once per frame
    void Update()
    {
        if (_step < focusObjects.Length)
        {
            if (waited >= waitedThreshold)
            {
                if (!prepared)
                {
                    prepareStep(_step);

                    if (skipStep(_step))
                    {
                        next();
                    }
                    else
                    {
                        // Debug.Log(this.GetType() + " preparing step " + _step + " searching for " + focusObjects[_step]);
                        GameObject go = GameObject.Find(focusObjects[_step]);
                        if (go == null)
                        {
                            Debug.LogError(this.GetType() + " GameObject not found at step " + _step + ": " + focusObjects[_step]);
                            next();
                        }
                        else
                        {
                            // Debug.Log(this.GetType() + " go != null at step=" + _step + ", go.transform.position=" + go.transform.position + " & go.transform.localPosition=" + go.transform.localPosition);
                            ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                            if (null != target)
                            {
                                // Debug.Log(this.GetType() + " target != null at step=" + _step);
                                focusMaskManager.focusOn(target, next, textHints[_step], true);
                            }
                            else
                            {
                                // Debug.Log(this.GetType() + " target == null at step=" + _step);
                                focusMaskManager.focusOn(go, next, textHints[_step], true);
                            }
                            // Debug.Log(this.GetType() + " prepared step=" + _step);
                            prepared = true;
                        }
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

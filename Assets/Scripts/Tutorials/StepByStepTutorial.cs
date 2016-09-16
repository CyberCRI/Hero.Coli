using UnityEngine;

public abstract class StepByStepTutorial : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;
    private float waited = 0;
    private const float waitedThreshold = 0f;

    protected const string _bacterium = "Perso";
    protected const string _craftWindow = "CraftPanelSprite";
    protected const string _craftButton = "CraftButton";
    protected const string _backgroundSuffix = "Background";
    protected const string _exitCross = "CraftCloseButton";

    protected const string _availableDisplayedPrefix = "AvailableDisplayed";
    protected const string _listedPrefix = "l_";
    protected const string _equippedPrefix = "e_";
    protected const string _craftResultPrefix = "c_";
    protected const string _moveDevice1 = "PRCONS:RBS3:MOV:DTER";
    protected const string _moveDevice2 = "PRCONS:RBS2:MOV:DTER";
    protected const string _moveDevice3 = "PRCONS:RBS1:MOV:DTER";
    protected const string _craftSlot1 = "slot0SelectionSprite";
    protected const string _RBS2brick = _availableDisplayedPrefix + "RBS2";

    protected const string _genericTextKeyPrefix = "HINT.";
    private string[] textHints;

    protected abstract string textKeyPrefix { get; }
    protected abstract int stepCount { get; }
    protected abstract string[] focusObjects { get; }

    private Vector3 manualScale = new Vector3(440, 77, 1);

    public void next()
    {
        // Debug.Log("StepByStepTutorial next");
        prepared = false;
        waited = 0f;
        step++;
    }

    void Awake()
    {
        // Debug.Log("Awake " + this.GetType()
        // + " step=" + stepCount
        // + " prepared=" + prepared
        // + " waited=" + waited
        // + " textHints=" + textHints
        // + " textKeyPrefix=" + textKeyPrefix
        // + " stepCount=" + stepCount
        // + " focusObjects=" + focusObjects
        // );
        textHints = new string[stepCount];
        for (int index = 0; index < textHints.Length; index++)
        {
            textHints[index] = textKeyPrefix + index;
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
                    }
                    else
                    {
                        // Debug.Log("go != null at step=" + step + ", go.transform.position=" + go.transform.position + " & go.transform.localPosition=" + go.transform.localPosition);
                        ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                        if (null != target)
                        {
                            // Debug.Log("target != null at step=" + step);
                            FocusMaskManager.get().focusOn(target, next, textHints[step]);
                        }
                        else
                        {
                            // Debug.Log("target == null at step=" + step);
                            FocusMaskManager.get().focusOn(go, next, textHints[step], true);
                        }
                    }
                    // Debug.Log(this.GetType() + " prepared step=" + step);
                    prepared = true;
                }
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
        Destroy(this);
    }
}

using UnityEngine;

public abstract class StepByStepTutorial : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;

    protected const string _bacterium = "Perso";
    protected const string _craftWindow = "CraftPanelSprite";
    protected const string _craftButton = "CraftButton";
    protected const string _backgroundSuffix = "Background";
    protected const string _exitCross = "CraftCloseButton";

    protected const string _availableDisplayedPrefix = "AvailableDisplayed";
    protected const string _listedPrefix = "l_";
    protected const string _equippedPrefix = "e_";
    protected const string _craftResultPrefix = "c_";

    protected const string _genericTextKeyPrefix = "HINT.";
    private string[] textHints;

    protected abstract string textKeyPrefix { get; }    
    protected abstract int stepCount { get; }
    protected abstract string[] focusObjects { get; }    

    public void next()
    {
        // Debug.Log("StepByStepTutorial next");
        prepared = false;
        step++;
    }

    void Awake()
    {
        Debug.Log("Awake "+this+" with _stepCount="+stepCount);
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
            if (!prepared)
            {
                Debug.Log("StepByStepTutorial preparing step " + step + " searching for " + focusObjects[step]);
                GameObject go = GameObject.Find(focusObjects[step]);
                if (go == null)
                {
                    Debug.LogError("StepByStepTutorial " + this + " couldn't find " + focusObjects[step]);
                }
                else
                {
                    Debug.Log("go != null at step=" + step + ", go.transform.position="+go.transform.position+" & go.transform.localPosition="+go.transform.localPosition);
                    ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                    if (null != target)
                    {
                        Debug.Log("target != null at step=" + step);
                        FocusMaskManager.get().focusOn(target, next, textHints[step]);
                    }
                    else
                    {
                        Debug.Log("target == null at step=" + step);
                        FocusMaskManager.get().focusOn(go, next, textHints[step], true);
                    }
                }
                Debug.Log("StepByStepTutorial prepared step=" + step);
                prepared = true;
            }
        }
        else
        {
            Destroy(this);
        }
    }
}

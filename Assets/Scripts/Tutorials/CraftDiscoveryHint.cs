using UnityEngine;

public class CraftDiscoveryHint : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;

    private const string _craftWindow = "CraftPanelSprite";
    private const string _slowMoveDevice = "PRCONS:RBS3:MOV:DTER";
    private const string _cellPanelEquippedDevice = "e_" + _slowMoveDevice;
    private const string _craftButton = "CraftButton";
    private const string _backgroundSuffix = "Background";
    private const string _craftResultDeviceBackground = "c_" + _slowMoveDevice + _backgroundSuffix;    
    private const string _craftSlot = "slot0SelectionSprite";
    private const string _exitCross = "CraftCloseButton";

    private const string _prefix = "AvailableDisplayed";
    private const string _brick1 = _prefix + "PRCONS";

    private const string _textKeyPrefix = "HINT.CRAFTDISCOVERY.";

    private Vector3 manualScale = new Vector3(440, 77, 1);

    private const int _stepCount = 6;
    private string[] focusObjects = new string[_stepCount] { _cellPanelEquippedDevice, _craftButton, _craftWindow, _craftResultDeviceBackground, _craftSlot, _exitCross };
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
        if (step < focusObjects.Length)
        {
            if (!prepared)
            {
                Debug.Log("preparing step " + step + " searching for "+focusObjects[step]);
                GameObject go = GameObject.Find(focusObjects[step]);
                if (go == null)
                {
                    Debug.LogError("couldn't find " + focusObjects[step]);
                }
                else
                {
                    Debug.Log("go != null at step="+step);
                    ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                    if (null != target)
                    {
                        Debug.Log("target != null at step="+step);
                        FocusMaskManager.get().focusOn(target, next, textHints[step]);
                    }
                    else
                    {
                        Debug.Log("target == null at step="+step);
                        if(4 == step)
                        {
                            Debug.Log("calling FocusMaskManager with manualScale="+manualScale);
                            FocusMaskManager.get().focusOn(go, true, manualScale, textHints[step], true);
                        }
                        else
                        {
                            Debug.Log("step != 4 at step="+step);
                            FocusMaskManager.get().focusOn(go, true, textHints[step], true);
                        }
                    }
                }
                Debug.Log("prepared step="+step);
                prepared = true;
            }
        }
        else
        {
            Destroy(this);
        }
    }
}

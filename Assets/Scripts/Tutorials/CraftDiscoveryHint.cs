using UnityEngine;

public class CraftDiscoveryHint : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;

    private const string _bacterium = "Perso";
    private const string _craftWindow = "CraftPanelSprite";
    private const string _slowMoveDevice = "PRCONS:RBS3:MOV:DTER";
    private const string _cellPanelEquippedDevice = "e_" + _slowMoveDevice;
    private const string _craftButton = "CraftButton";
    private const string _backgroundSuffix = "Background";
    private const string _craftResultDevice = "c_" + _slowMoveDevice;
    private const string _listedDevice = "l_" + _slowMoveDevice;
    private const string _craftResultDeviceBackground = _craftResultDevice + _backgroundSuffix;    
    private const string _craftSlot = "slot0SelectionSprite";
    private const string _exitCross = "CraftCloseButton";

    private const string _prefix = "AvailableDisplayed";
    private const string _brick1 = _prefix + "PRCONS";

    private const string _textKeyPrefix = "HINT.CRAFTDISCOVERY.";

    private Vector3 manualScale = new Vector3(440, 77, 1);

    private const int _stepCount = 12;
    private string[] focusObjects = new string[_stepCount] { 
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
    private string[] textHints = new string[_stepCount];

    public void next()
    {
        Debug.Log("CraftDiscoveryHint next");
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
                Debug.Log("CraftDiscoveryHint preparing step " + step + " searching for "+focusObjects[step]);
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
                        if(45 == step)
                        {
                            Debug.Log("calling FocusMaskManager with manualScale="+manualScale);
                            FocusMaskManager.get().focusOn(target, manualScale, next, textHints[step]);
                        }
                        else
                        {
                            Debug.Log("calling FocusMaskManager without manualScale");
                            FocusMaskManager.get().focusOn(target, next, textHints[step]);
                        }
                    }
                    else
                    {
                        Debug.LogError("target == null at step="+step);
                        FocusMaskManager.get().focusOn(go, next, textHints[step], true);
                    }
                }
                Debug.Log("CraftDiscoveryHint prepared step="+step);
                prepared = true;
            }
        }
        else
        {
            Destroy(this);
        }
    }
}

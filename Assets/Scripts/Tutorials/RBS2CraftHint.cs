using UnityEngine;

public class RBS2CraftHint : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;

    private const string _craftButton = "CraftButton";
    private const string _listedPrefix = "Listed";
    private const string _device = "PRCONS:RBS3:MOV:DTER";
    private const string _brick = "AvailableDisplayedRBS2";
    private const string _craftWindow = "CraftPanelSprite";
    private const string _exitCross = "CraftCloseButton";
    private const string _textKeyPrefix = "HINT.GFPCRAFT.";

    private const int _stepCount = 5;
    private string[] focusObjects = new string[_stepCount] { _craftButton, _listedPrefix + _device, _brick, _craftWindow, _exitCross };

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
                if ((1 == step) && (CraftFinalizer.get().isEquiped(_device)))
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
                            FocusMaskManager.get().focusOn(go, true, next, textHints[step], true);
                        }
                        prepared = true;
                        }
                }
            }
        }
        else
        {
            Destroy(this);
        }
    }
}

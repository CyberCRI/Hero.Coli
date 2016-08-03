using UnityEngine;

public class GFPCraftHint : MonoBehaviour
{

    private int step = 0;
    private bool prepared = false;

    private const string _craftButton = "CraftButton";
    private const string _listedPrefix = "Listed";
    private const string _device = "PRCONS:RBS2:MOV:DTER";
    private const string _brick = "AvailableDisplayedFLUO1";
    private const string _exitCross = "CraftCloseButton";

    private string[] focusObjects = new string[4] { _craftButton, _listedPrefix + _device, _brick, _exitCross };

    private string[] textHints = new string[4] { "HINT.GFPCRAFT.0", "HINT.GFPCRAFT.1", "HINT.GFPCRAFT.2", "HINT.GFPCRAFT.3" };

    public void next()
    {
        prepared = false;
        step++;
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
                    ExternalOnPressButton target = GameObject.Find(focusObjects[step]).GetComponent<ExternalOnPressButton>();
                    FocusMaskManager.get().focusOn(target, next, textHints[step]);
                    prepared = true;
                }
            }
        }
        else
        {
            Destroy(this);
        }
    }
}

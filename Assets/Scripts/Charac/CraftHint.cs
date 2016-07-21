using UnityEngine;

public class CraftHint : MonoBehaviour
{

    private int step = 0;
    public int bricks = 0;
    private bool prepared = false;

    private const string _craftButton = "CraftButton";
    private const string _prefix = "AvailableDisplayed";
    private const string _brick1 = _prefix + "PRCONS",
    _brick2 = _prefix + "RBS2",
    _brick3 = _prefix + "MOV",
    _brick4 = _prefix + "DTER";
    private const string _exitCross = "CraftCloseButton";

    private string[] focusObjects = new string[6] { _craftButton, _brick1, _brick2, _brick3, _brick4, _exitCross };

    public void next()
    {
        prepared = false;
        step++;
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
                    ExternalOnPressButton target = GameObject.Find(focusObjects[step]).GetComponent<ExternalOnPressButton>();
                    FocusMaskManager.get().focusOn(target, next);
                    prepared = true;
                }
            }
            else
            {
                Destroy(this);
            }
        }
    }
}

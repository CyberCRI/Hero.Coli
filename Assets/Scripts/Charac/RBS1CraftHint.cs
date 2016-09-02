using UnityEngine;

public class RBS1CraftHint : MonoBehaviour
{

    private bool prepared = false;

    private const string _craftButton = "CraftButton";
    private const string _hintText = "HINT.RBS1CRAFT.0";

    public void next()
    {
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!prepared)
        {
            ExternalOnPressButton target = GameObject.Find(_craftButton).GetComponent<ExternalOnPressButton>();
            FocusMaskManager.get().focusOn(target, next, _hintText);
            prepared = true;
        }
    }
}

using UnityEngine;

public class RBS1CraftHint : MonoBehaviour
{

    private bool prepared = false;

    private const string _craftButton = "CraftButton";

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
            FocusMaskManager.get().focusOn(target, next);
            prepared = true;
        }
    }
}

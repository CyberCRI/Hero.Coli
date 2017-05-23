using UnityEngine;

public class GraphicsOptionMainMenuItem : MainMenuItem
{
    [SerializeField]
    private ResolutionScript.RESOLUTION _resolution;
    [SerializeField]
    private ResolutionScript.ASPECTRATIO _ratio;

    private const string _keyPrefix = "MENU.GRAPHICS.";

    public override void click()
    {
        // Debug.Log(this.GetType() + " click " + itemName);

        GraphicsOptionsMainMenuItemArray array = transform.GetComponentInParent<GraphicsOptionsMainMenuItemArray>();
        if (null != array)
        {
            array.applyGraphicsConfiguration(_resolution, _ratio);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " clicked found no parent GraphicsOptionsMainMenuItemArray");
        }
    }

    public override void initialize()
    {
        // Debug.Log(this.GetType() + " initialize " + name + " with _resolution=" + _resolution + " and _ratio=" + _ratio + " with " + this.GetComponentInChildren<UILabel>().text);
        UILocalize localize = this.GetComponentInChildren<UILocalize>();
        if (null != localize)
        {
            localize.key = _resolution == ResolutionScript.RESOLUTION.NONE ? _keyPrefix+_ratio.ToString() : _keyPrefix+_resolution.ToString();
            localize.Localize();
        }
        
    }
}

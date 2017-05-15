using UnityEngine;

public class GraphicsOptionMainMenuItem : MainMenuItem
{
    [SerializeField]
    private ResolutionScript.RESOLUTION _resolution;
    [SerializeField]
    private ResolutionScript.ASPECTRATIO _ratio;

    public override void click()
    {
        Debug.Log(this.GetType() + " clicked " + itemName);

        GameObject[] scripts = Resources.FindObjectsOfTypeAll(typeof(ResolutionScript)) as GameObject[];

        if (null != scripts)
        {
            Debug.Log(this.GetType() + " clicked found " + scripts.Length + " scripts");

            foreach (GameObject go in scripts)
            {
                ResolutionScript script = go.GetComponent<ResolutionScript>();
                if (null != script)
                {
                    Debug.Log(this.GetType() + " clicked calling setResolution(" + _resolution + ", " + _ratio + ") on go=" + go.name);
                    script.setResolution(_resolution, _ratio);
                }
                else
                {
                    Debug.Log(this.GetType() + " clicked failed to find ResolutionScript on go=" + go.name);
                }
            }
        }
        else
        {
            Debug.LogWarning(this.GetType() + " clicked found 0 scripts");
        }

        CustomData data = new CustomData(CustomDataTag.GRAPHICS, _resolution.ToString());
        data.merge(new CustomData(CustomDataTag.GRAPHICS, _resolution.ToString()));
        RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, data);
    }

    public override void initialize()
    {
        this.GetComponentInChildren<UILabel>().text = _resolution == ResolutionScript.RESOLUTION.NONE ? _ratio.ToString() : _resolution.ToString();
    }
}

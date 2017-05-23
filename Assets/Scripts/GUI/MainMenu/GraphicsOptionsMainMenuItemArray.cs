using UnityEngine;

public class GraphicsOptionsMainMenuItemArray : MainMenuItemArray
{
    [SerializeField]
    private ResolutionScript[] _resolutionScripts;

    public void applyGraphicsConfiguration(ResolutionScript.RESOLUTION _resolution, ResolutionScript.ASPECTRATIO _ratio)
    {
        if (null != _resolutionScripts)
        {
            // Debug.Log(this.GetType() + " applyGraphicsConfiguration found " + _resolutionScripts.Length + " _resolutionScripts");

            foreach (ResolutionScript script in _resolutionScripts)
            {
                if (null != script)
                {
                    // Debug.Log(this.GetType() + " applyGraphicsConfiguration calling setResolution(" + _resolution + ", " + _ratio + ") on script=" + script.name);
                    script.setResolution(_resolution, _ratio);
                }
                else
                {
                    Debug.LogWarning(this.GetType() + " applyGraphicsConfiguration failed to find ResolutionScript on script=" + script.name);
                }
            }

            CustomData data = new CustomData(CustomDataTag.GRAPHICS, _resolution.ToString());
            data.merge(new CustomData(CustomDataTag.GRAPHICS, _resolution.ToString()));
            RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, data);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " applyGraphicsConfiguration found 0 _resolutionScripts");
        }
    }
}

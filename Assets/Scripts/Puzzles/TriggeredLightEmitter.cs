using UnityEngine;

public class TriggeredLightEmitter : TriggeredCascade {
    
    // The protein that is required to open the door
    public string protein = "FLUO1";
    
    // The color which will be applied to the bacterium
    public Color colorTo;
    
    // The value over which the door will be activated
    public float threshold;
}

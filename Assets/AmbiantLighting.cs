using UnityEngine;
using System.Collections;

public class AmbiantLighting : MonoBehaviour {

    [SerializeField]
    private Light _directionaleLight;
    [SerializeField]
    private Light _phenoLight;
    [SerializeField]
    private Light _spotLight;
    [SerializeField]
    private Color[] _color;
    [SerializeField]
    private GameObject _background;
    private Material[] _backgroundMaterial;

    /*Values : 
    Phenolight : 
    GFP classic = _phenoLight, _color[1] (8,255,0,0), 24, 4

    SpotLight : 
    GFP Classic = _spotLight, _color[2] (105,255,118,255), 57.5, 5.3
    
    */

    public void ChangeLightColor(Light light, Color color)
    {
        light.color = color;
    }

    public void ChangeLightRange(Light light, float range)
    {
        light.range = range;
    }

    public void ChangeLightIntensity(Light light, float intensity)
    {
        light.intensity = intensity;
    }

    public void ChangeLightProperty(Light light, Color color, float range, float intensity)
    {
        light.color = color;
        light.range = range;
        light.intensity = intensity;
    }

    public void EnableLight(Light light, bool value)
    {
        light.enabled = value;
    }
}

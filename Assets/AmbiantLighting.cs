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

    /*Values : 
    GFP classic = _phenoLight, _color[1] (8,255,0), 24, 4
    
    */



    // Use this for initialization
    void Start () {
        /*EnableLight(_directionaleLight, false);
        ChangeLightProperty(_phenoLight, _color[2], 10, 8);
        ChangeLightProperty(_spotLight, _color[2], 57.5f, 8);
        EnableLight(_phenoLight, true);
        EnableLight(_spotLight, true);*/
        ChangeLightProperty(_phenoLight, _color[1], 24, 4);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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

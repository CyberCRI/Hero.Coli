using UnityEngine;
using System.Collections;

public class NpcLighting : MonoBehaviour {

    [SerializeField]
    private Light _phenoLight;
    [SerializeField]
    private Light _spotLight;
    [SerializeField]
    private Color _color;
    [SerializeField]
    private TriggeredDoor _triggeredDoor;

    // Use this for initialization
    void Start () {
	
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
        light.range = range;
        light.intensity = intensity;
        light.color = color;
    }

    public void EnableLight(Light light, bool value)
    {
        light.enabled = value;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "NPCLight")
        {
            ChangeLightProperty(_spotLight, _color, 250, 8);
            ChangeLightProperty(_phenoLight, _color, 250, 8);
            EnableLight(_spotLight, true);
            EnableLight(_phenoLight, true);
            _triggeredDoor.triggerStart();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "NPCLight")
        {
            EnableLight(_spotLight, false);
            EnableLight(_phenoLight, false);
            _triggeredDoor.triggerExit();
        }
    }
}

using UnityEngine;

public class NpcLighting : TriggeredCascade
{

    [SerializeField]
    private Light _phenoLight;
    [SerializeField]
    private Light _spotLight;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "NPCLight")
        {
            _spotLight.enabled = true;
            _phenoLight.enabled = true;
            triggerStart();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "NPCLight")
        {
            triggerStay();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "NPCLight")
        {
            _spotLight.enabled = false;
            _phenoLight.enabled = false;
            triggerExit();
        }
    }
}

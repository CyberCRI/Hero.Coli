using UnityEngine;
using System.Collections.Generic;

public class Sector : MonoBehaviour {

    protected static string staticName = "Sector";
    public int id;
    public string genericName {
        get {
            return staticName+id;
        }
    }
    public string customName;
    public string sectorName {
        get {
            string result = string.IsNullOrEmpty(customName)?genericName:customName;
            return result;
        }
    }
    
    protected static List<Sector> allSectors = new List<Sector>();

    // The game objects comprised by the sector, some of which may have to be inactivated/activated sometimes
    public GameObject[] elements;
    public Sector[] neighboringSectors;
    
    private bool _areEmittersOn = true;
    private List<ParticleEmitter> _particleEmitters = new List<ParticleEmitter>();
    private List<ParticleSystem> _particleSystems = new List<ParticleSystem>();
    private List<ForceFlowParticle> _forceFlowParticles = new List<ForceFlowParticle>();
    
    public bool switchOnNextFrame = false;
    public bool initializeOnNextFrame = false;

    private bool _initialized = false;

    // Switches all elements to ON or OFF
    // Assumes the elements list doesn't change over time 
    // See commit #37b96ae692f49557f781b03e466de59d001d0ffc for runtime changes 
    private void switchTo(bool active)
    {   
        
        if(!_initialized)
        {
            initialize();
        }
        
        // Case: switch on
        if(!_areEmittersOn && active)
        {
            foreach (ParticleEmitter pe in _particleEmitters)
            {
                if(null != pe)
                {
                    pe.gameObject.SetActive(true);
                    pe.enabled = true;   
                }
            }
            
            foreach (ParticleSystem ps in _particleSystems)
            {
                if(null != ps)
                {
                    ps.gameObject.SetActive(true);
                    ParticleSystem.EmissionModule em = ps.emission;
                    em.enabled = true;   
                }
            }
            
            foreach (ForceFlowParticle ffp in _forceFlowParticles)
            {
                if(null != ffp)
                {
                    ffp.gameObject.SetActive(true);
                    ffp.enabled = true;   
                }
            }
            
            _areEmittersOn = true;
        }
        // Case: switch off
        else if(_areEmittersOn && !active)
        {
            foreach (ParticleEmitter pe in _particleEmitters)
            {
                pe.gameObject.SetActive(false);
            }
            foreach (ParticleSystem ps in _particleSystems)
            {
                ps.gameObject.SetActive(false);
            }
            foreach (ForceFlowParticle ffp in _forceFlowParticles)
            {
                ffp.gameObject.SetActive(false);
            }
            
            _areEmittersOn = false;
        }
    }

    public void switchOn()
    {
        switchTo(true);
    }
    
    public void switchOff()
    {
        switchTo(false);
    }
    
    public void activate()
    {        
        this.switchOn();
        
        bool contains = false;
        
        foreach(Sector sector in allSectors)
        {
            if(sector.id != this.id)
            {
                contains = false;
                if((null != neighboringSectors) && (0 != neighboringSectors.Length))
                {
                    foreach(Sector neighbor in neighboringSectors)
                    {
                        if(sector == neighbor)
                        {
                            contains = true;
                            break;
                        }
                    }
                }
                if(!sector._areEmittersOn && contains)
                {
                    sector.switchOn();
                }
                else if(sector._areEmittersOn && !contains)
                {
                    sector.switchOff();
                }
            }
        }        
    }

    void Awake()
    {
        //Debug.Log(string.Format("{0}::Awake starts with #emitters={1}, #systems={2}, #forceFlows={3}", sectorName, _particleEmitters.Count, _particleSystems.Count, _forceFlowParticles.Count));
        // Make inventory of all relevant elements
        
        if(!_initialized)
        {
            initialize();
        }
        //Debug.Log(string.Format("{0}::Awake ends with #emitters={1}, #systems={2}, #forceFlows={3}", sectorName, _particleEmitters.Count, _particleSystems.Count, _forceFlowParticles.Count));
    }
    
    void initialize()
    {
        _particleEmitters.Clear();
        _particleSystems.Clear();
        _forceFlowParticles.Clear();
        foreach(GameObject go in elements)
        {
            foreach(ParticleEmitter pe in go.GetComponentsInChildren<ParticleEmitter>())
            {
                if(pe.gameObject.activeSelf && pe.enabled)
                {
                    _particleEmitters.Add(pe);
                }
            }
            foreach(ParticleSystem ps in go.GetComponentsInChildren<ParticleSystem>())
            {
                if(ps.gameObject.activeSelf)
                {                    
                    ParticleSystem.EmissionModule em = ps.emission;
                    if(em.enabled)
                    {   
                        _particleSystems.Add(ps);
                    }
                }
            }
            foreach(ForceFlowParticle ffp in go.GetComponentsInChildren<ForceFlowParticle>())
            {
                if(ffp.gameObject.activeSelf && ffp.enabled)
                {
                    _forceFlowParticles.Add(ffp);
                }
            }
        }
        
        _initialized = true;
        
        // switchOff contains a call to initialize conditioned by _initialized,
        // therefore _initialized is set to true before the call to switchOff.
        switchOff();
        
        allSectors.Add(this);
    }
    
	// Update is called once per frame
    void Update () {
                
        if(initializeOnNextFrame)
        {
            initialize();
            initializeOnNextFrame = false;
       }
        
	   if(switchOnNextFrame)
       {
           switchTo(!_areEmittersOn);
           switchOnNextFrame = false;
       }
	}
}

using UnityEngine;
using System.Collections.Generic;


public class LimitedBiobricksCraftZoneManager : CraftZoneManager
{

    protected new LinkedList<BioBrick> _currentBioBricks {
        get {
            if(null != activeSlot)
            {
                return activeSlot.getCurrentBricks();
            }
            else
            {
                return new LinkedList<BioBrick>();   
            } 
        }
    }

    protected List<CraftDeviceSlot> slots = new List<CraftDeviceSlot>();
    protected CraftDeviceSlot activeSlot;
    
    public GameObject slotPrefab;
    
    /* TODO initialize slot list
    void Start()
    {
        
    }
    */
  
  public override void initialize()
  {
      Debug.LogError("LimitedBiobricksCraftZoneManager initialize");
      
      slots.Clear();
      
      GameObject activeSlotGO = GameObject.Instantiate(craftSlotDummy);
      activeSlotGO.transform.parent = craftSlotDummy.transform.parent;
      activeSlotGO.transform.localScale = craftSlotDummy.transform.localScale;
      activeSlotGO.transform.localPosition = craftSlotDummy.transform.localPosition;      
      activeSlotGO.SetActive(true);
      
      activeSlot = activeSlotGO.GetComponent<CraftDeviceSlot>();
      
      slots.Add(activeSlot);
      
      base.initialize();      
  }

    public override void replaceWithBioBrick(BioBrick brick)
    {
        Debug.LogError("replaceWithBioBrick("+brick.getName()+")");
        if (null != activeSlot)
        {
            Debug.LogError("null != activeSlot");
            activeSlot.addBrick(brick);
        }
        else
        {
            Debug.LogError("null == activeSlot");
        }
    }

    public void addBrick(CraftZoneDisplayedBioBrick brick)
    {
        if (null != activeSlot)
        {
            activeSlot.addBrick(brick);
        }
    }
    /*
    public void removeBioBrick(CraftZoneDisplayedBioBrick brick)
    {
        Debug.LogError("LimitedBiobricksCraftZoneManager::removeBioBrick(czdb)");
        if(null != brick)
        {
            Debug.LogError("removeBioBrick null != brick");
            foreach(CraftDeviceSlot slot in slots)
            {
                Debug.LogError("removeBioBrick slot "+slot);
                if(null != slot && slot.removeBrick(brick))
                {
                    return;
                }   
            }
        }
    }
    */
    public override void removeBioBrick(BioBrick brick)
    {
        Debug.LogError("LimitedBiobricksCraftZoneManager::removeBioBrick(brick)");
        if(null != brick)
        {
            Debug.LogError("removeBioBrick null != brick");
            foreach(CraftDeviceSlot slot in slots)
            {
                Debug.LogError("removeBioBrick slot "+slot);
                if(null != slot && slot.removeBrick(brick))
                {
                    return;
                }   
            }
        }
    }
    
    
    public override Device getCurrentDevice() {
        Debug.LogError("LimitedBiobricksCraftZoneManager getCurrentDevice");
        return activeSlot.getCurrentDevice();
    }
    
    public override void OnBioBricksChanged()
    {
        foreach(CraftDeviceSlot slot in slots)
        {
            slot.updateDisplay();
        }
    }
    
    public override void setDevice(Device device) {
        Debug.LogError("LimitedBioBricksCraftZoneManager::setDevice("+device+")");
        if(null != activeSlot)
       {
           activeSlot.setDevice(device);
       } 
    }
    
    protected override void removeAllBricksFromCraftZone() {
       if(null != activeSlot)
       {
           activeSlot.removeAllBricks();
       }
    }
}



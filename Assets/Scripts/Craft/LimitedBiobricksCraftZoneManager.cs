using UnityEngine;
using System.Collections.Generic;


public class LimitedBiobricksCraftZoneManager : CraftZoneManager
{

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
      
      GameObject activeSlotGO = GameObject.Instantiate(craftSlotDummy);
      activeSlotGO.transform.parent = craftSlotDummy.transform.parent;
      activeSlotGO.transform.localScale = craftSlotDummy.transform.localScale;
      activeSlotGO.transform.localPosition = craftSlotDummy.transform.localPosition;      
      activeSlotGO.SetActive(true);
      
      activeSlot = activeSlotGO.GetComponent<CraftDeviceSlot>();
      
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
    
    
    public void removeBioBrick(CraftZoneDisplayedBioBrick brick)
    {
        if(null != brick)
        {
            removeBioBrick(brick._biobrick);
        }
    }
    
    public override void removeBioBrick(BioBrick brick)
    {
        if(null != brick)
        {
            foreach(CraftDeviceSlot slot in slots)
            {
                if(null != slot)
                {
                    slot.removeBrick(brick);
                }   
            }
        }
    }
    
    public override void OnBioBricksChanged()
    {
        foreach(CraftDeviceSlot slot in slots)
        {
            slot.updateDisplay();
        }
    }
}

public class ActivationButton { }

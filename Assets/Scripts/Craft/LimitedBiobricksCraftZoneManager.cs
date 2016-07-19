using UnityEngine;
using System.Collections.Generic;


public class LimitedBiobricksCraftZoneManager : CraftZoneManager
{

    private bool _initialized = false;
    protected List<CraftDeviceSlot> slots = new List<CraftDeviceSlot>();
    protected CraftDeviceSlot selectedSlot;
    protected int slotCount = 4;
    public GameObject                         craftSlotDummy1;
    public GameObject                         craftSlotDummy2;

    protected new LinkedList<BioBrick> _currentBioBricks {
        get {
            if(null != selectedSlot)
            {
                return selectedSlot.getCurrentBricks();
            }
            else
            {
                return new LinkedList<BioBrick>();   
            } 
        }
    }

    public override void initialize()
    {
        //Debug.LogError("LimitedBiobricksCraftZoneManager initialize");

        if (!_initialized)
        {
            slots.Clear();

            GameObject slotGO;
            CraftDeviceSlot slot;
            float offset = craftSlotDummy1.transform.localPosition.y
            - craftSlotDummy2.transform.localPosition.y;
            Vector3 firstPosition = 
            (craftSlotDummy1.transform.localPosition +
            craftSlotDummy2.transform.localPosition) / 2
            + (slotCount - 1) * offset * Vector3.up / 2;

            for (int index = 0; index < slotCount; index++)
            {
                slotGO = GameObject.Instantiate(craftSlotDummy1);
                slotGO.transform.parent = craftSlotDummy1.transform.parent;
                slotGO.transform.localScale = craftSlotDummy1.transform.localScale;
                slotGO.transform.localPosition = firstPosition - new Vector3(0, index * offset, 0);

                slot = slotGO.GetComponent<CraftDeviceSlot>();

                slots.Add(slot);
            }

            selectSlot(slots[0]);
            
            Destroy(craftSlotDummy1);
            Destroy(craftSlotDummy2);

            base.initialize();

            _initialized = true;
        }
    }

    public void selectSlot(CraftDeviceSlot slot)
    {
        //Debug.LogError("LimitedBiobricksCraftZoneManager selectSlot");
        if (null != slot)
        {
            if (null != selectedSlot)
            {
                selectedSlot.setSelectedBackground(false);
            }
            selectedSlot = slot;
            //Debug.LogError("selectSlot selectedSlot.setSelectedBackground(true); with selectedSlot="+selectedSlot);
            selectedSlot.setSelectedBackground(true);
        }
    }

    public override void replaceWithBioBrick(BioBrick brick)
    {
        //Debug.LogError("replaceWithBioBrick("+brick.getName()+")");
        if (null != selectedSlot)
        {
            //Debug.LogError("null != selectedSlot");
            selectedSlot.addBrick(brick);
        }
        else
        {
            //Debug.LogError("null == selectedSlot");
        }
    }

    public void addBrick(CraftZoneDisplayedBioBrick brick)
    {
        if (null != selectedSlot)
        {
            selectedSlot.addBrick(brick);
        }
    }
    
    public void removeBioBrick(CraftZoneDisplayedBioBrick brick)
    {
        //Debug.LogError("LimitedBiobricksCraftZoneManager::removeBioBrick(czdb)");
        if(null != brick)
        {
            //Debug.LogError("removeBioBrick null != brick");
            foreach(CraftDeviceSlot slot in slots)
            {
                //Debug.LogError("removeBioBrick slot "+slot);
                if(null != slot && slot.removeBrick(brick))
                {
                    return;
                }   
            }
        }
    }
    /*
    //unsafe if distinct genetic constructs have same brick
    public override void removeBioBrick(BioBrick brick)
    {
        //Debug.LogError("LimitedBiobricksCraftZoneManager::removeBioBrick(brick)");
        if(null != brick)
        {
            //Debug.LogError("removeBioBrick null != brick");
            foreach(CraftDeviceSlot slot in slots)
            {
                //Debug.LogError("removeBioBrick slot "+slot);
                if(null != slot && slot.removeBrick(brick))
                {
                    return;
                }   
            }
        }
    }
    */
    
    public override Device getCurrentDevice() {
        //Debug.LogError("LimitedBiobricksCraftZoneManager getCurrentDevice");
        return selectedSlot.getCurrentDevice();
    }
    
    public override void OnBioBricksChanged()
    {
        foreach(CraftDeviceSlot slot in slots)
        {
            slot.updateDisplay();
        }
    }
    
    public override void setDevice(Device device) {
        //Debug.LogError("LimitedBioBricksCraftZoneManager::setDevice("+device+")");
        if(null != selectedSlot)
       {
           selectedSlot.setDevice(device);
       } 
    }
    
    protected override void removeAllBricksFromCraftZone() {
       if(null != selectedSlot)
       {
           selectedSlot.removeAllBricks();
       }
    }
    
    //TODO add "new recipe" feedback
    public override void craft () {
        //Debug.LogError("LimitedBioBricksCraftZoneManager::craft");
    }
}



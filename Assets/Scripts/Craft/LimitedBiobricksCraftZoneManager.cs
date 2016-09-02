using UnityEngine;
using System.Collections.Generic;


public class LimitedBiobricksCraftZoneManager : CraftZoneManager
{

    private bool _initialized = false;
    protected List<CraftDeviceSlot> slots = new List<CraftDeviceSlot>();
    protected CraftDeviceSlot selectedSlot;
    protected int slotCount;
    [SerializeField]
    private GameObject slotPrefab;
    public Transform slotsGrid;

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
            
            slotCount = MemoryManager.get().configuration.gameMap==GameConfiguration.GameMap.SANDBOX2?5:1;
            // Debug.Log("going to destroy children slots");
            for(int index = 0; index < slotsGrid.childCount; index++)
            {
                Destroy(slotsGrid.GetChild(index).gameObject);
            }
            // Debug.Log("going to add children slots");
            for (int index = 0; index < slotCount; index++)
            {
                addSlot();
            }
            // Debug.Log("done children slots");
            selectSlot(slots[0]);

            base.initialize();

            _initialized = true;
        }
    }

    public void addSlot()
    {
        // Debug.Log("adding slot #"+slotsGrid.childCount);
        GameObject slotGO = GameObject.Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, slotsGrid) as GameObject;
        slotGO.transform.localPosition = new Vector3(slotGO.transform.localPosition.x, slotGO.transform.localPosition.y, 0);
        CraftDeviceSlot slot = slotGO.GetComponent<CraftDeviceSlot>();
        slots.Add(slot);
        slotsGrid.GetComponent<UIGrid>().repositionNow = true;
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            addSlot();
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

/*
    public void addBrick(CraftZoneDisplayedBioBrick brick)
    {
        if (null != selectedSlot)
        {
            selectedSlot.addBrick(brick);
        }
    }
    */
    
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
    
    public override void addAndEquipDevice(Device device)
    {
        if(Device.isValid(device))
        {
            //add every brick to the stock
            foreach (BioBrick brick in device.getExpressionModules().First.Value.getBioBricks())
            {
                AvailableBioBricksManager.get ().addAvailableBioBrick (brick, false);
            }
            //set device
            setDevice(device);
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



using UnityEngine;
using System.Collections.Generic;

public class CraftDeviceSlot : MonoBehaviour
{
    // GUI elements
    public UISprite craftSlotSprite;
    private const string slotActiveSprite = "craft_slot_active";
    private const string slotInactiveSprite = "craft_slot_inactive";
    public UISprite selectionSprite;
    private const string slotSelectedSprite = "transparent-light-grey";
    private const string slotUnselectedSprite = "transparent-grey";
    public CraftResultDevice resultDevice;
    public UISprite resultSprite;
    private const string resultActiveSprite = "craft_result_active";
    private const string resultInactiveSprite = "craft_result_inactive";

    // logical elements
    protected CraftZoneDisplayedBioBrick[] currentBricks = new CraftZoneDisplayedBioBrick[4];
    
    public GameObject[] dummyBrickGameObjects;
    
    protected bool _isEquiped;
    public bool isEquiped
    {
        get
        {
            return _isEquiped;
        }
        set
        {
            if (!_isEquiped && value)
            {
                //Debug.LogError("set: !_isEquiped && value");
                Device currentDevice = getCurrentDevice();
                _isEquiped = (Equipment.AddingResult.SUCCESS == Equipment.get().askAddDevice(currentDevice));
                if (_isEquiped)
                {
                    if(null != craftSlotSprite)
                    {
                        //Debug.LogError("slotActiveSprite");
                        craftSlotSprite.spriteName = slotActiveSprite;
                    }
                    if(null != resultSprite)
                    {
                        resultSprite.spriteName = resultActiveSprite;
                    }
                    //set result device
                    if(null != resultDevice)
                    {
                        CraftResultDevice.Initialize(resultDevice, currentDevice, null, DevicesDisplayer.DeviceType.Listed);
                        resultDevice.gameObject.SetActive(true);
                    }                    
                }
            }
            else if(_isEquiped && !value)
            {
                //Debug.LogError("set: _isEquiped && !value");
                _isEquiped = false;
                Equipment.get().removeDevice(getCurrentDevice());
                if (null != craftSlotSprite)
                {
                    //Debug.LogError("slotInactiveSprite");
                    craftSlotSprite.spriteName = slotInactiveSprite;
                }
                if(null != resultSprite)
                {
                    resultSprite.spriteName = resultInactiveSprite;
                }
                //set result device
                if(null != resultDevice)
                {
                    resultDevice.gameObject.SetActive(false);
                }
            }
            else
            {
                //Debug.LogError("none");
            }
        }
    }

    protected void equip()
    {
        //Debug.LogError("equip");
        isEquiped = true;
    }

    protected void unequip()
    {
        //Debug.LogError("unequip");
        isEquiped = false;
    }
    
    // isLocked == active: active == true when the device is working and protected from edition
    /*
    protected bool _isLocked;
    public bool isLocked
    {
        get
        {
            return _isLocked;
        }
        set
        {
            if (!value || (null != _resultDevice))
            {
                _isLocked = value;
                if (null != craftSlotSprite)
                {
                    craftSlotSprite.spriteName = value ? slotActive : slotInactive;
                }
                if (null != _resultDevice)
                {
                    if (value)
                    {
                        Equipment.get().askAddDevice(_resultDevice);
                    }
                    else
                    {
                        Equipment.get().removeDevice(_resultDevice);
                    }
                }
                updateDisplay();
            }
        }
    }

    public void toggleActivation()
    {
        if (isLocked)
        {
            deactivate();
        }
        else
        {
            activate();
        }
    }

    protected void activate()
    {
        isLocked = true;
    }

    protected void deactivate()
    {
        isLocked = false;
    }
    */

    public int getIndexFromType(BioBrick.Type bbType)
    {
        int result = 0;
        switch (bbType)
        {
            case BioBrick.Type.PROMOTER:
                result = 0;
                break;
            case BioBrick.Type.RBS:
                result = 1;
                break;
            case BioBrick.Type.GENE:
                result = 2;
                break;
            case BioBrick.Type.TERMINATOR:
                result = 3;
                break;
            default:
                //Debug.LogError("unrecognized BioBrick type " + bbType);
                break;
        }
        return result;
    }

    public int getIndexFromBrick(BioBrick brick)
    {
        int result = 0;
        if (null != brick)
        {
            result = getIndexFromType(brick.getType());
        }
        else
        {
            //Debug.LogError("bad brick '" + brick + "'");
        }
        return result;
    }

    public int getIndexFromBrick(CraftZoneDisplayedBioBrick brick)
    {
        int result = 0;
        if (null != brick)
        {
            result = getIndexFromBrick(brick._biobrick);
        }
        else
        {
            //Debug.LogError("bad brick '" + brick + "'");
        }
        return result;
    }

    public void addBrick(BioBrick brick)
    {
        //Debug.LogError("addBrick("+brick.getName()+")");
        //if (!isLocked && (null != brick))
        if (null != brick)
        {
            int index = getIndexFromBrick(brick);
            
            //Debug.LogError("!isLocked && (null != brick); index="+index);

            // create new CraftZoneDisplayedBioBrick 
            CraftZoneDisplayedBioBrick czdb = CraftZoneDisplayedBioBrick.Create(
                this.transform,
                dummyBrickGameObjects[index].transform.localPosition,
                null,
                brick
            );
            czdb.slot = this;
            addBrick(czdb);
            AvailableBioBricksManager.get().addBrickAmount(brick, -1);
        }
    }

    public void addBrick(CraftZoneDisplayedBioBrick brick)
    {
        //Debug.LogError("addBrick(czdb)");
        //if (!isLocked && (null != brick) && (null != brick._biobrick))
        if ((null != brick) && (null != brick._biobrick))
        {
            //Debug.LogError("!isLocked && (null != brick) && (null != brick._biobrick)");
            int index = getIndexFromBrick(brick);
            removeBrick(currentBricks[index]);
            currentBricks[index] = brick;
            
            checkDevice();
            
            updateDisplay();
        }
    }
    
    private void checkDevice()
    {
        if(
            (null != currentBricks[0])
            && (null != currentBricks[1])
            && (null != currentBricks[2])
            && (null != currentBricks[3])
        )
        {
            CraftFinalizer.get().finalizeCraft();
            
            equip();
        }
        else
        {
            unequip();
        }
    }

    public Device getCurrentDevice() {
        //Debug.LogError("CraftDeviceSlot getCurrentDevice");
        if(
            (null != currentBricks[0])
            && (null != currentBricks[1])
            && (null != currentBricks[2])
            && (null != currentBricks[3])
        )
        {
            List<BioBrick> currentBricksL = new List<BioBrick>{currentBricks[0]._biobrick,currentBricks[1]._biobrick,currentBricks[2]._biobrick,currentBricks[3]._biobrick};
            LinkedList<BioBrick> currentBricksLL = new LinkedList<BioBrick>(currentBricksL);
            return CraftZoneManager.get().getDeviceFromBricks(currentBricksLL);
        }
        return null;
    }
    
    public LinkedList<BioBrick> getCurrentBricks()
    {
        LinkedList<BioBrick> result = new LinkedList<BioBrick>();
        for(int i = 0; i < 4; i++)
        {
            if (null != currentBricks[i])
            {
                result.AddLast(currentBricks[i]._biobrick);
            }
        }
        return result;
    }
    
    public bool removeBrick(BioBrick brick)
    {
        //Debug.LogError("removeBrick(brick)");
        if (null != brick)
        {
            //Debug.LogError("removeBrick(brick="+brick.getName()+")");
            foreach (CraftZoneDisplayedBioBrick czdb in currentBricks)
            {
                if ((null != czdb) && (czdb._biobrick == brick))
                {
                    innerRemoveBrick(czdb);
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool removeBrick(CraftZoneDisplayedBioBrick brick)
    {
        //Debug.LogError("removeBrick(czdb)");
        if(null != brick)
        {
            //Debug.LogError("removeBrick(czdb="+brick._biobrick.getName()+")");   
            foreach (CraftZoneDisplayedBioBrick czdb in currentBricks)
            {
                if(czdb == brick)
                {
                    innerRemoveBrick(brick);
                    return true;
                }
            }
            //Debug.LogError("failed to find czdb brick to remove called '"+brick._biobrick.getName()+"'");
        }
        else
        {
            //Debug.LogError("null == czdb");
        }
        return false;
    }

    private void innerRemoveBrick(CraftZoneDisplayedBioBrick brick)
    {
        //Debug.LogError("innerRemoveBrick("+brick._biobrick.getName()+")");
        //isLocked = false;
        setSelected(true);
        unequip();
        AvailableBioBricksManager.get().addBrickAmount(brick._biobrick, 1);
        GameObject.Destroy(brick.gameObject);
        currentBricks[getIndexFromType(brick._biobrick.getType())] = null;        
    }
    
    public void removeAllBricks()
    {
        removeBrick(currentBricks[0]);
        removeBrick(currentBricks[1]);
        removeBrick(currentBricks[2]);
        removeBrick(currentBricks[3]);
        if(isEquiped)
        {
            unequip();
        }
    }
    
    public void setDevice(Device device)
    {
        if(device != null)
        {
            //Debug.LogError("CDS setDevice");
            //check that can afford device            
            LinkedList<BioBrick> newBricks = device.getExpressionModules().First.Value.getBioBricks();
            LinkedList<BioBrick> currentBricks = getCurrentBricks();
            AvailableBioBricksManager abbm = AvailableBioBricksManager.get(); 
            
            foreach(BioBrick brick in newBricks)
            {
                if(!currentBricks.Contains(brick))
                {
                    BioBrick abb = abbm.getBioBrickFromAll(brick.getName());
                    if(0 == abb.amount) 
                    {
                        //Debug.LogError("CDS setDevice abort with "+brick.getName());
                        // can't afford; abort
                        return;
                    }
                }
            }
            
            //Debug.LogError("CDS setDevice removeAllBricks");
            removeAllBricks();
            
            foreach(BioBrick brick in newBricks)
            {
                addBrick(brick);
            }
            
            updateDisplay();
        }
    }

    public void setSelected(bool selected)
    {
        //Debug.LogError("CraftDeviceSlot selectSlot("+selected+")");
        LimitedBiobricksCraftZoneManager lbczm = ((LimitedBiobricksCraftZoneManager)CraftZoneManager.get());
        if (null != lbczm)
        {
            lbczm.selectSlot(this);
        }
    }
    
    public void setSelectedBackground(bool selected)
    {
        //Debug.LogError("CraftDeviceSlot setSelectedBackground("+selected+")");
        selectionSprite.spriteName = selected?slotSelectedSprite:slotUnselectedSprite;        
    }

    public void updateDisplay()
    {
        for (int index = 0; index < 4; index++)
        {
            if (null != currentBricks[index])
            {
                currentBricks[index].gameObject.transform.localPosition = dummyBrickGameObjects[index].transform.localPosition;
                currentBricks[index].gameObject.SetActive(true);
            }
        }
    }
    
    void initialize()
    {
        _isEquiped = true;
        isEquiped = false;
    }
    
    void Awake()
    {
        initialize();
    }
}
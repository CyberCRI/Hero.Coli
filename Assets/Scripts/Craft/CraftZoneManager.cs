using UnityEngine;
using System.Collections.Generic;

/*
 * TODO:
 * Replace LinkedList by an array or fields
 * OnHover for DisplayedBioBrick
 * OnPress for AvailableDisplayedBioBrick + Update of _currentDisplayedBricks in CraftZone
 * OnPress for CraftZoneDisplayedBioBrick + Update of _currentDisplayedBricks in CraftZone
 * Update of state of CraftFinalizationButton
 */

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class CraftZoneManager : MonoBehaviour
{


    //////////////////////////////// singleton fields & methods ////////////////////////////////
    protected const string gameObjectName = "CraftZoneManager";
    protected static CraftZoneManager _instance;
    public static CraftZoneManager get()
    {
        // Debug.Log("CraftZoneManager get");
        if (_instance == null)
        {
            Debug.LogWarning("CraftZoneManager get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<CraftZoneManager>();
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        initializeIfNecessary();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////

    private const int sandboxSlotCount = 10;
    private const int tutorialSlotCount = 1;
    protected List<CraftDeviceSlot> slots = new List<CraftDeviceSlot>();
    protected CraftDeviceSlot selectedSlot;
    protected int slotCount;
    [SerializeField]
    private GameObject slotPrefab;
    private const string _slotNameRoot = "slot";
    public Transform slotsGrid;
    protected LinkedList<CraftZoneDisplayedBioBrick> _currentDisplayedBricks = new LinkedList<CraftZoneDisplayedBioBrick>();
    protected Device _currentDevice = null;
    public CraftFinalizer craftFinalizer;
    public GameObject assemblyZonePanel;
    protected static EditMode editMode = EditMode.UNLOCKED;
    protected bool _initialized = false;

    protected enum EditMode
    {
        LOCKED,
        UNLOCKED
    }

    public LinkedList<CraftZoneDisplayedBioBrick> getCurrentDisplayedBricks()
    {
        return new LinkedList<CraftZoneDisplayedBioBrick>(_currentDisplayedBricks);
    }

    public static bool isDeviceEditionOn()
    {
        return EditMode.UNLOCKED == editMode;
    }

    public static void setDeviceEdition(bool editionOn)
    {
        editMode = editionOn ? EditMode.UNLOCKED : EditMode.LOCKED;
    }

    protected LinkedList<BioBrick> _currentBioBricks
    {
        get
        {
            if (null != selectedSlot)
            {
                return selectedSlot.getCurrentBricks();
            }
            else
            {
                return new LinkedList<BioBrick>();
            }
        }
    }
    public void initializeIfNecessary()
    {
        if (!_initialized)
        {
            if (null != slotsGrid)
            {
                slots.Clear();

                slotCount = MemoryManager.get().configuration.gameMap == GameConfiguration.GameMap.SANDBOX2 ? sandboxSlotCount : tutorialSlotCount;
                // Debug.Log("going to destroy children slots");
                for (int index = 0; index < slotsGrid.childCount; index++)
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

                displayDevice();
                
                _initialized = true;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  BioBricks
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void setBioBricks(LinkedList<BioBrick> bricks)
    {
        // Debug.Log(this.GetType() + " setBioBricks(" + Logger.ToString<BioBrick>(bricks) + ")");
        removeAllBricksFromCraftZone();

        foreach (BioBrick brick in bricks)
        {
            AvailableBioBricksManager.get().addBrickAmount(brick, -1);
        }
        _currentBioBricks.AppendRange(bricks);

        OnBioBricksChanged();
    }

    public Equipment.AddingResult equip()
    {
        Equipment.AddingResult result = Equipment.get().askAddDevice(getCurrentDevice());
        OnBioBricksChanged();
        return result;
    }

    public void unequip()
    {
        Equipment.get().removeDevice(getCurrentDevice());
        OnBioBricksChanged();
    }

    public void unequip(Device device)
    {
        // Debug.Log("LBCZM unequip");
        foreach (CraftDeviceSlot slot in slots)
        {
            // TODO check why Device.Equals fails
            if (device.hasSameBricks(slot.getCurrentDevice()))
            {
                // Debug.Log("LBCZM unequip: match on slot " + slot.name);
                slot.removeAllBricks();
                return;
            }
        }
    }

    public void OnBioBricksChanged()
    {
        foreach (CraftDeviceSlot slot in slots)
        {
            slot.updateDisplay();
        }
    }

    protected static int getIndex(BioBrick brick)
    {
        int idx;
        switch (brick.getType())
        {
            case BioBrick.Type.PROMOTER:
                idx = 0;
                break;
            case BioBrick.Type.RBS:
                idx = 1;
                break;
            case BioBrick.Type.GENE:
                idx = 2;
                break;
            case BioBrick.Type.TERMINATOR:
                idx = 3;
                break;
            default:
                idx = 0;
                Debug.LogWarning("CraftZoneManager getIndex unknown type " + brick.getType());
                break;
        }
        return idx;
    }

    protected void removePreviousDisplayedBricks()
    {
        // Debug.Log(this.GetType() + " removePreviousDisplayedBricks()");
        //remove all previous biobricks
        foreach (CraftZoneDisplayedBioBrick brick in _currentDisplayedBricks)
        {
            Destroy(brick.gameObject);
        }
        _currentDisplayedBricks.Clear();
    }

    public void removeBioBrick(CraftZoneDisplayedBioBrick brick)
    {
        // Debug.Log(this.GetType() + "removeBioBrick(czdb)");
        if (null != brick)
        {
            // Debug.Log("removeBioBrick null != brick");
            foreach (CraftDeviceSlot slot in slots)
            {
                // Debug.Log("removeBioBrick slot "+slot);
                if (null != slot && slot.removeBrick(brick))
                {
                    return;
                }
            }
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // slots

    public void addSlot()
    {
        // Debug.Log("adding slot #"+slotsGrid.childCount);
        GameObject slotGO = GameObject.Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, slotsGrid) as GameObject;
        slotGO.transform.localPosition = new Vector3(slotGO.transform.localPosition.x, slotGO.transform.localPosition.y, 0);
        slotGO.name = _slotNameRoot + slots.Count;
        CraftDeviceSlot slot = slotGO.GetComponent<CraftDeviceSlot>();
        slots.Add(slot);
        slotsGrid.GetComponent<UIGrid>().repositionNow = true;
    }

    public void selectSlot(CraftDeviceSlot slot)
    {
        // Debug.Log(this.GetType() + " selectSlot");
        if (null != slot)
        {
            if (null != selectedSlot)
            {
                selectedSlot.setSelectedBackground(false);
            }
            selectedSlot = slot;
            // Debug.Log("selectSlot selectedSlot.setSelectedBackground(true); with selectedSlot="+selectedSlot);
            selectedSlot.setSelectedBackground(true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // utilities

    protected BioBrick findFirstBioBrick(BioBrick.Type type)
    {
        foreach (BioBrick brick in _currentBioBricks)
        {
            if (brick.getType() == type) return brick;
        }
        // Debug.Log(this.GetType() + " findFirstBioBrick(" + type + ") failed with current bricks=" + Logger.ToString<BioBrick>(_currentBioBricks));
        return null;
    }

    public void replaceWithBioBrick(BioBrick brick)
    {
        // Debug.Log("replaceWithBioBrick("+brick.getName()+")");
        if (null != selectedSlot)
        {
            // Debug.Log("null != selectedSlot");
            selectedSlot.addBrick(brick);
        }
        else
        {
            // Debug.Log("null == selectedSlot");
        }
    }

    protected void insertOrdered(BioBrick toInsert)
    {
        // Debug.Log("insertOrdered("+toInsert.getName()+")");
        BioBrick sameBrick = LinkedListExtensions.Find(_currentBioBricks, b => b.getName() == toInsert.getName());

        if (null != sameBrick)
        {
            // the brick is already present on the crafting table: remove it
            removeBioBrick(toInsert);
        }
        else
        {
            bool inserted = false;

            foreach (BioBrick brick in _currentBioBricks)
            {
                if (brick.getType() > toInsert.getType())
                {
                    // the brick is inserted before the next brick
                    LinkedListNode<BioBrick> afterNode = _currentBioBricks.Find(brick);
                    _currentBioBricks.AddBefore(afterNode, toInsert);
                    inserted = true;
                    break;
                }
                else if (brick.getType() == toInsert.getType())
                {
                    // the brick will replace a brick of the same type
                    LinkedListNode<BioBrick> toReplaceNode = _currentBioBricks.Find(brick);
                    _currentBioBricks.AddAfter(toReplaceNode, toInsert);
                    _currentBioBricks.Remove(brick);

                    //the brick is put out of the crafting table and is therefore available for new crafts
                    AvailableBioBricksManager.get().addBrickAmount(brick, 1);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
            {
                // the brick is inserted in the last position
                _currentBioBricks.AddLast(toInsert);
            }

            // the brick was inserted: there's one less brick to use
            AvailableBioBricksManager.get().addBrickAmount(toInsert, -1);
        }
    }

    public void removeBioBrick(BioBrick brick)
    {
        // Debug.Log(this.GetType() + " removeBioBrick");
        string debug = null != brick ? "contains=" + _currentBioBricks.Contains(brick) : "brick==null";
        // Debug.Log(this.GetType() + " removeBioBrick with "+debug);
        if (null != brick && _currentBioBricks.Contains(brick))
        {
            // Debug.Log(this.GetType() + " removeBioBrick(" + brick.getInternalName() + ")");
            unequip();

            _currentBioBricks.Remove(brick);
            AvailableBioBricksManager.get().addBrickAmount(brick, 1);
            OnBioBricksChanged();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Device
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void addAndEquipDevice(Device device, bool replace = true)
    {
        if (Device.isValid(device))
        {
            if (!replace)
            {
                foreach (CraftDeviceSlot slot in slots)
                {
                    if (!slot.isEquiped)
                    {
                        slot.setSelected(true);
                        break;
                    }
                }
            }
            //add every brick to the stock
            foreach (BioBrick brick in device.getExpressionModules().First.Value.getBioBricks())
            {
                AvailableBioBricksManager.get().addAvailableBioBrick(brick, false);
            }
            //set device
            setDevice(device);
        }
    }

    public bool canAfford(Device device)
    {
        bool canAfford = true;
        AvailableBioBricksManager abbm = AvailableBioBricksManager.get();
        foreach (ExpressionModule module in device.getExpressionModules())
        {
            foreach (BioBrick brick in module.getBioBricks())
            {
                canAfford &= ((abbm.getBrickAmount(brick) > 0) || _currentBioBricks.Contains(brick));
            }
        }
        return canAfford;
    }

    public void setDevice(Device device)
    {
        // Debug.Log(this.GetType() + " setDevice("+device+")");
        if (null != selectedSlot)
        {
            selectedSlot.setDevice(device);
        }
    }

    protected void removeAllBricksFromCraftZone()
    {
        if (null != selectedSlot)
        {
            selectedSlot.removeAllBricks();
        }
    }

    public void craft()
    {
        // Debug.Log(this.GetType() + " craft");
    }

    protected void consumeBricks()
    {
        // Debug.Log(this.GetType() + " consumeBricks()");
        foreach (BioBrick brick in _currentBioBricks)
        {
            AvailableBioBricksManager.get().addBrickAmount(brick, -1);
        }
    }

    protected void displayDevice()
    {
        // Debug.Log(this.GetType() + " displayDevice()");
        if (null != craftFinalizer)
        {
            craftFinalizer.setDisplayedDevice(_currentDevice);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // utilities

    public Device getDeviceFromBricks(LinkedList<BioBrick> bricks)
    {
        // Debug.Log(this.GetType() + " getDeviceFromBricks(" + Logger.ToString<BioBrick>(bricks) + ")");

        if (!ExpressionModule.isBioBricksSequenceValid(bricks))
        {
            Debug.LogWarning(this.GetType() + " getDeviceFromBricks invalid biobricks sequence");
            return null;
        }

        ExpressionModule module = new ExpressionModule("test", bricks);
        LinkedList<ExpressionModule> modules = new LinkedList<ExpressionModule>();
        modules.AddLast(module);

        Device device = Device.buildDevice(modules);
        if (device != null)
        {
            // Debug.Log(this.GetType() + " getDeviceFromBricks produced " + device.getInternalName());
        }
        else
        {
            Debug.LogWarning(this.GetType() + " getDeviceFromBricks device==null with bricks=" + Logger.ToString<BioBrick>(bricks));
        }
        return device;
    }

    public Device getCurrentDevice()
    {
        // Debug.Log(this.GetType() + " getCurrentDevice");
        return selectedSlot.getCurrentDevice();
    }

    public static bool isOpenable()
    {
        //FIXME doesn't work with test null != _instance._currentDevice
        return !(0 == AvailableBioBricksManager.get().getAvailableBioBricks().Count || Hero.isBeingInjured);
    }
}

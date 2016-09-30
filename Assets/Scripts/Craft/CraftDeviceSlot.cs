using UnityEngine;
using System.Collections.Generic;

public class CraftDeviceSlot : MonoBehaviour
{
    // GUI elements
    [SerializeField]
    private UISprite _craftSlotSprite;
    private const string _slotActiveSprite = "craft_slot_active";
    private const string _slotInactiveSprite = "craft_slot_inactive";
    [SerializeField]
    private UISprite _selectionSprite;
    private const string _slotSelectedSprite = "transparent-light-grey";
    private const string _slotUnselectedSprite = "transparent-grey";
    [SerializeField]
    private CraftResultDevice _resultDevice;
    [SerializeField]
    private UISprite _resultSprite;
    private const string _resultActiveSprite = "craft_result_active";
    private const string _resultInactiveSprite = "craft_result_inactive";

    private const float _brickZ = -0.08789063f;

    [SerializeField]
    private BioBricksCollapse _bricksCollapse;

    private bool _initialized = false;

    private bool _isCollapsingBricks = false;
    private bool _isExpandingBricks = false;
    private bool _hasToCollapseBricks = false;
    private bool _hasToExpandBricks = false;
    private bool _isCollapsed = false;
    private bool _isExpanded = true;

    private bool _isSelectedSlot = false;

    private bool _interruptOnDisable = false;

    private bool _isCallingRecursively = false;

    [SerializeField]
    private GameObject[] dummyBrickGameObjects;
    [SerializeField]
    private UISprite[] dummyBrickGameObjectsSprites = new UISprite[4];

    // logical elements
    protected CraftZoneDisplayedBioBrick[] currentBricks = new CraftZoneDisplayedBioBrick[4];


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
                //Debug.Log("set: !_isEquiped && value");
                Device currentDevice = getCurrentDevice();
                _isEquiped = (Equipment.AddingResult.SUCCESS == Equipment.get().askAddDevice(currentDevice));
                if (_isEquiped)
                {
                    if (null != _resultSprite)
                    {
                        _resultSprite.spriteName = _resultActiveSprite;
                    }
                    //set result device
                    if (null != _resultDevice)
                    {
                        _resultDevice.Initialize(currentDevice);
                        _resultDevice.gameObject.SetActive(true);
                    }
                }
                forcePositionIfNecessary(false);
            }
            else if (_isEquiped && !value)
            {
                //Debug.Log("set: _isEquiped && !value");
                _isEquiped = false;
                // Debug.Log("CDS calls Equipment.get().removeDevice");
                _isCallingRecursively = true;
                Equipment.get().removeDevice(getCurrentDevice());
                _isCallingRecursively = false;
                if (null != _resultSprite)
                {
                    _resultSprite.spriteName = _resultInactiveSprite;
                }
                //set result device
                if (null != _resultDevice)
                {
                    _resultDevice.gameObject.SetActive(false);
                }
                forcePositionIfNecessary(true);
            }
            else
            {
                //Debug.Log("none");
            }
        }
    }

    protected void forcePositionIfNecessary(bool expanded)
    {
        if (!gameObject.activeInHierarchy)
        {
            if (expanded)
            {
                onBricksExpandedCallback();
            }
            else
            {
                onBricksCollapsedCallback();
            }
            setPosition(expanded);
            displayCraftSlotSprite();
        }
    }

    protected void equip()
    {
        //Debug.Log("equip");
        isEquiped = true;
        onBricksCollapsedCallback = noneCallback;
    }

    protected void unequip()
    {
        _isCraftSuccess = false;
        //Debug.Log("unequip");
        isEquiped = false;
    }

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
                Debug.LogError("unrecognized BioBrick type " + bbType);
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
            Debug.LogError("bad brick '" + brick + "'");
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
            Debug.LogError("bad brick '" + brick + "'");
        }
        return result;
    }

    // doesn't check brick amount
    public void addBrick(BioBrick brick)
    {
        // Debug.Log("addBrick("+brick.getName()+")");
        if (null != brick)
        {
            int index = getIndexFromBrick(brick);

            //Debug.Log("!isLocked && (null != brick); index="+index);

            // create new CraftZoneDisplayedBioBrick
            Vector3 dummyPosition = dummyBrickGameObjects[index].transform.localPosition;
            CraftZoneDisplayedBioBrick czdb = CraftZoneDisplayedBioBrick.Create(
                this.transform,
                Vector3.zero,
                null,
                brick
            );
            czdb.transform.localPosition = new Vector3(dummyPosition.x, dummyPosition.y, _brickZ);
            // Debug.Log("added " + brick.getInternalName() + " at z=" + czdb.transform.localPosition.z);
            czdb.slot = this;
            addBrick(czdb);
            AvailableBioBricksManager.get().addBrickAmount(brick, -1);

            // Debug.Log("added "+brick);
        }
    }

    // doesn't check brick amount
    public void addBrick(CraftZoneDisplayedBioBrick brick)
    {
        // Debug.Log("addBrick(czdb="+brick+" containing brick="+brick._biobrick+")");
        if ((null != brick) && (null != brick._biobrick))
        {
            //Debug.Log("!isLocked && (null != brick) && (null != brick._biobrick)");
            int index = getIndexFromBrick(brick);
            removeBrick(currentBricks[index]);
            currentBricks[index] = brick;
            dummyBrickGameObjectsSprites[index].enabled = false;

            checkDevice();

            updateDisplay();

            // Debug.Log("added "+brick);
        }
    }

    private bool areAllBricksNonNull()
    {
        return (
            (null != currentBricks[0])
            && (null != currentBricks[1])
            && (null != currentBricks[2])
            && (null != currentBricks[3])
        );
    }
    public delegate void Callback();
    public static void noneCallback() { }
    public Callback onBricksCollapsedCallback = noneCallback;
    public Callback onBricksExpandedCallback = noneCallback;

    private void displayCraftSlotSprite()
    {
        _craftSlotSprite.gameObject.SetActive(_isExpanded);
    }

    private void checkDevice()
    {
        if (areAllBricksNonNull())
        {
            // Debug.Log("checkDevice");
            _isCraftSuccess = (Inventory.get().canAddDevice(getCurrentDevice()) == Inventory.AddingResult.SUCCESS);
            onBricksCollapsedCallback = animateCollapseCompleted;
            askCollapseBricks();
            // Debug.Log("checkDevice done");
        }
        else
        {
            unequip();
        }
    }
    private bool _isCraftSuccess = false;
    public void animateCollapseCompleted()
    {
        // Debug.Log("animateCollapseCompleted");

        equip();

        // Debug.Log("animateCollapseCompleted _isCraftSuccess="+_isCraftSuccess);

        if (_isCraftSuccess)
        {
            // feedback on new listed device
            CraftFinalizer.get().finalizeCraft();
            // feedback on crafted device
            _resultDevice.playFeedback();

            // feedback on new listed device
            CraftFinalizer.get().finalizeCraft();

            _isCraftSuccess = false;
        }
        // Debug.Log("animateCollapseCompleted done");
    }

    public Device getCurrentDevice()
    {
        //Debug.Log("CraftDeviceSlot getCurrentDevice");
        if (areAllBricksNonNull())
        {
            List<BioBrick> currentBricksL = new List<BioBrick> { currentBricks[0]._biobrick, currentBricks[1]._biobrick, currentBricks[2]._biobrick, currentBricks[3]._biobrick };
            LinkedList<BioBrick> currentBricksLL = new LinkedList<BioBrick>(currentBricksL);
            return CraftZoneManager.get().getDeviceFromBricks(currentBricksLL);
        }
        return null;
    }

    public LinkedList<BioBrick> getCurrentBricks()
    {
        LinkedList<BioBrick> result = new LinkedList<BioBrick>();
        for (int i = 0; i < 4; i++)
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
        //Debug.Log("removeBrick(brick)");
        if (null != brick)
        {
            //Debug.Log("removeBrick(brick="+brick.getName()+")");
            foreach (CraftZoneDisplayedBioBrick czdb in currentBricks)
            {
                if ((null != czdb) && (czdb._biobrick == brick))
                {
                    innerRemoveBrick(czdb);
                    askExpandBricks();
                    return true;
                }
            }
        }
        return false;
    }

    private void askCollapseBricks()
    {
        //Debug.Log("askCollapseBricks "+getDebugBoolsString());
        if (_isCollapsed)
        {
            _isCollapsingBricks = false;
            _hasToCollapseBricks = false;
        }
        else
        {
            if (!_isCollapsingBricks && !_isExpandingBricks)
            {
                //Debug.Log("askExpandBricks validated "+getDebugBoolsString());
                collapseBricks();
            }
            else if (_isCollapsingBricks && _isExpandingBricks)
            {
                Debug.LogError("forbidden state _isCollapsingBricks && _isExpandingBricks");
                //Debug.Log("askExpandBricks denied: error "+getDebugBoolsString());
            }
            else if (_isCollapsingBricks)
            {
                // Debug.Log("askCollapseBricks _hasToCollapseBricks = false;");

                //do nothing
                _hasToCollapseBricks = false;
                //Debug.Log("askExpandBricks denied: _isCollapsingBricks "+getDebugBoolsString());
            }
            else if (_isExpandingBricks)
            {
                // Debug.Log("askCollapseBricks _hasToCollapseBricks = true;");

                _hasToCollapseBricks = true;
                //Debug.Log("askExpandBricks denied: _isExpandingBricks "+getDebugBoolsString());
            }
            else
            {
                Debug.LogError("impossible state");
                //Debug.Log("askExpandBricks denied: impossible "+getDebugBoolsString());
            }
        }
    }

    private void askExpandBricks()
    {
        //Debug.Log("askExpandBricks "+getDebugBoolsString());

        if (_isExpanded)
        {
            _isExpandingBricks = false;
            _hasToExpandBricks = false;
        }
        else
        {
            if (!_isCollapsingBricks && !_isExpandingBricks)
            {
                //Debug.Log("askExpandBricks validated "+getDebugBoolsString());
                expandBricks();
            }
            else if (_isCollapsingBricks && _isExpandingBricks)
            {
                Debug.LogError("forbidden state _isCollapsingBricks && _isExpandingBricks");
                //Debug.Log("askExpandBricks denied: error "+getDebugBoolsString());
            }
            else if (_isCollapsingBricks)
            {
                _hasToExpandBricks = true;
                //Debug.Log("askExpandBricks denied: _isCollapsingBricks "+getDebugBoolsString());
            }
            else if (_isExpandingBricks)
            {
                //do nothing
                _hasToExpandBricks = false;
                //Debug.Log("askExpandBricks denied: _isExpandingBricks "+getDebugBoolsString());
            }
            else
            {
                Debug.LogError("impossible state");
                //Debug.Log("askExpandBricks denied: impossible "+getDebugBoolsString());
            }
        }
    }

    private void collapseBricks()
    {
        // Debug.Log("collapseBricks");

        //Debug.Log("collapseBricks "+getDebugBoolsString());
        if (_isCollapsed)
        {
            // Debug.Log("collapseBricks _isCollapsed");
            _isCollapsingBricks = false;
            _hasToCollapseBricks = false;
        }
        else if (!_isExpandingBricks && areAllBricksNonNull())
        {
            // Debug.Log("collapseBricks !_isExpandingBricks && areAllBricksNonNull");

            string startString = getDebugBoolsString();
            _isExpandingBricks = false;

            _isCollapsingBricks = true;
            _hasToCollapseBricks = false;

            _isExpanded = false;
            _isCollapsed = false;

            _bricksCollapse.startCollapseBricks();

            displayCraftSlotSprite();
            //Debug.Log("collapseBricks for reals start="+startString+"\nend="+getDebugBoolsString());
        }
    }

    private void expandBricks()
    {
        //Debug.Log("expandBricks "+getDebugBoolsString());
        if (_isExpanded)
        {
            _isExpandingBricks = false;
            _hasToExpandBricks = false;
        }
        else if (!_isExpandingBricks)
        {
            string startString = getDebugBoolsString();
            _isCollapsingBricks = false;

            _isExpandingBricks = true;
            _hasToExpandBricks = false;

            _isExpanded = false;
            _isCollapsed = false;

            _bricksCollapse.startExpandBricks();

            displayCraftSlotSprite();
            //Debug.Log("expandBricks for reals start="+startString+"\nend="+getDebugBoolsString());
        }
    }

    public void onBricksStoppedMoving()
    {
        // Debug.Log("onBricksStoppedMoving");
        // string startString = getDebugBoolsString();

        _isCollapsed = _isCollapsingBricks;
        _isExpanded = _isExpandingBricks;

        _isCollapsingBricks = false;
        _isExpandingBricks = false;

        if (_isCollapsed)
        {
            onBricksCollapsedCallback();
        }
        else if (_isExpanded)
        {
            onBricksExpandedCallback();
        }

        if (_hasToCollapseBricks)
        {
            askCollapseBricks();
        }
        else if (_hasToExpandBricks)
        {
            askExpandBricks();
        }
        displayCraftSlotSprite();
        //Debug.Log("onBricksStoppedMoving start="+startString+"\nend="+getDebugBoolsString());
    }

    public bool removeBrick(CraftZoneDisplayedBioBrick brick)
    {
        if (null != brick)
        {
            //Debug.Log("removeBrick(czdb="+brick._biobrick.getName()+")");   
            foreach (CraftZoneDisplayedBioBrick czdb in currentBricks)
            {
                if (czdb == brick)
                {
                    innerRemoveBrick(brick);
                    return true;
                }
            }
            Debug.LogError("failed to find czdb brick to remove called '" + brick._biobrick.getName() + "'");
        }
        else
        {
            //Debug.Log("null == czdb");
        }
        return false;
    }

    private void innerRemoveBrick(CraftZoneDisplayedBioBrick brick)
    {
        // Debug.Log("innerRemoveBrick("+brick._biobrick.getName()+")");
        setSelected(true);
        unequip();
        AvailableBioBricksManager.get().addBrickAmount(brick._biobrick, 1);
        GameObject.Destroy(brick.gameObject);
        int index = getIndexFromType(brick._biobrick.getType());
        currentBricks[index] = null;
        dummyBrickGameObjectsSprites[index].enabled = true;
        expandBricks();
    }

    public void removeAllBricks()
    {
        if (!_isCallingRecursively)
        {
            removeBrick(currentBricks[0]);
            removeBrick(currentBricks[1]);
            removeBrick(currentBricks[2]);
            removeBrick(currentBricks[3]);
            if (isEquiped)
            {
                unequip();
            }
        }
    }
    
    private bool canAfford(Device device)
    {
        lazyInitialize();
        if (device != null)
        {
            AvailableBioBricksManager abbm = AvailableBioBricksManager.get();
            LinkedList<BioBrick> currentBricks = getCurrentBricks();
            
            foreach (BioBrick brick in device.getBioBricks())
            {
                if((abbm.getBrickAmount(brick) == 0) && !currentBricks.Contains(brick))
                {
                    // Debug.Log("can't afford " + brick.getInternalName());
                    // Debug.Log("brick = " + brick.getInternalName() + "\nbricks = " + Logger.ToString<BioBrick>(currentBricks, b => b.getInternalName()));
                    // Debug.Log("brick = " + brick + "\nbricks = " + Logger.ToString<BioBrick>(currentBricks));
                    return false;
                }
            }
            return true;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " device was null");
            return false;
        }
    }

    public void setDevice(Device device)
    {
        if (device != null)
        {
            if(canAfford(device))
            {
                //Debug.Log("CDS setDevice removeAllBricks");
                removeAllBricks();

                foreach (BioBrick brick in device.getBioBricks())
                {
                    addBrick(brick);
                }

                updateDisplay();
            }
        }
    }

    public void setSelected(bool selected)
    {
        //Debug.Log("CraftDeviceSlot selectSlot("+selected+")");
        CraftZoneManager czm = CraftZoneManager.get();
        if (null != czm)
        {
            czm.selectSlot(this);
        }
    }

    public void setSelectedBackground(bool selected)
    {
        //Debug.Log("CraftDeviceSlot setSelectedBackground("+selected+")");
        _isSelectedSlot = selected;
        _selectionSprite.spriteName = selected ? _slotSelectedSprite : _slotUnselectedSprite;
        _selectionSprite.type = UISprite.Type.Sliced;
    }

    public void updateDisplay()
    {
        for (int index = 0; index < 4; index++)
        {
            if (null != currentBricks[index])
            {
                currentBricks[index].gameObject.SetActive(true);
                dummyBrickGameObjectsSprites[index].enabled = false;
            }
            else
            {
                dummyBrickGameObjectsSprites[index].enabled = true;
            }
        }
    }

    void lazyInitialize()
    {
        if (!_initialized)
        {
            // Debug.Log("not yet initialized");
            _initialized = true;

            _selectionSprite.name = gameObject.name + _selectionSprite.name;

            _isEquiped = true;
            isEquiped = false;

            for (int index = 0; index < 4; index++)
            {
                dummyBrickGameObjectsSprites[index] = dummyBrickGameObjects[index].GetComponent<UISprite>();
            }

            _bricksCollapse.onBricksStoppedMoving = onBricksStoppedMoving;
        }
        // else
        // {
        //     Debug.Log("already initialized");
        // }
    }

    void Awake()
    {
        lazyInitialize();
    }

    public CraftZoneDisplayedBioBrick[] getCraftZoneDisplayedBioBricks()
    {
        return currentBricks;
    }

    /*
        void Update()
        {
            if (_isSelectedSlot && Input.GetKeyUp(KeyCode.KeypadMinus))
            {
                Debug.LogError(getDebugBoolsString());
                _interruptOnDisable = !_interruptOnDisable;
            }        
        }
        */

    void OnDisable()
    {
        if (!_interruptOnDisable)
        {
            // finish craft & equip if needed
            if (_isCollapsingBricks || _hasToCollapseBricks)
            {
                onBricksCollapsedCallback();
                setPosition(false);
            }
            else if (_isExpandingBricks || _hasToExpandBricks)
            {
                onBricksExpandedCallback();
                setPosition(true);
            }
        }
        else
        {
            // interrupt and abort equip & craft
            if (_isCollapsingBricks || _hasToCollapseBricks)
            {
                removeAllBricks();
            }
            setPosition(!_isCollapsed);
        }
        displayCraftSlotSprite();
    }

    private void setPosition(bool isExpanded)
    {
        _isCollapsingBricks = false;
        _isExpandingBricks = false;
        _hasToCollapseBricks = false;
        _hasToExpandBricks = false;
        _isCollapsed = !isExpanded;
        _isExpanded = isExpanded;

        _bricksCollapse.setPosition(isExpanded);
    }

    private string getDebugBoolsString()
    {
        return "bools:\n_isCollapsingBricks = " + _isCollapsingBricks + ";" +
                "\n_isExpandingBricks = " + _isExpandingBricks + ";" +
                "\n_hasToCollapseBricks = " + _hasToCollapseBricks + ";" +
                "\n_hasToExpandBricks = " + _hasToExpandBricks + ";" +
                "\n_isCollapsed = " + _isCollapsed + ";" +
                "\n_isExpanded = " + _isExpanded
                ;
    }
}
using UnityEngine;

public class CraftDeviceSlot : MonoBehaviour
{
    // GUI elements
    protected ActivationButton activationButton;
    protected UISprite craftSlotSprite;
    // TODO remove test sprites, put real sprites
    const string slotActive = "arrow";
    const string slotInactive = "arrow-bottom-white-bg";

    // logical elements
    protected CraftZoneDisplayedBioBrick[] currentBricks = new CraftZoneDisplayedBioBrick[4];
    
    public GameObject[] dummyBrickGameObjects;
    
    protected Device _resultDevice;
    // isLocked == active: active == true when the device is working and protected from edition
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

    public void addBrick(BioBrick brick)
    {
        Debug.LogError("addBrick("+brick.getName()+")");
        if (!isLocked && (null != brick))
        {
            int index = getIndexFromBrick(brick);
            
            Debug.LogError("!isLocked && (null != brick); index="+index);

            // create new CraftZoneDisplayedBioBrick 
            CraftZoneDisplayedBioBrick czdb = CraftZoneDisplayedBioBrick.Create(
                this.transform,
                dummyBrickGameObjects[index].transform.localPosition,
                null,
                brick
            );

            addBrick(czdb);
            AvailableBioBricksManager.get().addBrickAmount(brick, -1);
        }
    }

    public void addBrick(CraftZoneDisplayedBioBrick brick)
    {
        Debug.LogError("addBrick(czdb)");
        if (!isLocked && (null != brick) && (null != brick._biobrick))
        {
            Debug.LogError("!isLocked && (null != brick) && (null != brick._biobrick)");
            int index = getIndexFromBrick(brick);
            removeBrick(currentBricks[index]);
            currentBricks[index] = brick;
            updateDisplay();
        }
    }

    public bool removeBrick(BioBrick brick)
    {
        Debug.LogError("removeBrick(brick)");
        if (null != brick)
        {
            Debug.LogError("removeBrick(brick="+brick.getName()+")");
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
        Debug.LogError("removeBrick(czdb)");
        if(null != brick)
        {
            Debug.LogError("removeBrick(czdb="+brick._biobrick.getName()+")");   
            foreach (CraftZoneDisplayedBioBrick czdb in currentBricks)
            {
                if(czdb == brick)
                {
                    innerRemoveBrick(brick);
                    return true;
                }
            }
            Debug.LogError("failed to find czdb brick to remove called '"+brick._biobrick.getName()+"'");
        }
        return false;
    }

    private void innerRemoveBrick(CraftZoneDisplayedBioBrick brick)
    {
        Debug.LogError("innerRemoveBrick("+brick._biobrick.getName()+")");
        isLocked = false;
        AvailableBioBricksManager.get().addBrickAmount(brick._biobrick, 1);
        GameObject.Destroy(brick.gameObject);
        currentBricks[getIndexFromType(brick._biobrick.getType())] = null;
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
        foreach(GameObject go in dummyBrickGameObjects)
        {
            go.SetActive(false);
        }
    }
    
    void Awake()
    {
        initialize();
    }
    
    void OnEnable()
    {
        initialize();
    }
}
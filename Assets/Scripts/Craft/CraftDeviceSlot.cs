using UnityEngine;

public class CraftDeviceSlot : MonoBehaviour
{
    // GUI elements
    protected ActivationButton activationButton;
    protected UISprite craftSlotSprite;
    // TODO remove test sprites, put real sprites
    const string slotActive = "arrow";
    const string slotInactive = "arrow-bottom-white-bg";
    protected CraftZoneDisplayedBioBrick[] currentBricks = new CraftZoneDisplayedBioBrick[4];

    // logical elements
    public GameObject[] brickGameObjects;
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
        if (!isLocked && (null != brick))
        {
            int index = getIndexFromBrick(brick);

            // create new CraftZoneDisplayedBioBrick 
            CraftZoneDisplayedBioBrick czdb = CraftZoneDisplayedBioBrick.Create(
                this.transform,
                brickGameObjects[index].transform.localPosition,
                null,
                brick
            );

            addBrick(czdb);
            AvailableBioBricksManager.get().addBrickAmount(brick, -1);
        }
    }

    public void addBrick(CraftZoneDisplayedBioBrick brick)
    {
        if (!isLocked && (null != brick) && (null != brick._biobrick))
        {
            int index = getIndexFromBrick(brick);
            removeBrick(currentBricks[index]);
            currentBricks[index] = brick;
            updateDisplay();
        }
    }

    public void removeBrick(BioBrick brick)
    {
        if (null != brick)
        {
            foreach (CraftZoneDisplayedBioBrick czdb in currentBricks)
            {
                if ((null != czdb) && (czdb._biobrick == brick))
                {
                    isLocked = false;
                    AvailableBioBricksManager.get().addBrickAmount(brick, 1);
                    removeBrick(czdb);
                    return;
                }
            }
        }
    }

    public void removeBrick(CraftZoneDisplayedBioBrick brick)
    {
        if (null != brick)
        {
            GameObject.Destroy(brick.gameObject);
            currentBricks[getIndexFromType(brick._biobrick.getType())] = null;
        }
    }

    public void updateDisplay()
    {
        for (int index = 0; index < 4; index++)
        {
            if (null != currentBricks[index] && (4 == brickGameObjects.Length) && (null != brickGameObjects[index]))
            {
                currentBricks[index].gameObject.transform.localPosition = brickGameObjects[index].transform.localPosition;
            }
        }
    }
}
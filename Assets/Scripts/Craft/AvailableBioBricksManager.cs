using UnityEngine;
using System.Collections.Generic;

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class AvailableBioBricksManager : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "BioBrickInventory";
    private static AvailableBioBricksManager _instance;
    public static AvailableBioBricksManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("AvailableBioBricksManager get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<AvailableBioBricksManager>();
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
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            _initialized = true;
            initialize();
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    private BiobrickListData _allBioBricksData;

    private CheckPointData _availableBioBricksData;

    public CheckPointData availableBioBrickData
    {
        set
        {
            // Debug.Log(this.GetType() + " availableBioBrickData set value=" + value);
            _availableBioBricksData = value;
            _availableBioBricks.Clear();
            loadAvailableBioBricks();
        }
    }

    public void logAvailableBioBrickData()
    {
        string debugString = this.GetType() + " logAvailableBioBrickData bricks=[";
        debugString += Logger.ToString<BioBrick>(_availableBioBricks);
        debugString += "]";
        Logger.Log(debugString);
    }

    // the panel on which the BioBricks will be drawn
    public GameObject bioBricksPanel;

    //prefab for available biobricks
    public Transform promoterBrickCategoryGrid, rbsBrickCategoryGrid, geneBrickCategoryGrid, terminatorBrickCategoryGrid;

    private List<GameObject> dummies = new List<GameObject>();
    private List<Transform> dummyGrids = new List<Transform>();

    //visual, clickable biobricks currently displayed
    //only used in unlimited bricks mode
    LinkedList<AvailableDisplayedBioBrick> _displayedBioBricks = new LinkedList<AvailableDisplayedBioBrick>();

    //biobrick data catalog
    private LinkedList<BioBrick> _allBioBricks = new LinkedList<BioBrick>();
    private LinkedList<BioBrick> _availableBioBricks = new LinkedList<BioBrick>();

    //visual, clickable biobrick catalog
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailablePromoters = new LinkedList<AvailableDisplayedBioBrick>();
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailableRBS = new LinkedList<AvailableDisplayedBioBrick>();
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailableGenes = new LinkedList<AvailableDisplayedBioBrick>();
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailableTerminators = new LinkedList<AvailableDisplayedBioBrick>();

    public void initialize()
    {
        // Debug.Log(this.GetType() + " initialize()");
        _allBioBricks.Clear();
        loadAllBioBricksIfNecessary();
        _availableBioBricks.Clear();

        destroyAllAvailableDisplayedBioBricks();

        initializeDummies();
    }

    void destroyAllAvailableDisplayedBioBricks(LinkedList<AvailableDisplayedBioBrick> bricks)
    {
        foreach (AvailableDisplayedBioBrick brick in bricks)
        {
            //Debug.LogError("destroying "+brick.gameObject.name);
            Destroy(brick.gameObject);
        }
        bricks.Clear();
    }

    void destroyAllAvailableDisplayedBioBricks()
    {
        //Debug.LogError("destroyAllAvailableDisplayedBioBricks");
        destroyAllAvailableDisplayedBioBricks(_displayableAvailablePromoters);
        destroyAllAvailableDisplayedBioBricks(_displayableAvailableRBS);
        destroyAllAvailableDisplayedBioBricks(_displayableAvailableGenes);
        destroyAllAvailableDisplayedBioBricks(_displayableAvailableTerminators);
    }

    void initializeDummies()
    {
        // Debug.Log(this.GetType() + " initializeDummies");

        dummyGrids.Clear();
        dummyGrids.AddRange(new List<Transform>{
            promoterBrickCategoryGrid,
            rbsBrickCategoryGrid,
            geneBrickCategoryGrid,
            terminatorBrickCategoryGrid
            });

        foreach (Transform grid in dummyGrids)
        {
            if (null != grid)
            {
                // Debug.Log(this.GetType() + " initializeDummies grid="+grid.name);
                for (int index = 0; index < grid.childCount; index++)
                {
                    Destroy(grid.GetChild(index).gameObject);
                }
            }
        }
    }

    public double getBrickAmount(BioBrick brick)
    {
        BioBrick currentBrick = LinkedListExtensions.Find<BioBrick>(
                _availableBioBricks
                , b => b.getName() == brick.getName()
                , false
                , this.GetType() + " getBrickAmount(" + brick + ")"
                );

        if (null != currentBrick)
        {
            return currentBrick.amount;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " brick " + brick.getInternalName() + "not found");
            return 0;
        }
    }

    public void addBrickAmount(BioBrick brick, double amount)
    {
        // Debug.Log(this.GetType() + " addBrickAmount("+brick+","+amount+")");
        BioBrick currentBrick = LinkedListExtensions.Find<BioBrick>(
                _availableBioBricks
                , b => b.getName() == brick.getName()
                , false
                , this.GetType() + " addBrickAmount(" + brick + ", " + amount + ")"
                );

        if (null != currentBrick)
        {
            currentBrick.addAmount(amount);
        }
    }

    private void updateDisplayedBioBricks()
    {
        // Debug.Log(this.GetType() + " updateDisplayedBioBricks");
        destroyAllAvailableDisplayedBioBricks();

        LinkedList<BioBrick> availablePromoters = new LinkedList<BioBrick>();
        LinkedListExtensions.AppendRange<BioBrick>(availablePromoters, getAvailableBioBricksOfType(BioBrick.Type.PROMOTER));
        LinkedList<BioBrick> availableRBS = new LinkedList<BioBrick>();
        LinkedListExtensions.AppendRange<BioBrick>(availableRBS, getAvailableBioBricksOfType(BioBrick.Type.RBS));
        LinkedList<BioBrick> availableGenes = new LinkedList<BioBrick>();
        LinkedListExtensions.AppendRange<BioBrick>(availableGenes, getAvailableBioBricksOfType(BioBrick.Type.GENE));
        LinkedList<BioBrick> availableTerminators = new LinkedList<BioBrick>();
        LinkedListExtensions.AppendRange<BioBrick>(availableTerminators, getAvailableBioBricksOfType(BioBrick.Type.TERMINATOR));

        _displayableAvailablePromoters = getDisplayableAvailableBioBricks(
          availablePromoters
          , getDisplayableAvailableBioBrick
          , promoterBrickCategoryGrid
          );

        _displayableAvailableRBS = getDisplayableAvailableBioBricks(
          availableRBS
          , getDisplayableAvailableBioBrick
          , rbsBrickCategoryGrid
          );
        _displayableAvailableGenes = getDisplayableAvailableBioBricks(
          availableGenes
          , getDisplayableAvailableBioBrick
          , geneBrickCategoryGrid
          );
        _displayableAvailableTerminators = getDisplayableAvailableBioBricks(
          availableTerminators
          , getDisplayableAvailableBioBrick
          , terminatorBrickCategoryGrid
          );

        // Debug.Log(this.GetType() + " updateDisplayedBioBricks"
        //   + "\n\navailablePromoters=" + Logger.ToString<BioBrick>(availablePromoters)
        //   + ",\n\navailableRBS=" + Logger.ToString<BioBrick>(availableRBS)
        //   + ",\n\navailableGenes=" + Logger.ToString<BioBrick>(availableGenes)
        //   + ",\n\navailableTerminators=" + Logger.ToString<BioBrick>(availableTerminators)

        //   + ",\n\n_displayableAvailablePromoters=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailablePromoters)
        //   + ",\n\n_displayableAvailableRBS=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableRBS)
        //   + ",\n\n_displayableAvailableGenes=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableGenes)
        //   + ",\n\n_displayableAvailableTerminators=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableTerminators)
        // );

        displayAll();

    }

    private LinkedList<BioBrick> getAvailableBioBricksOfType(BioBrick.Type type)
    {
        // Debug.Log(this.GetType() + " getAvailableBioBricksOfType(" + type + ")");
        return LinkedListExtensions.Filter<BioBrick>(_availableBioBricks, b => ((b.getType() == type)));// && (b.amount > 0)));
    }

    public bool addBioBrickToList(BioBrick brick, LinkedList<BioBrick> destination, bool updateView = true)
    {
        // Debug.Log(this.GetType() + " addAvailableBioBrick(" + brick + ")");

        if (null != brick)
        // TODO deeper safety check
        // && !LinkedListExtensions.Find<BioBrick>(_availableBioBricks, b => b..Equals(brick), true, this.GetType() + " addAvailableBioBrick("+brick+", "+updateView+")")
        {
            string bbName = brick.getName();

            // Debug.Log(this.GetType() + " addAvailableBioBrick(" + brick + ") _availableBioBricks with bbName=" + bbName);

            BioBrick currentBrick = LinkedListExtensions.Find<BioBrick>(
                destination
                , b => b.getName() == bbName
                , false
                , this.GetType() + " addAvailableBioBrick(" + brick + ", " + updateView + ")"
            );

            if (null == currentBrick)
            {
                // Debug.Log(this.GetType() + " addAvailableBioBrick null == currentBrick");
                destination.AddLast(brick);
                if (updateView)
                {
                    updateDisplayedBioBricks();
                }
                return true;
            }
            else
            {
                // Debug.Log("before: amount= "+currentBrick.amount);
                destination.Find(currentBrick).Value.addAmount(brick.amount);
                // Debug.Log("after: amount= "+
                return true;
            }
        }
        else
        {
            Debug.LogWarning(this.GetType() + " addAvailableBioBrick() failed: brick == null");
            return false;
        }
    }

    public bool addAvailableBioBrick(BioBrick brick, bool updateView = true)
    {
        return addBioBrickToList(brick, _availableBioBricks, updateView);
    }

    public void OnPanelEnabled()
    {
        // Debug.Log(this.GetType() + " OnEnable");
        updateDisplayedBioBricks();
    }

    private delegate AvailableDisplayedBioBrick DisplayableAvailableBioBrickCreator(BioBrick brick, int index, Transform parentTransform);

    private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks(
      LinkedList<BioBrick> bioBricks
      , DisplayableAvailableBioBrickCreator creator
      , Transform parentTransform
    )
    {
        LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
        foreach (BioBrick brick in bioBricks)
        {
            AvailableDisplayedBioBrick availableBrick = creator(brick, result.Count, parentTransform);
            availableBrick.display(false);
            result.AddLast(availableBrick);
        }
        return result;
    }

    private AvailableDisplayedBioBrick getDisplayableAvailableBioBrick(BioBrick brick, int index, Transform parentTransform)
    {

        Transform parentTransformParam = (null == parentTransform) ? bioBricksPanel.transform : parentTransform;
        Vector3 localPositionParam = Vector3.zero;

        // Debug.Log(this.GetType() + " getDisplayableAvailableBioBrick(brick=" + brick + ", index=" + index + "),"
        //   + ", parentTransformParam=" + parentTransformParam
        //   + ", localPositionParam=" + localPositionParam
        //   + ", brick=" + brick
        //   );

        AvailableDisplayedBioBrick resultBrick = AvailableDisplayedBioBrick.Create(
          parentTransformParam
          , localPositionParam
          , brick
        );

        return resultBrick;
    }

    public void displayAll()
    {
        display(_displayableAvailablePromoters, true);
        display(_displayableAvailableRBS, true);
        display(_displayableAvailableGenes, true);
        display(_displayableAvailableTerminators, true);

        if (
            (null != promoterBrickCategoryGrid)
            && (null != rbsBrickCategoryGrid)
            && (null != geneBrickCategoryGrid)
            && (null != terminatorBrickCategoryGrid)
        )
        {
            promoterBrickCategoryGrid.GetComponent<UIGrid>().repositionNow = true;
            rbsBrickCategoryGrid.GetComponent<UIGrid>().repositionNow = true;
            geneBrickCategoryGrid.GetComponent<UIGrid>().repositionNow = true;
            terminatorBrickCategoryGrid.GetComponent<UIGrid>().repositionNow = true;
        }
    }
    public void displayPromoters()
    {
        // Debug.Log(this.GetType() + " displayPromoters");
        switchTo(_displayableAvailablePromoters);
    }
    public void displayRBS()
    {
        // Debug.Log(this.GetType() + " displayRBS");
        switchTo(_displayableAvailableRBS);
    }
    public void displayGenes()
    {
        // Debug.Log(this.GetType() + " displayGenes");
        switchTo(_displayableAvailableGenes);
    }
    public void displayTerminators()
    {
        // Debug.Log(this.GetType() + " displayTerminators");
        switchTo(_displayableAvailableTerminators);
    }
    private void switchTo(LinkedList<AvailableDisplayedBioBrick> list)
    {
        string listToString = "list=[";
        foreach (AvailableDisplayedBioBrick brick in list)
        {
            listToString += brick.ToString() + ", ";
        }
        listToString += "]";
        // Debug.Log(this.GetType() + " switchTo(" + listToString + ")");
        display(_displayedBioBricks, false);
        _displayedBioBricks.Clear();
        _displayedBioBricks.AppendRange(list);
        display(_displayedBioBricks, true);
    }

    private void display(LinkedList<AvailableDisplayedBioBrick> bricks, bool enabled)
    {
        foreach (AvailableDisplayedBioBrick brick in bricks)
        {
            brick.display(enabled);
        }
    }

    public LinkedList<BioBrick> getAllBioBricks()
    {
        // Debug.Log(this.GetType() + " getAllBioBricks");

        loadAllBioBricksIfNecessary();

        return _allBioBricks;
    }

    private void loadAllBioBricksIfNecessary()
    {
        if (_allBioBricks == null || _allBioBricks.Count == 0)
        {
            _instance.loadAllBioBricks();
        }
    }

    public BioBrick getBioBrickFromAll(string brickName)
    {
        // Debug.Log(this.GetType() + " getBioBrickFromAll");

        BioBrick brick = LinkedListExtensions.Find<BioBrick>(
            getAllBioBricks()
            , b => (b.getName() == brickName)
            , false
            , this.GetType() + " getBioBrickFromAll(" + brickName + ")"
            );
        if (brick != null)
        {
            // Debug.Log(this.GetType() + " getBioBrickFromAll found " + brick);
            return brick;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " getBioBrickFromAll failed to find brick with name " + brickName + "!");
            return null;
        }
    }

    private void loadAllBioBricks()
    {
        // Debug.Log(this.GetType() + " loadAllBioBricks");
        loadBioBricks(_allBioBricksData.biobrickDataList, _allBioBricks);
        // Debug.Log(this.GetType() + " loadAllBioBricks _allBioBricks=" + _allBioBricks.Count);
    }

    // Warning: inputFiles is an array of names of files inside 'biobrickFilesPathPrefix'
    private void loadBioBricks(BiobrickDataCount[] biobricksData, LinkedList<BioBrick> destination)
    {
        foreach (var biobrickDataCount in biobricksData)
        {
            addBioBrickToList(
                BiobrickBuilder.createBioBrickFromData(biobrickDataCount),
                destination
            );
        }
    }


    public LinkedList<BioBrick> getAvailableBioBricks()
    {
        string availableBB = null == _availableBioBricks ? "null" : _availableBioBricks.Count.ToString();
        // Debug.Log(this.GetType() + " getAvailableBioBricks with initial _availableBioBricks=" + availableBB);
        if (_availableBioBricks == null || _availableBioBricks.Count == 0)
        {
            loadAvailableBioBricks();
        }
        // Debug.Log(this.GetType() + " getAvailableBioBricks returns " + _availableBioBricks.Count + " elements");
        return _availableBioBricks;
    }

    private void loadCheckPointBrickList(CheckPointData checkPoint, LinkedList<BioBrick> destination)
    {
        if (checkPoint == null)
            return;
        loadBioBricks(checkPoint.biobrickDataList, destination);
        // just a small check to prevent an infinite loop
        if (checkPoint.previous != null && checkPoint.previous != checkPoint)
        {
            loadCheckPointBrickList(checkPoint.previous, destination);
        }
    }


    private void loadAvailableBioBricks()
    {
        // Debug.Log(this.GetType() + " loadAvailableBioBricks");
        LevelInfo levelInfo = null;
        MemoryManager.get().tryGetCurrentLevelInfo(out levelInfo);
        if (null != levelInfo && levelInfo.areAllBioBricksAvailable)
        {
            // load all biobricks
            // loadBioBricks(_allBioBrickFiles, _availableBioBricks);
            // or just copy them
            foreach (BioBrick bb in getAllBioBricks())
            {
                _availableBioBricks.AddLast(bb);
            }
        }
        else
        {
            loadCheckPointBrickList(_availableBioBricksData, _availableBioBricks);
        }
        // Debug.Log(this.GetType() + " loadAvailableBioBricks _availableBioBricks=" + _availableBioBricks.Count);
    }
}

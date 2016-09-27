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
            Logger.Log("AvailableBioBricksManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<AvailableBioBricksManager>();
        }
        return _instance;
    }
    
    void Awake()
    {
        Debug.Log(this.GetType() + " Awake");
        if((_instance != null) && (_instance != this))
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
        Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;  
    private void initializeIfNecessary()
    {
        if(!_initialized)
        {
            _initialized = true;
            initialize();
        }
    }

    void Start()
    {
        Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////


    public string[] _allBioBrickFiles;

    //Parameters/BioBricks/availablebiobricks
    public string[] _availableBioBrickFiles;
    public string biobrickFilesPathPrefix;

    //width of a displayed BioBrick
    //set in Unity editor
    public int _width;
    public int _bricksPerRow;

    // the panel on which the BioBricks will be drawn
    public GameObject bioBricksPanel;

    //prefab for available biobricks
    public GameObject availableBioBrick;
    public Transform promoterBrickCategoryGrid, rbsBrickCategoryGrid, geneBrickCategoryGrid, terminatorBrickCategoryGrid;

    private List<GameObject> dummies = new List<GameObject>();
    private List<Transform> dummyGrids = new List<Transform>();

    //visual, clickable biobricks currently displayed
    //only used in unlimited bricks mode
    LinkedList<AvailableDisplayedBioBrick> _displayedBioBricks = new LinkedList<AvailableDisplayedBioBrick>();

    //biobrick data catalog
    /*
     * TODO use for optimization
    private static LinkedList<PromoterBrick>        _availablePromoters   = new LinkedList<PromoterBrick>();
    private static LinkedList<RBSBrick>             _availableRBS         = new LinkedList<RBSBrick>();
    private static LinkedList<GeneBrick>            _availableGenes       = new LinkedList<GeneBrick>();
    private static LinkedList<TerminatorBrick>      _availableTerminators = new LinkedList<TerminatorBrick>();
    */
    private static LinkedList<BioBrick> _allBioBricks = new LinkedList<BioBrick>();
    private static LinkedList<BioBrick> _availableBioBricks = new LinkedList<BioBrick>();

    //visual, clickable biobrick catalog
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailablePromoters = new LinkedList<AvailableDisplayedBioBrick>();
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailableRBS = new LinkedList<AvailableDisplayedBioBrick>();
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailableGenes = new LinkedList<AvailableDisplayedBioBrick>();
    LinkedList<AvailableDisplayedBioBrick> _displayableAvailableTerminators = new LinkedList<AvailableDisplayedBioBrick>();

    public void initialize()
    {
        Logger.Log("AvailableBioBricksManager::initialize()", Logger.Level.INFO);
        _allBioBricks.Clear();
        loadAllBioBricksIfNecessary();
        _availableBioBricks.Clear();

        destroyAllAvailableDisplayedBioBricks();
        
        initializeDummies();
    }
    
    void destroyAllAvailableDisplayedBioBricks(LinkedList<AvailableDisplayedBioBrick> bricks)
    {
        foreach(AvailableDisplayedBioBrick brick in bricks)
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
        //Debug.LogError("initializeDummies");
        dummies.Clear();
        dummies.AddRange(new List<GameObject>{
            availableBioBrick
            });
        
        foreach(GameObject dummy in dummies)
        {
            if(null != dummy)
            {
                //Debug.LogError("initializeDummies dummy="+dummy.name);
                dummy.SetActive(false);
            }
        }

        dummyGrids.Clear();
        dummyGrids.AddRange(new List<Transform>{
            promoterBrickCategoryGrid,
            rbsBrickCategoryGrid,
            geneBrickCategoryGrid,
            terminatorBrickCategoryGrid
            });
        
        foreach(Transform grid in dummyGrids)
        {
            if(null != grid)
            {
                // Debug.Log("initializeDummies grid="+grid.name);
                for(int index = 0; index < grid.childCount; index++)
                {
                    Destroy(grid.GetChild(index).gameObject);
                }
            }
        }
    }
    
    public void addBrickAmount(BioBrick brick, double amount)
    {
        // Debug.Log("addBrickAmount("+brick+","+amount+")");
        BioBrick currentBrick = LinkedListExtensions.Find<BioBrick>(
                _availableBioBricks
                , b => b.getName() == brick.getName()
                , false
                , " AvailableBioBricksManager::addBrickAmount(" + brick + ", " + amount + ")"
                );
                
                if(null != currentBrick)
                {
                    currentBrick.addAmount(amount);
                }
    }

    private void updateDisplayedBioBricks()
    {
        Logger.Log("AvailableBioBricksManager::updateDisplayedBioBricks", Logger.Level.DEBUG);


        //Debug.LogError("updateDisplayedBioBricks");
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

        Logger.Log("AvailableBioBricksManager::updateDisplayedBioBricks"
          + "\n\navailablePromoters=" + Logger.ToString<BioBrick>(availablePromoters)
          + ",\n\navailableRBS=" + Logger.ToString<BioBrick>(availableRBS)
          + ",\n\navailableGenes=" + Logger.ToString<BioBrick>(availableGenes)
          + ",\n\navailableTerminators=" + Logger.ToString<BioBrick>(availableTerminators)

          + ",\n\n_displayableAvailablePromoters=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailablePromoters)
          + ",\n\n_displayableAvailableRBS=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableRBS)
          + ",\n\n_displayableAvailableGenes=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableGenes)
          + ",\n\n_displayableAvailableTerminators=" + Logger.ToString<AvailableDisplayedBioBrick>(_displayableAvailableTerminators)
          , Logger.Level.TRACE);

        displayAll();

    }

    private LinkedList<BioBrick> getAvailableBioBricksOfType(BioBrick.Type type)
    {
        Logger.Log("AvailableBioBricksManager::getAvailableBioBricksOfType(" + type + ")", Logger.Level.TRACE);
        return LinkedListExtensions.Filter<BioBrick>(_availableBioBricks, b => ((b.getType() == type)));// && (b.amount > 0)));
    }

    public bool addAvailableBioBrick(BioBrick brick, bool updateView = true)
    {
        Logger.Log("AvailableBioBricksManager::addAvailableBioBrick(" + brick + ")", Logger.Level.INFO);
        string bbName = brick.getName();
        if ((null != brick))
        // TODO deeper safety check
        // && !LinkedListExtensions.Find<BioBrick>(_availableBioBricks, b => b..Equals(brick), true, " AvailableBioBricksManager::addAvailableBioBrick("+brick+", "+updateView+")")
        {
            Logger.Log("AvailableBioBricksManager::addAvailableBioBrick(" + brick + ") will _availableBioBricks.AddLast(" + brick + ")", Logger.Level.INFO);

            BioBrick currentBrick = LinkedListExtensions.Find<BioBrick>(
                _availableBioBricks
                , b => b.getName() == bbName
                , false
                , " AvailableBioBricksManager::addAvailableBioBrick(" + brick + ", " + updateView + ")"
                );

            if (null == currentBrick)
            {
                _availableBioBricks.AddLast(brick);
                if (updateView)
                {
                    updateDisplayedBioBricks();
                }
                return true;                
            } else {
                //TODO fix this, maybe use addBrickAmount?
                brick.addAmount(currentBrick.amount);
                //currentBrick.addAmount(brick.amount);
                _availableBioBricks.Remove(currentBrick);
                _availableBioBricks.AddLast(brick);
                return true;
            }
        }
        else
        {
            Logger.Log("AvailableBioBricksManager::addAvailableBioBrick(" + brick + ") fail", Logger.Level.INFO);
            return false;
        }
    }

    public void OnPanelEnabled()
    {
        Logger.Log("AvailableBioBricksManager::OnEnable", Logger.Level.DEBUG);
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

        Transform parentTransformParam = (null==parentTransform)?bioBricksPanel.transform:parentTransform;
        Vector3 localPositionParam = Vector3.zero;
                
        string spriteNameParam = AvailableDisplayedBioBrick.getSpriteName(brick);
        BioBrick biobrickParam = brick;

        Logger.Log("AvailableBioBricksManager::getDisplayableAvailableBioBrick(brick=" + brick + ", index=" + index + "),"
          + ", parentTransformParam=" + parentTransformParam
          + ", localPositionParam=" + localPositionParam
          + " (width=" + _width + ")"
          + ", spriteNameParam=" + spriteNameParam
          + ", biobrickParam=" + biobrickParam
          , Logger.Level.TRACE
          );

        AvailableDisplayedBioBrick resultBrick = AvailableDisplayedBioBrick.Create(
          parentTransformParam
          , localPositionParam
          , spriteNameParam
          , biobrickParam
        );
        
        return resultBrick;
    }

    public void displayAll()
    {
        display(_displayableAvailablePromoters, true);
        display(_displayableAvailableRBS, true);
        display(_displayableAvailableGenes, true);
        display(_displayableAvailableTerminators, true);

        if(
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
        Logger.Log("AvailableBioBricksManager::displayPromoters", Logger.Level.TRACE);
        switchTo(_displayableAvailablePromoters);
    }
    public void displayRBS()
    {
        Logger.Log("AvailableBioBricksManager::displayRBS", Logger.Level.TRACE);
        switchTo(_displayableAvailableRBS);
    }
    public void displayGenes()
    {
        Logger.Log("AvailableBioBricksManager::displayGenes", Logger.Level.TRACE);
        switchTo(_displayableAvailableGenes);
    }
    public void displayTerminators()
    {
        Logger.Log("AvailableBioBricksManager::displayTerminators", Logger.Level.TRACE);
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
        Logger.Log("AvailableBioBricksManager::switchTo(" + listToString + ")", Logger.Level.TRACE);
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
        Logger.Log("AvailableBioBricksManager::getAllBioBricks", Logger.Level.DEBUG);

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
        Logger.Log("AvailableBioBricksManager::getBioBrickFromAll", Logger.Level.DEBUG);

        BioBrick brick = LinkedListExtensions.Find<BioBrick>(
            getAllBioBricks()
            , b => (b.getName() == brickName)
            , false
            , "AvailableBioBricksManager::getBioBrickFromAll(" + brickName + ")"
            );
        if (brick != null)
        {
            Logger.Log("AvailableBioBricksManager::getBioBrickFromAll found " + brick, Logger.Level.TRACE);
            return brick;
        }
        else
        {
            Logger.Log("AvailableBioBricksManager::getBioBrickFromAll failed to find brick with name " + brickName + "!", Logger.Level.WARN);
            return null;
        }
    }

    private void loadAllBioBricks()
    {
        Logger.Log("AvailableBioBricksManager::loadAllBioBricks", Logger.Level.DEBUG);
        loadBioBricks(_allBioBrickFiles, _allBioBricks);
        Logger.Log("AvailableBioBricksManager::loadAllBioBricks _allBioBricks=" + _allBioBricks.Count, Logger.Level.INFO);
    }

    // Warning: inputFiles is an array of names of files inside 'biobrickFilesPathPrefix'
    private void loadBioBricks(string[] inputFiles, LinkedList<BioBrick> destination)
    {
        Logger.Log("AvailableBioBricksManager::loadBioBricks", Logger.Level.INFO);
        //load biobricks from xml
        BioBrickLoader bLoader = new BioBrickLoader();

        //_availableBioBricks   = new LinkedList<BioBrick>();
        string files = "";

        foreach (string file in inputFiles)
        {
            string fullPath = biobrickFilesPathPrefix + file;
            Logger.Log("AvailableBioBricksManager::loadBioBricks loads biobrick file " + fullPath, Logger.Level.DEBUG);
            LinkedList<BioBrick> bb = bLoader.loadBioBricksFromFile(fullPath);
            Logger.Log("AvailableBioBricksManager::loadBioBricks appended bb=" + bb.Count.ToString() + " from file " + fullPath, Logger.Level.DEBUG);
            LinkedListExtensions.AppendRange<BioBrick>(destination, bb);
            if (!string.IsNullOrEmpty(files))
            {
                files += ", ";
            }
            files += fullPath;
        }
        Logger.Log("AvailableBioBricksManager::loadBioBricks loaded " + files + " so that destination=" + destination.Count, Logger.Level.DEBUG);
    }


    public LinkedList<BioBrick> getAvailableBioBricks()
    {
        string availableBB = null == _availableBioBricks ? "null" : _availableBioBricks.Count.ToString();
        Logger.Log("AvailableBioBricksManager::getAvailableBioBricks with initial _availableBioBricks=" + availableBB, Logger.Level.DEBUG);
        if (_availableBioBricks == null || _availableBioBricks.Count == 0)
        {
            loadAvailableBioBricks();
        }
        Logger.Log("AvailableBioBricksManager::getAvailableBioBricks returns " + _availableBioBricks.Count + " elements", Logger.Level.DEBUG);
        return _availableBioBricks;
    }


    private void loadAvailableBioBricks()
    {
        Logger.Log("AvailableBioBricksManager::loadAvailableBioBricks", Logger.Level.INFO);
        LevelInfo levelInfo = null;
        MemoryManager.get().tryGetCurrentLevelInfo(out levelInfo);
        if (null != levelInfo && levelInfo.areAllBioBricksAvailable)
        {
            //load all biobricks
            //loadBioBricks(_allBioBrickFiles, _availableBioBricks);
            //or just copy them
            foreach (BioBrick bb in getAllBioBricks())
            {
                _availableBioBricks.AddLast(bb);
            }
        }
        else
        {
            //default behavior
            List<string> filesToLoad = new List<string>(_availableBioBrickFiles);
            filesToLoad.Add(MemoryManager.get().configuration.getGameMapName());
            loadBioBricks(filesToLoad.ToArray(), _availableBioBricks);
        }
        Logger.Log("AvailableBioBricksManager::loadAvailableBioBricks _availableBioBricks=" + _availableBioBricks.Count, Logger.Level.DEBUG);
    }

/*
    // Use this for initialization
    void Start()
    {
        Logger.Log("AvailableBioBricksManager::Start()", Logger.Level.TRACE);
        displayPromoters();
    }
    */
}

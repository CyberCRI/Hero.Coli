using UnityEngine;


/*DESCRIPTION
 * This class creates the links between the Interface's Scene, classes and GameObject and the others
 * */

public class InterfaceLinkManager : LinkManager
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public const string gameObjectName = "InterfaceLinkManager";
    private static InterfaceLinkManager _instance;

    public static InterfaceLinkManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("InterfaceLinkManager::get was badly initialized");
            GameObject go = GameObject.Find(gameObjectName);
            if (go)
            {
                _instance = go.GetComponent<InterfaceLinkManager>();
            }
            else
            {
                Debug.LogError(gameObjectName + " not found");
            }
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
        }
    }

    new void Start()
    {
        Debug.Log(this.GetType() + " Start");
        base.Start();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    private Fade fade;
    [SerializeField]
    private TooltipPanel biobrickTooltipPanel, deviceTooltipPanel;

    [SerializeField]
    private GameObject craftZoneDisplayedBioBrickPrefab;
    [SerializeField]
    private GameObject lastHoveredInfo;
    [SerializeField]
    private GameObject genericInfoWindow;


    bool isCraftMode1 = true;

    // craft screen with activate/deactivate button, device slots, recipes, biobricks sorted by columns
    [SerializeField]
    private GameObject craftScreenPanel1;

    private GameObject craftScreenPanel;
    [SerializeField]
    private Transform craftSlotsGrid;

    [SerializeField]
    private CraftFinalizer craftFinalizer;

    [SerializeField]
    private GameObject tutorialArrow;

    [SerializeField]
    private GameObject tutorialPanels;

    [SerializeField]
    private GameObject introduction1, introduction2, okButton1, okButton2, end, pauseIndicator;
    [SerializeField]
    private EndMainMenuButton endMainMenuButton;

    // main menu
    public ControlsMainMenuItemArray controlsArray;
    public AbsoluteWASDButton absoluteWASDButton;
    public LeftClickToMoveButton leftClickToMoveButton;
    public RelativeWASDButton relativeWASDButton;
    public RightClickToMoveButton rightClickToMoveButton;
    public UISprite selectedKeyboardControlTypeSprite;
    public UISprite selectedMouseControlTypeSprite;
    [SerializeField]
    private GameObject modalBackground;
    [SerializeField]
    private GameObject genericModalWindow;
    
    public MainMenuManager mainMenu;

    [SerializeField]
    private LoggerLabel loggerGUIComponent;

    [SerializeField]
    private VectrosityPanel celliaGraph, roomGraph;
    [SerializeField]
    private GraphMoleculeList graphMoleculeList;
    [SerializeField] // WorldScreensPanel
    private GameObject worldScreensPanel;
    public FocusMaskManager focusMaskManager;

    protected override int getLMIndex()
    {
        return GameStateController.ilmIndex;
    }

    public override void initialize()
    {
        base.initialize ();

       //  Debug.Log("InterfaceLinkManager: mainMenu=" + mainMenu);

        // activate everything
        activateAllChildren(true);

        //shortcut
        CraftZoneManager craftZoneManager = CraftZoneManager.get();
        GameStateController gameStateController = GameStateController.get();
        //CraftFinalizer _craftfinalizer = craftFinalizer;
        GUITransitioner guiTransitioner = GUITransitioner.get();
        DevicesDisplayer devicesDisplayer = DevicesDisplayer.get();
        InfoWindowManager infoWindowManager = InfoWindowManager.get();
        AvailableBioBricksManager availableBioBricksManager = AvailableBioBricksManager.get();
        TooltipManager tooltipManager = TooltipManager.get();
        ModalManager modalManager = ModalManager.get();

        //GUITransitioner
        guiTransitioner.celliaGraph = celliaGraph;
        guiTransitioner.roomGraph = roomGraph;

        guiTransitioner.worldScreen = worldScreensPanel;

        craftScreenPanel = craftScreenPanel1;

        guiTransitioner.craftScreen = craftScreenPanel;

        ContinueButton cb = okButton1.GetComponent<ContinueButton>();
        StartGameButton sgb = okButton2.GetComponent<StartGameButton>();

        //GameStateController
        gameStateController.intro = introduction1;
        gameStateController.introContinueButton = cb;
        gameStateController.fadeSprite = fade;
        gameStateController.endWindow = end;
        EndMainMenuButton emmb = endMainMenuButton.GetComponent<EndMainMenuButton>();
        gameStateController.endMainMenuButton = emmb;
        gameStateController.mainMenu = mainMenu;

        gameStateController.pauseIndicator = pauseIndicator;

        //initialization of intro panels
        cb.nextInfoPanel = introduction2;
        cb.nextInfoPanelContinue = sgb;

        //CraftZoneManager
        craftZoneManager.craftFinalizer = craftFinalizer;

        //CraftFinalizer _craftFinalizer2 = CraftZoneManager.get().GetComponent<CraftZoneManager>().craftFinalizer;
        if (null == craftFinalizer.craftFinalizationButton)
            craftFinalizer.craftFinalizationButton = GameObject.Find("CraftButton").GetComponent<CraftFinalizationButton>();

        craftZoneManager.displayedBioBrick = craftZoneDisplayedBioBrickPrefab;
        craftZoneManager.lastHoveredInfoManager = lastHoveredInfo.GetComponent<LastHoveredInfoManager>();


        string assemblyZoneName = isCraftMode1 ? "CraftSlotsPanel" : "AssemblyZonePanel";
        craftZoneManager.assemblyZonePanel = craftScreenPanel.transform.FindChild("TopPanel").transform.FindChild(assemblyZoneName).gameObject;

        if (isCraftMode1)
        {
            LimitedBiobricksCraftZoneManager lbczm = (LimitedBiobricksCraftZoneManager)craftZoneManager;
            lbczm.slotsGrid = craftSlotsGrid;
        }


        //DevicesDisplayer
        devicesDisplayer.listedInventoryPanel = craftScreenPanel.transform.FindChild("BottomPanel").transform.FindChild("DevicesPanel").GetComponent<UIPanel>();
        devicesDisplayer.listedDevicesGrid = GameObject.Find("ListedDevicesGrid").transform;

        devicesDisplayer.graphMoleculeList = graphMoleculeList;


        //InfoWindowManager
        infoWindowManager.infoPanel = genericInfoWindow;
        infoWindowManager.titleLabel = genericInfoWindow.transform.FindChild("TitleLabel").GetComponent<UILocalize>();
        infoWindowManager.subtitleLabel = genericInfoWindow.transform.FindChild("SubtitleLabel").GetComponent<UILocalize>();
        infoWindowManager.explanationLabel = genericInfoWindow.transform.FindChild("ExplanationLabel").GetComponent<UILocalize>();
        infoWindowManager.bottomLabel = genericInfoWindow.transform.FindChild("BottomLabel").GetComponent<UILocalize>();
        infoWindowManager.infoSprite = genericInfoWindow.transform.FindChild("InfoSprite").GetComponent<UISprite>();

        //ModalManager
        modalManager.modalBackground = modalBackground;
        modalManager.genericModalWindow = genericModalWindow;
        modalManager.titleLabel = genericModalWindow.transform.FindChild("TitleLabel").GetComponent<UILocalize>();
        modalManager.explanationLabel = genericModalWindow.transform.FindChild("ExplanationLabel").GetComponent<UILocalize>();
        modalManager.infoSprite = genericModalWindow.transform.FindChild("InfoSprite").GetComponent<UISprite>();
        modalManager.genericValidateButton = genericModalWindow.transform.FindChild("ValidateButton").gameObject;
        modalManager.genericCenteredValidateButton = genericModalWindow.transform.FindChild("CenteredValidateButton").gameObject;
        modalManager.genericCancelButton = genericModalWindow.transform.FindChild("CancelButton").gameObject;

        //BiobrickInventory

        //AvailableBioBricksManager.get().bioBricksPanel = GameObject.Find("BiobricksPanel");
        availableBioBricksManager.bioBricksPanel = craftScreenPanel.transform.FindChild("BottomPanel").transform.FindChild("BiobricksPanel").gameObject;

        if (isCraftMode1)
        {
            availableBioBricksManager.promoterBrickCategoryGrid = GameObject.Find("PromoterBrickCategoryGrid").transform;
            availableBioBricksManager.rbsBrickCategoryGrid = GameObject.Find("RBSBrickCategoryGrid").transform;
            availableBioBricksManager.geneBrickCategoryGrid = GameObject.Find("CodingSequenceBrickCategoryGrid").transform;
            availableBioBricksManager.terminatorBrickCategoryGrid = GameObject.Find("TerminatorBrickCategoryGrid").transform;
        }
        else
        {
            availableBioBricksManager.availableBioBrick = availableBioBricksManager.bioBricksPanel.transform.FindChild("AvailableDisplayedBioBrickPrefab").gameObject;
        }

        //TooltipManager
        tooltipManager.bioBrickTooltipPanel = biobrickTooltipPanel;
        tooltipManager.deviceTooltipPanel = deviceTooltipPanel;
        tooltipManager.uiCamera = GameObject.Find("Camera").GetComponent<Camera>();

        Logger.get().loggerGUIComponent = loggerGUIComponent;
    }


    public override void finishInitialize()
    {
        base.finishInitialize ();

        GameObject bars = GameObject.Find("CutSceneBlackBars");

        activateAllChildren(false);

        //TODO should be done in gameStateController instead

        // in TutorialPanels
        tutorialPanels.SetActive(true);
        introduction1.SetActive(false);
        introduction2.SetActive(false);
        end.SetActive(false);
        bars.SetActive(true);

        // in WorldScreensPanel
        pauseIndicator.SetActive(false);

        CraftZoneManager.get().initializeIfNecessary();
        AvailableBioBricksManager.get().initialize();
        focusMaskManager.reinitialize();

        Inventory.get().initialize();
        GUITransitioner.get().initialize();
    }

}

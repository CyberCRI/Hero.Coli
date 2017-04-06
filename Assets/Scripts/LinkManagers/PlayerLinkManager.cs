using UnityEngine;

/*
 * This class creates the links between the Player's Scene, classes and GameObject and the other scenes
 * */

public class PlayerLinkManager : LinkManager
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "PlayerLinkManager";
    private static PlayerLinkManager _instance;

    public static PlayerLinkManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("PlayerLinkManager get was badly initialized");
            GameObject go = GameObject.Find(gameObjectName);
            if (go)
            {
                _instance = go.GetComponent<PlayerLinkManager>();
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
        }
    }

    new void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        base.Start();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    private GameObject characterGO;
    [SerializeField]
    private Character character;
    [SerializeField]
    private PhenoFickContact phenoFickContact;
    [SerializeField]
    private CellControl cellControl;
    [SerializeField]
    private BoundCamera mainBoundCamera;

    protected override int getLMIndex()
    {
        return GameStateController.plmIndex;
    }

    public override void initialize()
    {
        base.initialize();

        GUITransitioner guiTransitioner = GUITransitioner.get();

        //Cellcontrol connection
        guiTransitioner.control = cellControl;

        InterfaceLinkManager interfaceLinkManager = GameObject.Find(InterfaceLinkManager.gameObjectName).GetComponent<InterfaceLinkManager>();
        cellControl.absoluteWASDButton = interfaceLinkManager.absoluteWASDButton;
        cellControl.leftClickToMoveButton = interfaceLinkManager.leftClickToMoveButton;
        cellControl.relativeWASDButton = interfaceLinkManager.relativeWASDButton;
        cellControl.rightClickToMoveButton = interfaceLinkManager.rightClickToMoveButton;

        cellControl.selectedKeyboardControlTypeSprite = interfaceLinkManager.selectedKeyboardControlTypeSprite;
        cellControl.selectedMouseControlTypeSprite = interfaceLinkManager.selectedMouseControlTypeSprite;

        interfaceLinkManager.controlsArray.cellControl = cellControl;
        interfaceLinkManager.focusMaskManager.cellControl = cellControl;

        //Character connections
        character.lifeAnimation = interfaceLinkManager.lifeIndicator;
        character.energyAnimation = interfaceLinkManager.energyIndicator;

        //PhenoFickcontact connections
        //TODO use InterfaceLinkManager
        phenoFickContact.vectroPanel = GameObject.Find("RoomMediumInfoBackgroundSprite").GetComponent<VectrosityPanel>();
        phenoFickContact.graphMoleculeList = GameObject.Find("MediumInfoPanelRoom").GetComponent<GraphMoleculeList>();

        //Main Camera
        guiTransitioner.mainBoundCamera = mainBoundCamera;
    }

    public override void finishInitialize()
    {
        cellControl.initialize();

        base.finishInitialize();
    }
}

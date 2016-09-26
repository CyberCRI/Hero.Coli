using UnityEngine;


/*
 * This class creates the links between the Player's Scene, classes and GameObject and the other scenes
 * */

public class PlayerLinkManager : LinkManager
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "PlayerLinkManager";
    private static PlayerLinkManager _instance;

    public static PlayerLinkManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("PlayerLinkManager::get was badly initialized");
            GameObject go = GameObject.Find(gameObjectName);
            if (go)
            {
                _instance = go.GetComponent<PlayerLinkManager>();
            }
        }
        return _instance;
    }

    void Awake()
    {
        _instance = this;
        //Debug.LogError("PlayerLinkManager awakes with (_instance == null)=="+(_instance == null));
    }

    void OnDestroy()
    {
        //Debug.LogError("PlayerLinkManager OnDestroy");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public override void initialize()
    {

        GameObject perso = Hero.get().gameObject;
        Hero hero = perso.GetComponent<Hero>();
        PhenoFickContact pheno = perso.GetComponent<PhenoFickContact>();
        GUITransitioner guiTransitioner = GUITransitioner.get();
        CellControl cellControl = perso.GetComponent<CellControl>();

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


        //Hero connections
        hero.lifeAnimation = GameObject.Find("LifeLogo").GetComponent<LifeLogoAnimation>();
        hero.energyAnimation = GameObject.Find("EnergyLogo").GetComponent<EnergyLogoAnimation>();

        GameObject.Find("LifeIndicator").GetComponent<LifeIndicator>().hero = hero;
        GameObject.Find("EnergyIndicator").GetComponent<EnergyIndicator>().hero = hero;
        guiTransitioner.hero = hero;


        //PhenoFickcontact connections
        //TODO use InterfaceLinkManager
        pheno.vectroPanel = GameObject.Find("RoomMediumInfoBackgroundSprite").GetComponent<VectrosityPanel>();
        pheno.graphMoleculeList = GameObject.Find("MediumInfoPanelRoom").GetComponent<GraphMoleculeList>();


        //Main Camera
        guiTransitioner.mainBoundCamera = GameObject.Find("Main Camera").GetComponent<BoundCamera>();

        //MemoryManager reporting
        Debug.Log("PlayerLinkManager calls MemoryManager.get()");
        MemoryManager.get().hero = hero;
    }

    public override void finishInitialize()
    {
    }
}

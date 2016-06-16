using UnityEngine;


/*
 * This class creates the links between the Player's Scene, classes and GameObject and the other scenes
 * */

public class PlayerLinkManager : LinkManager
{

    private const string persoGameObjectName = "Perso";


	public override void initialize ()
    {

    GameObject perso = GameObject.Find(persoGameObjectName);
		Hero hero = perso.GetComponent<Hero>();
		PhenoFickContact pheno = perso.GetComponent<PhenoFickContact>();
    GUITransitioner guiTransitioner = GUITransitioner.get ();
    CellControl cellControl = perso.GetComponent<CellControl>();

		//Cellcontrol connection
    guiTransitioner.control = cellControl;

    InterfaceLinkManager interfaceLinkManager = GameObject.Find(InterfaceLinkManager.interfaceLinkManagerGameObjectName).GetComponent<InterfaceLinkManager>();
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
		
		GameObject.Find ("LifeIndicator").GetComponent<LifeIndicator>().hero = hero;
		GameObject.Find ("EnergyIndicator").GetComponent<EnergyIndicator>().hero = hero;
    guiTransitioner.hero = hero;


		//PhenoFickcontact connections
    //TODO use InterfaceLinkManager
		pheno.vectroPanel = GameObject.Find("RoomMediumInfoBackgroundSprite").GetComponent<VectrosityPanel>();
		pheno.graphMoleculeList = GameObject.Find("MediumInfoPanelRoom").GetComponent<GraphMoleculeList>();


		//Main Camera
    guiTransitioner.mainBoundCamera = GameObject.Find ("Main Camera").GetComponent<BoundCamera>();

    //MemoryManager reporting
    MemoryManager.get ().hero = hero;
  }
  
    public override void finishInitialize ()
    {
    }
}

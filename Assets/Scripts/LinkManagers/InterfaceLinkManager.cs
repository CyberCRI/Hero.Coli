using UnityEngine;
using System.Collections;

/*DESCRIPTION
 * This class creates the links between the Interface's Scene, classes and GameObject and the others
 * */

public class InterfaceLinkManager : MonoBehaviour {

	public Fade fade;
	public TooltipPanel biobrickTooltipPanel,deviceTooltipPanel;
	public GameObject inventoryDevicePrefab;
	public GameObject listedDevicePrefab;

	public GameObject craftZoneDisplayedBioBrickPrefab;
	public GameObject lastHoveredInfo;
	public GameObject genericInfoWindow;
	public GameObject craftScreenPanel;
  public GameObject equipedDeviceButtonPrefabPos, equipedDeviceButtonPrefabPos2;
  public UIPanel equipedDevicesSlotsPanel;
  public GameObject equipedDevice ,equipedDevice2;
  public GameObject tinyBioBrickIconPrefabPos ,tinyBioBrickIconPrefabPos2;
	public CraftFinalizer craftFinalizer;
	public UIPanel inventoryDevicesSlotsPanel;

	public GameObject tutorialArrow;

  public GameObject tutorialPanels;

  public GameObject introduction1;
  public GameObject introduction2;
  public GameObject okButton1;
  public GameObject okButton2;
  public GameObject end;

  public CellControlButton absoluteWASDButton;
  public CellControlButton leftClickToMoveButton;
  public CellControlButton relativeWASDButton;
  public CellControlButton rightClickToMoveButton;
  public UISprite selectedControlTypeSprite;
  public GameObject modalBackground;
  public GameObject genericModalWindow;

	//public Camera _uicamera;


	// Use this for initialization
	void Awake () {
		//shortcut
		CraftZoneManager craftZoneManager = CraftZoneManager.get();
		GameStateController gameStateController = GameStateController.get();
		//CraftFinalizer _craftfinalizer = craftFinalizer;
		GUITransitioner guiTransitioner = GUITransitioner.get ();
    DevicesDisplayer devicesDisplayer = DevicesDisplayer.get();
    InfoWindowManager infoWindowManager = InfoWindowManager.get();
    AvailableBioBricksManager availableBioBricksManager = AvailableBioBricksManager.get();
    TooltipManager tooltipManager = TooltipManager.get();
    ModalManager modalManager = ModalManager.get();
    GameObject mediumInfoPanelCell = GameObject.Find("MediumInfoPanelCell");

		//GUITransitioner
    guiTransitioner.celliaGraph = mediumInfoPanelCell.transform.Find("CellMediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		guiTransitioner.roomGraph = GameObject.Find ("MediumInfoPanelRoom").transform.Find("RoomMediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		guiTransitioner.animator = GameObject.Find ("WorldEquipButton").GetComponent<InventoryAnimator>();
		guiTransitioner.worldScreen = GameObject.Find ("WorldScreensPanel");
		guiTransitioner.craftScreen = craftScreenPanel;


		//GameStateController
    gameStateController.intro = introduction1;
		gameStateController.fadeSprite = fade;
		gameStateController.end = end;

		//Object with GameStateController 
		okButton1.GetComponent<ContinueButton>().gameStateController = gameStateController;
		okButton2.GetComponent<StartGameButton>().gameStateController = gameStateController;
    tutorialPanels.SetActive (true);
    introduction1.SetActive(false);
    introduction2.SetActive(false);
    end.SetActive(false);

		//CraftFinalizer
		craftFinalizer.ToCraftZoneManager = craftZoneManager;

		//CraftZoneManager
		craftZoneManager.GetComponent<CraftZoneManager>().craftFinalizer = craftFinalizer;

		//CraftFinalizer _craftFinalizer2 = CraftZoneManager.get().GetComponent<CraftZoneManager>().craftFinalizer;
    if(null == craftFinalizer.craftFinalizationButton)
      craftFinalizer.craftFinalizationButton = GameObject.Find("CraftButton").GetComponent<CraftFinalizationButton>();

		craftZoneManager.displayedBioBrick = craftZoneDisplayedBioBrickPrefab;
		craftZoneManager.lastHoveredInfoManager = lastHoveredInfo.GetComponent<LastHoveredInfoManager>();
		craftZoneManager.assemblyZonePanel = craftScreenPanel.transform.FindChild ("TopPanel").transform.FindChild("AssemblyZonePanel").gameObject;


		//DevicesDisplayer
        
    devicesDisplayer.equipPanel = equipedDevicesSlotsPanel;
    devicesDisplayer.inventoryPanel = inventoryDevicesSlotsPanel;
    devicesDisplayer.listedInventoryPanel = craftScreenPanel.transform.FindChild ("BottomPanel").transform.FindChild("DevicesPanel").GetComponent<UIPanel>();

    devicesDisplayer.graphMoleculeList = mediumInfoPanelCell.GetComponent<GraphMoleculeList>() as GraphMoleculeList;
            
    devicesDisplayer.equipedDevice = equipedDeviceButtonPrefabPos;
    devicesDisplayer.equipedDevice2 = equipedDeviceButtonPrefabPos2;

    devicesDisplayer.inventoryDevice = inventoryDevicePrefab;
		devicesDisplayer.listedInventoryDevice =listedDevicePrefab;
		

		//InfoWindowManager

		infoWindowManager.infoPanel = genericInfoWindow;

        /* TODO
         * should rename children with unique names
         * so that children can be fetched using GameObject.Find
         * instead of browsing children
         */ 
		infoWindowManager.titleLabel = genericInfoWindow.transform.FindChild("TitleLabel").GetComponent<UILabel>();
		infoWindowManager.subtitleLabel = genericInfoWindow.transform.FindChild("SubtitleLabel").GetComponent<UILabel>();
		infoWindowManager.explanationLabel = genericInfoWindow.transform.FindChild("ExplanationLabel").GetComponent<UILabel>();
		infoWindowManager.bottomLabel = genericInfoWindow.transform.FindChild("BottomLabel").GetComponent<UILabel>();
		infoWindowManager.infoSprite = genericInfoWindow.transform.FindChild("InfoSprite").GetComponent<UISprite>();


		//DeviceInventory
		Inventory.get().animator = GameObject.Find ("WorldEquipButton").GetComponent<InventoryAnimator>();
		Inventory.get ().animator.tutorialArrowAnimation = tutorialArrow.GetComponent<ArrowAnimation>();
		
		//BiobrickInventory
		
		//AvailableBioBricksManager.get().bioBricksPanel = GameObject.Find("BiobricksPanel");
		availableBioBricksManager.bioBricksPanel = craftScreenPanel.transform.FindChild ("BottomPanel").transform.FindChild("BiobricksPanel").gameObject;
		availableBioBricksManager.availableBioBrick = availableBioBricksManager.bioBricksPanel.transform.FindChild("AvailableDisplayedBioBrickPrefab").gameObject;
		//AvailableBioBricksManager.get ().availableBioBrick = GameObject.Find ("AvailableDisplayedBioBrickPrefab");
		
		//TooltipManager
    tooltipManager.bioBrickTooltipPanel = biobrickTooltipPanel;
		tooltipManager.deviceTooltipPanel = deviceTooltipPanel;
		tooltipManager.uiCamera = GameObject.Find("Camera").GetComponent<Camera>();

    //ModalManager
    modalManager.modalBackground = modalBackground;
    modalManager.genericModalWindow = genericModalWindow;
  }
}

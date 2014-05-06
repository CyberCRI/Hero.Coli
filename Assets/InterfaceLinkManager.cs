using UnityEngine;
using System.Collections;

/*DESCRIPTION
 * This class create the links between the Interface's Scene, classes and GameObject and the others
 * */

public class InterfaceLinkManager : MonoBehaviour {

	public Fade fade;
	public TooltipPanel biobrickTooltipPanel,deviceTooltipPanel;
	public GameObject inventoryButtonprefab;
	public GameObject listedDevicePrefab;

	public GameObject craftZoneDisplayedBioBrickPrefab;
	public GameObject lastHoveredInfo;
	public GameObject genericInfoWindow;
	public GameObject craftScreenPanel;
	public GameObject equipedDeviceButtonPrefabPos, equipedDeviceButtonPrefabPos2;
	public GameObject tinyBioBrickIconPrefabPos ,tinyBioBrickIconPrefabPos2;
	public CraftFinalizer craftFinalizer;
	public UIPanel inventoryDevicesSlotsPanel;
	public UIPanel end;

	//public Camera _uicamera;


	// Use this for initialization
	void Awake () {
		//shortcut
		CraftZoneManager _craftManager = CraftZoneManager.get();
		GameStateController _game = GameStateController.get();
		//CraftFinalizer _craftfinalizer = craftFinalizer;
		GUITransitioner _GUITransitioner = GUITransitioner.get ();

		//GUITransitioner
		_GUITransitioner._celliaGraph = GameObject.Find ("MediumInfoPanelCell").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		_GUITransitioner._roomGraph = GameObject.Find ("MediumInfoPanelRoom").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		_GUITransitioner.scriptAnimator = GameObject.Find ("WorldEquipButton").GetComponent<InventoryAnimator>();
		_GUITransitioner._worldScreen = GameObject.Find ("WorldScreensPanel");
		_GUITransitioner._craftScreen = craftScreenPanel;




		//GameStateController
		_game.introPanel = GameObject.Find ("Introduction1").GetComponent<UIPanel>();
		_game.fadeSprite = fade;
		_game.endPanel = end;

		//Object with GameStateController 

		GameObject.Find ("OK1Button").GetComponent<ContinueButton>().gameStateController = _game;
		GameObject.Find ("OK2Button").GetComponent<StartGameButton>().gameStateController = _game;
		GameObject.Find ("Introduction1").SetActive(false);
		GameObject.Find ("Introduction2").SetActive(false);





		//CraftFinalizer
		craftFinalizer.TOCraftZoneManager = _craftManager;

		//CraftZoneManager
		_craftManager.GetComponent<CraftZoneManager>().craftFinalizer = craftFinalizer;

		//CraftFinalizer _craftFinalizer2 = CraftZoneManager.get().GetComponent<CraftZoneManager>().craftFinalizer;
		craftFinalizer.craftFinalizationButton = craftFinalizer.transform.Find("CraftButton").gameObject
			.GetComponent<CraftFinalizationButton>();

		_craftManager.displayedBioBrick = craftZoneDisplayedBioBrickPrefab;
		_craftManager.lastHoveredInfoManager = lastHoveredInfo.GetComponent<LastHoveredInfoManager>();
		_craftManager.assemblyZonePanel = craftScreenPanel.transform.FindChild ("TopPanel").transform.FindChild("AssemblyZonePanel").gameObject;


		//DevicesDisplayer

		DevicesDisplayer.get().inventoryDevice = inventoryButtonprefab;
		DevicesDisplayer.get().listedInventoryDevice =listedDevicePrefab;
		DevicesDisplayer.get().equipedDevice = equipedDeviceButtonPrefabPos;
		DevicesDisplayer.get().equipedDevice2 = equipedDeviceButtonPrefabPos2;
		DevicesDisplayer.get ().equipPanel = GameObject.Find ("EquipedDevicesSlotsPanel").GetComponent<UIPanel>();
		DevicesDisplayer.get ().inventoryPanel = inventoryDevicesSlotsPanel;
		DevicesDisplayer.get().listedInventoryPanel = craftScreenPanel.transform.FindChild ("BottomPanel").transform.FindChild("DevicesPanel").GetComponent<UIPanel>();


		//InfoWindowManager

		InfoWindowManager.get().infoPanel = genericInfoWindow;
		InfoWindowManager.get().titleLabel = genericInfoWindow.transform.FindChild("TitleLabel").GetComponent<UILabel>();
		InfoWindowManager.get().subtitleLabel = genericInfoWindow.transform.FindChild("SubtitleLabel").GetComponent<UILabel>();
		InfoWindowManager.get().explanationLabel = genericInfoWindow.transform.FindChild("ExplanationLabel").GetComponent<UILabel>();
		InfoWindowManager.get().bottomLabel = genericInfoWindow.transform.FindChild("BottomLabel").GetComponent<UILabel>();
		InfoWindowManager.get().infoSprite = genericInfoWindow.transform.FindChild("InfoSprite").GetComponent<UISprite>();


		//DeviceInventory
		Inventory.get().scriptAnimator = GameObject.Find ("WorldEquipButton").GetComponent<InventoryAnimator>();
		
		//BiobrickInventory
		
		//AvailableBioBricksManager.get().bioBricksPanel = GameObject.Find("BiobricksPanel");
		AvailableBioBricksManager.get().bioBricksPanel = craftScreenPanel.transform.FindChild ("BottomPanel").transform.FindChild("BiobricksPanel").gameObject;
		AvailableBioBricksManager.get ().availableBioBrick = AvailableBioBricksManager.get().bioBricksPanel.transform.FindChild("AvailableDisplayedBioBrickPrefab").gameObject;
		//AvailableBioBricksManager.get ().availableBioBrick = GameObject.Find ("AvailableDisplayedBioBrickPrefab");
		
		//TooltipManager
		TooltipManager.get().bioBrickTooltipPanel = biobrickTooltipPanel;
		TooltipManager.get().deviceTooltipPanel = deviceTooltipPanel;
		TooltipManager.get ().uiCamera = GameObject.Find("Camera").GetComponent<Camera>();


	
	}
	
	// Update is called once per frame
	void Start () {


	
	}
}

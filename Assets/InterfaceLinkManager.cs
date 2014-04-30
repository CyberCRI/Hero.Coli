using UnityEngine;
using System.Collections;

public class InterfaceLinkManager : MonoBehaviour {

	public Fade fade;
	public TooltipPanel biobrickTooltipPanel,deviceTooltipPanel;
	public GameObject inventoryButtonprefab;
	public GameObject listedDevicePrefab;

	public GameObject craftZoneDisplayedBioBrickPrefab;
	public GameObject lastHoveredInfo;
	public GameObject genericInfoWindow;
	public UIPanel end;

	//public Camera _uicamera;


	// Use this for initialization
	void Start () {
		//shortcut
		CraftZoneManager _craftManager = CraftZoneManager.get();
		GameStateController _game = GameObject.Find ("GameStateController").GetComponent<GameStateController>();
		CraftFinalizer _craftfinalizer = GameObject.Find("FinalizationZonePanel").GetComponent<CraftFinalizer>();
		GUITransitioner _GUITransitioner = GUITransitioner.get ();

		//GUITransitioner
		_GUITransitioner._celliaGraph = GameObject.Find ("MediumInfoPanelCell").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		_GUITransitioner._roomGraph = GameObject.Find ("MediumInfoPanelRoom").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		_GUITransitioner.scriptAnimator = GameObject.Find ("WorldEquipButton").GetComponent<InventoryAnimator>();
		_GUITransitioner._worldScreen = GameObject.Find ("WorldScreensPanel");
		_GUITransitioner._craftScreen = GameObject.Find ("CraftScreenPanel");




		//GameStateController
		_game.introPanel = GameObject.Find ("Introduction1").GetComponent<UIPanel>();
		_game.fadeSprite = fade;
		_game.endPanel = end;

		//Object with GameStateController 

		GameObject.Find ("OK1Button").GetComponent<ContinueButton>().gameStateController = _game;
		GameObject.Find ("OK2Button").GetComponent<StartGameButton>().gameStateController = _game;
		GameObject.Find ("Introduction1").SetActive(false);
		GameObject.Find ("Introduction2").SetActive(false);


		//TooltipManager
		TooltipManager.get().bioBrickTooltipPanel = biobrickTooltipPanel;
		TooltipManager.get().deviceTooltipPanel = deviceTooltipPanel;
		TooltipManager.get ().uiCamera = GameObject.Find("Camera").GetComponent<Camera>();


		//CraftFinalizer
		GameObject.Find("FinalizationZonePanel").GetComponent<CraftFinalizer>().TOCraftZoneManager = _craftManager;

		//CraftZoneManager
		_craftManager.GetComponent<CraftZoneManager>().craftFinalizer = _craftfinalizer;

		//CraftFinalizer _craftFinalizer2 = CraftZoneManager.get().GetComponent<CraftZoneManager>().craftFinalizer;
		_craftfinalizer.craftFinalizationButton = GameObject.Find("FinalizationZonePanel").transform.Find("CraftButton").gameObject
			.GetComponent<CraftFinalizationButton>();

		_craftManager.displayedBioBrick = craftZoneDisplayedBioBrickPrefab;
		_craftManager.lastHoveredInfoManager = lastHoveredInfo.GetComponent<LastHoveredInfoManager>();
		_craftManager.assemblyZonePanel = GameObject.Find ("AssemblyZonePanel");


		//DevicesDisplayer

		DevicesDisplayer.get().inventoryDevice = inventoryButtonprefab;
		Logger.Log("ListedDevice ==>"+listedDevicePrefab,Logger.Level.WARN);
		DevicesDisplayer.get().listedInventoryDevice =listedDevicePrefab;
		DevicesDisplayer.get().equipedDevice = GameObject.Find("EquipedDeviceButtonPrefabPos");
		DevicesDisplayer.get().equipedDevice2 = GameObject.Find("EquipedDeviceButtonPrefabPos2");
		DevicesDisplayer.get ().equipPanel = GameObject.Find ("EquipedDevicesSlotsPanel").GetComponent<UIPanel>();
		DevicesDisplayer.get ().inventoryPanel = GameObject.Find ("InventoryDevicesSlotsPanel").GetComponent<UIPanel>();
		DevicesDisplayer.get().listedInventoryPanel = GameObject.Find ("DevicesPanel").GetComponent<UIPanel>();


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

		AvailableBioBricksManager.get().bioBricksPanel = GameObject.Find("BiobricksPanel");
		AvailableBioBricksManager.get ().availableBioBrick = GameObject.Find ("AvailableDisplayedBioBrickPrefab");



		//Hero

		GameObject.Find ("Perso").GetComponent<Hero>().energyAnimation = GameObject.Find ("EnergyLogo").GetComponent<EnergyLogoAnimation>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

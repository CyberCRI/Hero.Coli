﻿using UnityEngine;
using System.Collections;

/*DESCRIPTION
 * This class creates the links between the Interface's Scene, classes and GameObject and the others
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
		CraftZoneManager craftZoneManager = CraftZoneManager.get();
		GameStateController gameStateController = GameStateController.get();
		//CraftFinalizer _craftfinalizer = craftFinalizer;
		GUITransitioner guiTransitioner = GUITransitioner.get ();
    DevicesDisplayer devicesDisplayer = DevicesDisplayer.get();
    InfoWindowManager infoWindowManager = InfoWindowManager.get();
    AvailableBioBricksManager availableBioBricksManager = AvailableBioBricksManager.get();
    TooltipManager tooltipManager = TooltipManager.get();

		//GUITransitioner
		guiTransitioner._celliaGraph = GameObject.Find ("MediumInfoPanelCell").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		guiTransitioner._roomGraph = GameObject.Find ("MediumInfoPanelRoom").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		guiTransitioner.scriptAnimator = GameObject.Find ("WorldEquipButton").GetComponent<InventoryAnimator>();
		guiTransitioner._worldScreen = GameObject.Find ("WorldScreensPanel");
		guiTransitioner._craftScreen = craftScreenPanel;


		//GameStateController
		gameStateController.introPanel = GameObject.Find ("Introduction1").GetComponent<UIPanel>();
		gameStateController.fadeSprite = fade;
		gameStateController.endPanel = end;

		//Object with GameStateController 
		GameObject.Find ("OK1Button").GetComponent<ContinueButton>().gameStateController = gameStateController;
		GameObject.Find ("OK2Button").GetComponent<StartGameButton>().gameStateController = gameStateController;
		GameObject.Find ("Introduction1").SetActive(false);
		GameObject.Find ("Introduction2").SetActive(false);


		//CraftFinalizer
		craftFinalizer.ToCraftZoneManager = craftZoneManager;

		//CraftZoneManager
		craftZoneManager.GetComponent<CraftZoneManager>().craftFinalizer = craftFinalizer;

		//CraftFinalizer _craftFinalizer2 = CraftZoneManager.get().GetComponent<CraftZoneManager>().craftFinalizer;
		craftFinalizer.craftFinalizationButton = craftFinalizer.transform.Find("CraftButton").gameObject
			.GetComponent<CraftFinalizationButton>();

		craftZoneManager.displayedBioBrick = craftZoneDisplayedBioBrickPrefab;
		craftZoneManager.lastHoveredInfoManager = lastHoveredInfo.GetComponent<LastHoveredInfoManager>();
		craftZoneManager.assemblyZonePanel = craftScreenPanel.transform.FindChild ("TopPanel").transform.FindChild("AssemblyZonePanel").gameObject;


		//DevicesDisplayer

    devicesDisplayer.inventoryDevice = inventoryButtonprefab;
		devicesDisplayer.listedInventoryDevice =listedDevicePrefab;
		devicesDisplayer.equipedDevice = equipedDeviceButtonPrefabPos;
		devicesDisplayer.equipedDevice2 = equipedDeviceButtonPrefabPos2;
		devicesDisplayer.equipPanel = GameObject.Find ("EquipedDevicesSlotsPanel").GetComponent<UIPanel>();
		devicesDisplayer.inventoryPanel = inventoryDevicesSlotsPanel;
		devicesDisplayer.listedInventoryPanel = craftScreenPanel.transform.FindChild ("BottomPanel").transform.FindChild("DevicesPanel").GetComponent<UIPanel>();


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
		Inventory.get().scriptAnimator = GameObject.Find ("WorldEquipButton").GetComponent<InventoryAnimator>();
		
		//BiobrickInventory
		
		//AvailableBioBricksManager.get().bioBricksPanel = GameObject.Find("BiobricksPanel");
		availableBioBricksManager.bioBricksPanel = craftScreenPanel.transform.FindChild ("BottomPanel").transform.FindChild("BiobricksPanel").gameObject;
		availableBioBricksManager.availableBioBrick = availableBioBricksManager.bioBricksPanel.transform.FindChild("AvailableDisplayedBioBrickPrefab").gameObject;
		//AvailableBioBricksManager.get ().availableBioBrick = GameObject.Find ("AvailableDisplayedBioBrickPrefab");
		
		//TooltipManager
    tooltipManager.bioBrickTooltipPanel = biobrickTooltipPanel;
		tooltipManager.deviceTooltipPanel = deviceTooltipPanel;
		tooltipManager.uiCamera = GameObject.Find("Camera").GetComponent<Camera>();

  	}
}
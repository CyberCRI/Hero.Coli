using UnityEngine;
using System.Collections;

public class InterfaceLinkManager : MonoBehaviour {

	public Fade fade;
	public TooltipPanel biobrickTooltipPanel,deviceTooltipPanel;
	public GameObject inventoryButtonprefab;
	public GameObject listedDevicePrefab;


	// Use this for initialization
	void Start () {
		//shortcut
		CraftZoneManager _craftManager = CraftZoneManager.get();
		GameStateController _game = GameObject.Find ("GameStateController").GetComponent<GameStateController>();
		CraftFinalizer _craftfinalizer = GameObject.Find("FinalizationZonePanel").GetComponent<CraftFinalizer>();

		//GUITransitioner
		GUITransitioner.get ()._celliaGraph = GameObject.Find ("MediumInfoPanelCell").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();
		GUITransitioner.get ()._roomGraph = GameObject.Find ("MediumInfoPanelRoom").transform.Find("MediumInfoBackgroundSprite").gameObject
			.GetComponent<VectrosityPanel>();


		//GameStateController 
		_game.introPanel = GameObject.Find ("Introduction1").GetComponent<UIPanel>();
		GameObject.Find ("OK1Button").GetComponent<ContinueButton>().gameStateController = _game;
		GameObject.Find ("OK2Button").GetComponent<StartGameButton>().gameStateController = _game;
		GameObject.Find ("Introduction1").SetActive(false);
		GameObject.Find ("Introduction2").SetActive(false);
		_game.fadeSprite = fade;

		//TooltipManager
		TooltipManager.get().bioBrickTooltipPanel = biobrickTooltipPanel;
		TooltipManager.get().deviceTooltipPanel = deviceTooltipPanel;


		//CraftZoneManager in CraftFinalizer
		GameObject.Find("FinalizationZonePanel").GetComponent<CraftFinalizer>().TOCraftZoneManager = _craftManager;

		//CraftFinalizer in CraftZoneManager
		_craftManager.GetComponent<CraftZoneManager>().craftFinalizer = _craftfinalizer;

		CraftFinalizer _craftFinalizer = CraftZoneManager.get().GetComponent<CraftZoneManager>().craftFinalizer;
		_craftFinalizer.craftFinalizationButton = GameObject.Find("FinalizationZonePanel").transform.Find("CraftButton").gameObject
			.GetComponent<CraftFinalizationButton>();

		//DevicesDisplayer

		DevicesDisplayer.get().inventoryDevice = inventoryButtonprefab;
		DevicesDisplayer.get().listedInventoryDevice =listedDevicePrefab;
		DevicesDisplayer.get().equipedDevice = GameObject.Find("EquipedDeviceButtonPrefabPos");
		DevicesDisplayer.get().equipedDevice2 = GameObject.Find("EquipedDeviceButtonPrefabPos2");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class ActivePickingBioBrickInfoPanel : MonoBehaviour {
	
	public GameObject infoPanel;
	public GameStateController gameStateController;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void activate () {
		infoPanel.SetActive(true);
		gameStateController.StateChange(GameState.Pause);
	}
}

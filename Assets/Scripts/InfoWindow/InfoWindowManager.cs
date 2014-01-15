using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfoWindowManager : MonoBehaviour {

  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "InfoWindowManager";
  private static InfoWindowManager _instance;
  public static InfoWindowManager get() {
    if(_instance == null) {
      Logger.Log("InfoWindowManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<InfoWindowManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("InfoWindowManager::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  public GameObject infoPanel;

  public GameStateController gameStateController;

  public static void displayInfoWindow()
  {
    Logger.Log("InfoWindowManager::displayInfoWindow starts", Logger.Level.TEMP);
    _instance.infoPanel.SetActive(true);
    _instance.gameStateController.StateChange(GameState.Pause);
    _instance.gameStateController.dePauseForbidden = true;
  }
}


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

  string title;
  string subtitle;
  string texture;
  string explanation;
  string bottom;
  string onNext;

  public UILabel titleLabel;
  public UILabel subtitleLabel;
  public UILabel explanationLabel;
  public UILabel bottomLabel;

  // TODO manage onNext

  public GameObject infoPanel;
  public UISprite infoSprite;

  public GameStateController gameStateController;

  public static void displayInfoWindow()
  {
    Logger.Log("InfoWindowManager::displayInfoWindow starts", Logger.Level.TEMP);
    fillInFieldsFromCode("any_string");
    _instance.infoPanel.SetActive(true);
    _instance.gameStateController.StateChange(GameState.Pause);
    _instance.gameStateController.dePauseForbidden = true;
  }

  private static void fillInFieldsFromCode(string code)
  {
    Logger.Log("InfoWindowManager::fillInFieldsFromCode("+code+") starts", Logger.Level.TEMP);
    getStuffFromCode(code);

    _instance.titleLabel.text       = _instance.title;
    _instance.subtitleLabel.text    = _instance.subtitle;
    _instance.infoSprite.spriteName = _instance.texture;
    _instance.explanationLabel.text = _instance.explanation;
    _instance.bottomLabel.text      = _instance.bottom;

    // TODO manage onNext
  }

  private static void getStuffFromCode(string code)
  {
    Logger.Log("InfoWindowManager::getStuffFromCode("+code+") starts", Logger.Level.TEMP);

    _instance.title = "title";
    _instance.subtitle = "subtitle";
    _instance.texture = "tuto_intro-01";
    _instance.explanation = "explanation";
    _instance.bottom = "bottom";

    // TODO manage onNext
  }
}


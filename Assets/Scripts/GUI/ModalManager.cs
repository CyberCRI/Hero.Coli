using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Helps manage modal windows
 * TODO: merge with InfoWindowManager
 */ 
public class ModalManager : MonoBehaviour {
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "ModalManager";
    private static ModalManager _instance;
    public static ModalManager get() {
        if(_instance == null) {
            Logger.Log("ModalManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<ModalManager>();
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("ModalManager::Awake", Logger.Level.DEBUG);
        _instance = this;
        loadDataIntoDico(inputFiles, _loadedModalWindows);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

  public string[] inputFiles;
  public float foregroundZ;
  public GameObject modalBackground;
  public GameObject genericModalWindow;

  public UILabel titleLabel;
  public UILabel explanationLabel;

  public UISprite infoSprite;
  public UIButton validateButton;
  private string _validateButtonClass;

  private GameObject _currentModalElement;
  private float _previousZ;
  private Dictionary<string, StandardInfoWindowInfo> _loadedModalWindows = new Dictionary<string, StandardInfoWindowInfo>();
  private static string _genericPrefix = "MODAL.";
  private static string _genericTitle = ".TITLE";
  private static string _genericExplanation = ".EXPLANATION";
    
    
  private static StandardInfoWindowInfo retrieveFromDico(string code)
  {
      StandardInfoWindowInfo info;
      if(!_instance._loadedModalWindows.TryGetValue(code, out info))
      {
          Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
          info = null;
      }
      return info;
  }
    
  private void loadDataIntoDico(string[] inputFiles, Dictionary<string, StandardInfoWindowInfo> dico)
  {      
    InfoWindowLoader iwLoader = new InfoWindowLoader();
    
    string loadedFiles = "";
    
    foreach (string file in inputFiles) {
      foreach (StandardInfoWindowInfo info in iwLoader.loadInfoFromFile(file)) {
        dico.Add(info._code, info);
      }
      if(!string.IsNullOrEmpty(loadedFiles)) {
        loadedFiles += ", ";
      }
      loadedFiles += file;
    }
    
    Logger.Log("ModalManager::loadDataIntoDico loaded "+loadedFiles, Logger.Level.DEBUG);
  }
  
  private static bool fillInFieldsFromCode(string code)
  {      
    StandardInfoWindowInfo info = retrieveFromDico(code);
    
    if(null != info)
    {
      string generic = _genericPrefix+code.ToUpper();
      
      _instance.titleLabel.text       = Localization.Localize(generic+_genericTitle);
      _instance.infoSprite.spriteName = info._texture;
      _instance.explanationLabel.text = Localization.Localize(generic+_genericExplanation);

      _instance.validateButton.gameObject.AddComponent(info._next);
      _instance._validateButtonClass = info._next;
      
      return true;
    }
    else
    {
        return false;
    }
  }

  public static void setModal(GameObject guiComponent)
  {
    if(null != guiComponent)
    {
      Vector3 position = guiComponent.transform.localPosition;
      _instance._previousZ = position.z;
      guiComponent.transform.localPosition = new Vector3(position.x, position.y, _instance.foregroundZ);
      _instance._currentModalElement = guiComponent;

      _instance._currentModalElement.SetActive(true);
      _instance.modalBackground.SetActive(true);

      GameStateController.get().changeState(GameState.Pause);
      GameStateController.get().dePauseForbidden = true;
    }
  }
  
  public static bool setModal(string code)
  {
    if(null != _instance.genericModalWindow && fillInFieldsFromCode(code))
    {
        setModal(_instance.genericModalWindow);
        
        return true;
    }
    else
    {
        Logger.Log("InfoWindowManager::displayInfoWindow("+code+") failed", Logger.Level.WARN);
        return false;
    }
  }

  public static void unsetModal()
  {
    if(null != _instance._currentModalElement)
    {
      Vector3 position = _instance._currentModalElement.transform.localPosition;
      _instance._currentModalElement.transform.localPosition = new Vector3(position.x, position.y, _instance._previousZ);

      if(!string.IsNullOrEmpty(_instance._validateButtonClass))
      {
        Object.Destroy(_instance.validateButton.GetComponent(_instance._validateButtonClass));
        _instance._validateButtonClass = null;
      }
      _instance._currentModalElement.SetActive(false);
      _instance.modalBackground.SetActive(false);

      _instance._currentModalElement = null;
    }
  }
}

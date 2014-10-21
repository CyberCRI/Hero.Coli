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
      
      return true;
    }
    else
    {
        return false;
    }
  }

  public void setModal(GameObject guiComponent)
  {
    if(null != guiComponent)
    {
      Vector3 position = guiComponent.transform.localPosition;
      _previousZ = position.z;
      guiComponent.transform.localPosition = new Vector3(position.x, position.y, foregroundZ);
      _currentModalElement = guiComponent;

      _currentModalElement.SetActive(true);
      modalBackground.SetActive(true);

      GameStateController.get().changeState(GameState.Pause);
      GameStateController.get().dePauseForbidden = true;
    }
  }
  
  public static bool setModal(string code)
  {
    if(null != _instance.genericModalWindow && fillInFieldsFromCode(code))
    {
        _instance.setModal(_instance.genericModalWindow);
        
        return true;
    }
    else
    {
        Logger.Log("InfoWindowManager::displayInfoWindow("+code+") failed", Logger.Level.WARN);
        return false;
    }
  }

  public void unsetModal()
  {
    Vector3 position = _currentModalElement.transform.localPosition;
    _currentModalElement.transform.localPosition = new Vector3(position.x, position.y, _previousZ);

    _currentModalElement.SetActive(false);
    modalBackground.SetActive(false);
  }

  public void unsetModal(MonoBehaviour behaviour)
  {
    if(null != behaviour)
    {
      Object.Destroy(behaviour);
    }    
    unsetModal();
  }
}

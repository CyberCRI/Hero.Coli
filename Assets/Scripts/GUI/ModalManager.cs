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
  public UIButton genericValidateButton;
  public UIButton genericCenteredValidateButton;
  private string _validateButtonClass;
  public UIButton genericCancelButton;

  private UIButton _validateButton;
  private UIButton _cancelButton;

  private GameObject _currentModalElement;
  private float _previousZ;
  private Dictionary<string, StandardInfoWindowInfo> _loadedModalWindows = new Dictionary<string, StandardInfoWindowInfo>();
  private static string _genericPrefix = "MODAL.";
  private static string _genericTitle = ".TITLE";
  private static string _genericExplanation = ".EXPLANATION";
  private static string _quitModalClassName = "QuitModalWindow";
    
  private static StandardInfoWindowInfo retrieveFromDico(string code)
  {
      StandardInfoWindowInfo info;
      //TODO set case-insensitive
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
    
  private static bool needsCancelButton(string validateButtonClassName)
  {
      return (validateButtonClassName != _quitModalClassName);
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

            if(!string.IsNullOrEmpty(info._next))
            {
                if(needsCancelButton(info._next))
                {
                    _instance.genericValidateButton.gameObject.SetActive(true);
                    _instance.genericCancelButton.gameObject.SetActive(true);
                    _instance.genericCenteredValidateButton.gameObject.SetActive(false);

                    _instance.genericValidateButton.gameObject.AddComponent(info._next);
                    _instance._validateButtonClass = info._next;

                    _instance._validateButton = _instance.genericValidateButton;
                    _instance._cancelButton = _instance.genericCancelButton;
                }
                else
                {                    
                    _instance.genericValidateButton.gameObject.SetActive(false);
                    _instance.genericCancelButton.gameObject.SetActive(false);
                    _instance.genericCenteredValidateButton.gameObject.SetActive(true);

                    _instance.genericCenteredValidateButton.gameObject.AddComponent(info._next);
                    _instance._validateButtonClass = info._next;
                    
                    _instance._validateButton = _instance.genericCenteredValidateButton;
                    _instance._cancelButton = null;
                }
            }
            else
            {
                return false;
            }
      
            return true;
        }
        else
        {
            return false;
        }
    }

  public static void setModal(GameObject guiComponent, bool lockPause = true)
  {
    if(null != guiComponent)
    {
      Vector3 position = guiComponent.transform.localPosition;
      _instance._previousZ = position.z;
      guiComponent.transform.localPosition = new Vector3(position.x, position.y, _instance.foregroundZ);
      _instance._currentModalElement = guiComponent;

      _instance._currentModalElement.SetActive(true);
      _instance.modalBackground.SetActive(true);

      if(lockPause)
      {
        GameStateController.get().tryLockPause();
      }
    }
  }
  
  public static bool setModal(string code, bool lockPause = true)
  {
    if(null != _instance.genericModalWindow && fillInFieldsFromCode(code))
    {
        setModal(_instance.genericModalWindow, lockPause);
        
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
                Object.Destroy(_instance.genericValidateButton.GetComponent(_instance._validateButtonClass));
                Object.Destroy(_instance.genericCenteredValidateButton.GetComponent(_instance._validateButtonClass));
                _instance._validateButtonClass = null;
                _instance._validateButton = null;
                _instance._cancelButton = null;
            }
            _instance._currentModalElement.SetActive(false);
            _instance.modalBackground.SetActive(false);

            _instance._currentModalElement = null;
        }
    }

    // manages key presses on modal windows
    //
    // for generic modal windows:
    // enter: validate
    // escape: cancel
    public static GameState manageKeyPresses()
    {
        if((Input.GetKeyDown(KeyCode.Escape) || GameStateController.isShortcutKeyDown(GameStateController._pauseKey)) && (0 == GameStateController.getPausesInStackCount()))
        {
            ModalManager.unsetModal();
            return GameState.Game;
        }
        else
        {
            if(null != _instance._currentModalElement)
            {
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    if(_instance._validateButton.gameObject.activeInHierarchy)
                    {
                        ModalButton button = (ModalButton)_instance._validateButton.gameObject.GetComponent(_instance._validateButtonClass);
                        button.press();
                    }
                }
                else if(Input.GetKeyDown(KeyCode.Escape))
                {   
                    if(_instance._cancelButton.gameObject.activeInHierarchy)
                    {
                        CancelModal button = _instance._cancelButton.gameObject.GetComponent<CancelModal>();
                        button.press();
                    }
                }
            }
        }
        return GameState.Pause;
    }
}

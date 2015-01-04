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
  public GameObject genericValidateButton;
  public GameObject genericCenteredValidateButton;
  private string _validateButtonClass;
  public GameObject genericCancelButton;
  private string _cancelButtonClass;  

    private GameObject _validateButton;
    private GameObject _cancelButton;

  private GameObject _currentModalElement;
  private float _previousZ;
  private Dictionary<string, StandardInfoWindowInfo> _loadedModalWindows = new Dictionary<string, StandardInfoWindowInfo>();
  private static string _genericPrefix = "MODAL.";
  private static string _genericTitle = ".TITLE";
  private static string _genericExplanation = ".EXPLANATION";
  private static string _quitModalClassName = "QuitModalWindow";
  //the class of the component attached to the cancel button of the ModalWindow
  private static string _cancelModalClassName = "CancelModal";
    
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

                    if(!string.IsNullOrEmpty(info._cancel))
                    {
                        Object.Destroy(_instance.genericCancelButton.GetComponent(_instance._cancelButtonClass));
                        _instance.genericCancelButton.gameObject.AddComponent(info._cancel);
                        _instance._cancelButtonClass = info._cancel;
                    }
                    else
                    {
                        _instance._cancelButtonClass = null;
                    }

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

    //TODO custom cancel button class
    public static void setModal(GameObject guiComponent,
                                bool lockPause = true, 
                                GameObject validateButton = null, string validateButtonClass = null,
                                GameObject cancelButton = null, string cancelButtonClass = null
                                )
  {
    if(null != guiComponent)
    {
      Vector3 position = guiComponent.transform.localPosition;
      _instance._previousZ = position.z;
      guiComponent.transform.localPosition = new Vector3(position.x, position.y, _instance.foregroundZ);
      _instance._currentModalElement = guiComponent;

      _instance._currentModalElement.SetActive(true);
      _instance.modalBackground.SetActive(true);

      //TODO check usefulness
      //if(null!=validateButton) {
      _instance._validateButton = validateButton;
      _instance._validateButtonClass = validateButtonClass;
      _instance._cancelButton = cancelButton;
      _instance._cancelButtonClass = cancelButtonClass;

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

    //TODO manage stack of modal elements
    public static void unsetModal()
    {
        if(null != _instance._currentModalElement)
        {
            Vector3 position = _instance._currentModalElement.transform.localPosition;
            _instance._currentModalElement.transform.localPosition = new Vector3(position.x, position.y, _instance._previousZ);

            if(!string.IsNullOrEmpty(_instance._validateButtonClass))
            {
                resetGenericValidateButtons();
                resetGenericCancelButton();
            }
            _instance._currentModalElement.SetActive(false);
            _instance.modalBackground.SetActive(false);

            _instance._currentModalElement = null;
        }
    }

    public static void resetGenericValidateButtons()
    {
        Object.Destroy(_instance.genericValidateButton.GetComponent(_instance._validateButtonClass));
        Object.Destroy(_instance.genericCenteredValidateButton.GetComponent(_instance._validateButtonClass));
        _instance._validateButtonClass = null;
        _instance._validateButton = null;
    }

    public static void resetGenericCancelButton()
    {
        if(!string.IsNullOrEmpty(_instance._cancelButtonClass))
        {
            Object.Destroy(_instance.genericCancelButton.GetComponent(_instance._cancelButtonClass));
            _instance.genericCancelButton.AddComponent(_cancelModalClassName);
        }
        _instance._cancelButtonClass = null;
        _instance._cancelButton = null;
    }

    // manages key presses on modal windows
    //
    // for generic modal windows:
    // enter: validate
    // escape: cancel
    public static GameStateTarget manageKeyPresses()
    {
        //equivalent to: "consumed action", "did something", and so on
        bool keyPressedEventConsumed = false;

        //getting out of Pause
        if((Input.GetKeyDown(KeyCode.Escape) || GameStateController.isShortcutKeyDown(GameStateController._pauseKey)) && (0 == GameStateController.getPausesInStackCount()))
        {
            ModalManager.unsetModal();
            return GameStateTarget.Game;
        }
        else
            //pressing "validate" or "cancel" buttons
        {
            if(null != _instance._currentModalElement)
            {
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    keyPressedEventConsumed = manageValidateButton();
                }
                else if(Input.GetKeyDown(KeyCode.Escape))
                {   
                    keyPressedEventConsumed = manageCancelButton();
                }

                if(!keyPressedEventConsumed)
                {
                    return manageInfoWindows();
                }
                else
                {
                    //keyPressedEventConsumed but no specific game state was specified as target
                    return GameStateTarget.NoTarget;
                }
            }
        }
        return GameStateTarget.NoAction;
    }

    private static GameStateTarget manageInfoWindows()
    {
        return InfoWindowManager.manageKeyPresses();
    }
    
    private static bool manageValidateButton()
    {
        Debug.LogWarning("ModalManager::manageValidateButton()");
        return manageModalButton(_instance._validateButton, _instance._validateButtonClass);
    }
    
    private static bool manageCancelButton()
    {
        Debug.LogWarning("ModalManager::manageCancelButton()");
        return manageModalButton(_instance._cancelButton, _instance._cancelButtonClass);
    }

    private static bool manageModalButton(GameObject modalButton, string modalButtonClass)
    {        
        Debug.LogWarning(string.Format("ModalManager::manageModalButton({0}, {1})", modalButton.name, modalButtonClass));
        if(modalButton && modalButton.activeInHierarchy)
        {
            //TODO check need for getting component with class name "modalButtonClass"
            ModalButton button = (ModalButton)modalButton.GetComponent(modalButtonClass);
            button.press();
            return true;
        }
        return false;
    }

}

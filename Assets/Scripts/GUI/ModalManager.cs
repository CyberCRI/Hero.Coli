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

  //pointers to buttons on the generic modal window
  public GameObject genericValidateButton;
  public GameObject genericCenteredValidateButton;
  public GameObject genericCancelButton;

  //pointers to whatever buttons are used to validate/cancel
  //(nb: no choice on cancel buttons yet)
  private GameObject _validateButton;
  private GameObject _cancelButton;
  //names of classes of aforementioned buttons
  private string _validateButtonClass;
  private string _cancelButtonClass;  

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

    //sets the current component of the cancel button to the one class given as string parameter
    private static void prepareGenericCancelButton(string cancelClass)
    {
        Debug.LogWarning(string.Format("ModalManager::prepareGenericCancelButton({0})", cancelClass));

        string usedCancelClass = string.IsNullOrEmpty(cancelClass)?_cancelModalClassName:cancelClass;

        if(usedCancelClass != _instance._cancelButtonClass) {
            Debug.LogWarning(string.Format("ModalManager::prepareGenericCancelButton({0}) = (usedCancelClass={1}!=_instance._cancelButtonClass={2})",
                                           cancelClass, 
                                           usedCancelClass, 
                                           _instance._cancelButtonClass));
            Object.Destroy(_instance.genericCancelButton.GetComponent(_instance._cancelButtonClass));
            _instance._cancelButtonClass = usedCancelClass;
            _instance.genericCancelButton.AddComponent(usedCancelClass);
        }
        else
        {
            Debug.LogWarning(string.Format("ModalManager::prepareGenericCancelButton({0}) = (usedCancelClass={1}==_instance._cancelButtonClass)",
                                           cancelClass, usedCancelClass));
        }
        
        //defensive programming
        Debug.LogWarning(string.Format("ModalManager::prepareGenericCancelButton({0}) - set _instance._cancelButton to {1}", cancelClass, _instance.genericCancelButton.name));
        _instance._cancelButton = _instance.genericCancelButton;
    }

    //sets the cancel button to its initial state
    private static void resetGenericCancelButton()
    {
        prepareGenericCancelButton(_cancelModalClassName);
    }

  private static bool fillInFieldsFromCode(string code)
  {
        Debug.LogWarning("ModalManager::fillInFieldsFromCode("+code+")");
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
                    Debug.LogWarning("ModalManager::fillInFieldsFromCode - needs cancel button");

                    //affect class of action after validation
                    _instance._validateButtonClass = info._next;

                    //choose validation button
                    _instance._validateButton = _instance.genericValidateButton;

                    //set active buttons according to 'validate/cancel' pattern
                    _instance.genericValidateButton.gameObject.SetActive(true);
                    _instance.genericCenteredValidateButton.gameObject.SetActive(false);
                    _instance.genericCancelButton.gameObject.SetActive(true);

                    //add class for action after validation
                    _instance.genericValidateButton.gameObject.AddComponent(_instance._validateButtonClass);

                    //update cancel button component if necessary
                    prepareGenericCancelButton(info._cancel);

                    Debug.LogWarning("ModalManager::fillInFieldsFromCode - needs cancel button - DONE");
                }
                else
                {      
                    Debug.LogWarning("ModalManager::fillInFieldsFromCode - doesn't need cancel button");

                    //affect class of action after validation
                    _instance._validateButtonClass = info._next;

                    //choose validation button button
                    _instance._validateButton = _instance.genericCenteredValidateButton;
                    
                    //set active buttons according to 'validate' pattern
                    _instance.genericValidateButton.gameObject.SetActive(false);
                    _instance.genericCenteredValidateButton.gameObject.SetActive(true);
                    _instance.genericCancelButton.gameObject.SetActive(false);
                    
                    //add class for action after validation
                    _instance.genericCenteredValidateButton.gameObject.AddComponent(_instance._validateButtonClass);

                    //reset cancel button - isActive is used to test whether the button should respond to keys or not
                    resetGenericCancelButton();

                    Debug.LogWarning("ModalManager::fillInFieldsFromCode - doesn't need cancel button - DONE");
                }
            }
            else
            {
                Debug.LogWarning("ModalManager::fillInFieldsFromCode("+code+") - no next action");
                return false;
            }
      
            Debug.LogWarning("ModalManager::fillInFieldsFromCode("+code+") - DONE");
            return true;
        }
        else
        {
            Debug.LogWarning("ModalManager::fillInFieldsFromCode("+code+") - no info");
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

            /*
            string validateButtonDebug = null==validateButton?"null":validateButton.name;
            string validateButtonClassDebug = string.IsNullOrEmpty(validateButtonClass)?"null":validateButtonClass;
            string cancelButtonDebug = null==cancelButton?"null":cancelButton.name;
            string cancelButtonClassDebug = string.IsNullOrEmpty(cancelButtonClass)?"null":cancelButtonClass;
            */

            Debug.LogWarning(string.Format("ModalManager::setModal({0},{1},{2},{3},{4},{5}) - set _instance._cancelButton to {6}", 
                                           guiComponent.name,
                                           lockPause,
                                           validateButton,
                                           validateButtonClass,
                                           cancelButton,
                                           cancelButtonClass,
                                           cancelButton
                                           ));
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
        Debug.LogWarning("ModalManager::setModal("+code+")");
    if(null != _instance.genericModalWindow && fillInFieldsFromCode(code))
    {
            Debug.LogWarning("ModalManager::setModal("+code+") - setup ok");
        setModal(_instance.genericModalWindow,
                     lockPause,
                     _instance._validateButton,
                     _instance._validateButtonClass,
                     _instance._cancelButton,
                     _instance._cancelButtonClass
                     );

        //initialize cancel button
        //_instance._cancelButton = _instance.genericCancelButton;
        //_instance._cancelButtonClass = _instance._cancelModalClassName;
        
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
                    Debug.LogWarning("ModalManager::manageKeyPresses no need for manageInfoWindows()");
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
        //string cancelButtonDebug = null == _instance._cancelButton?"null":_instance._cancelButton;
        Debug.LogWarning(string.Format("ModalManager::manageCancelButton() with cb={0} and cbc={1}", _instance._cancelButton, _instance._cancelButtonClass));
        return manageModalButton(_instance._cancelButton, _instance._cancelButtonClass);
    }

    private static bool manageModalButton(GameObject modalButton, string modalButtonClass)
    {        
        string buttonName = null==modalButton?"null":modalButton.name;
        Debug.LogWarning(string.Format("ModalManager::manageModalButton({0}, {1})", buttonName, modalButtonClass));
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

// #define QUICKTEST

using UnityEngine;
using System.Collections.Generic;

/**
 * Helps manage modal windows
 * TODO: merge with InfoWindowManager
 */
public class ModalManager : MonoBehaviour
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "ModalManager";
    private static ModalManager _instance;
    public static ModalManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("ModalManager get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<ModalManager>();
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;
    private void initializeIfNecessary()
    {
        if (!_initialized)
        {
            loadDataIntoDico(inputFiles, _loadedModalWindows);
            _initialized = true;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public string[] inputFiles;
    public float foregroundZ;
    public GameObject modalBackground;
    public GameObject genericModalWindow;

    public UILocalize titleLabel;
    public UILocalize explanationLabel;

    private UISprite _infoSprite;

    //pointers to buttons on the generic modal window
    public GameObject genericValidateButton;
    public GameObject genericCenteredValidateButton;
    public GameObject genericCancelButton;

    private static float _maxYScale;
    private static float _maxXScale;
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
    private const string _genericPrefix = "MODAL.";
    private const string _genericTitle = ".TITLE";
    private const string _genericExplanation = ".EXPLANATION";
    // private const string _quitModalClassName = "QuitModalWindow" + completeNameSuffix;
    // private const string _restartModalClassName = "ModalRestart" + completeNameSuffix;
    //the class of the component attached to the cancel button of the ModalWindow
    private const string _cancelModalClassName = "CancelModal" + completeNameSuffix;

    //TODO find a (more) robust method
    private const string completeNameSuffix = ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

    private static StandardInfoWindowInfo retrieveFromDico(string code)
    {
        StandardInfoWindowInfo info;
        //TODO set case-insensitive
        if (!_instance._loadedModalWindows.TryGetValue(code, out info))
        {
            Debug.LogWarning("ModalManager retrieveFromDico(" + code + ") failed");
            info = null;
        }
        return info;
    }

    private void loadDataIntoDico(string[] inputFiles, Dictionary<string, StandardInfoWindowInfo> dico)
    {
        InfoWindowLoader iwLoader = new InfoWindowLoader();

        string loadedFiles = "";

        foreach (string file in inputFiles)
        {
            foreach (StandardInfoWindowInfo info in iwLoader.loadInfoFromFile(file))
            {
                info._next = string.IsNullOrEmpty(info._next) ? info._next : info._next + completeNameSuffix;
                info._cancel = string.IsNullOrEmpty(info._cancel) ? info._cancel : info._cancel + completeNameSuffix;
                // Debug.Log(this.GetType() + " loadDataIntoDico _cancel=" + info._cancel);
                dico.Add(info._code, info);
            }
            if (!string.IsNullOrEmpty(loadedFiles))
            {
                loadedFiles += ", ";
            }
            loadedFiles += file;
        }

        // Debug.Log(this.GetType() + " loadDataIntoDico loaded "+loadedFiles);
    }

    public static bool isDisplayed()
    {
        return (null != _instance) && (null != _instance._currentModalElement);
    }

    // no use for a 'Cancel' button currently
    private static bool needsCancelButton(string validateButtonClassName, string cancelButtonClassName)
    {
        // Debug.Log("ModalManager needsCancelButton(" + validateButtonClassName + "," + cancelButtonClassName + ")");
        // return ((validateButtonClassName == _restartModalClassName) || !string.IsNullOrEmpty(cancelButtonClassName));
        return false;
    }

    private static void removeAllModalButtonClasses(GameObject button)
    {
        ModalButton[] components = button.GetComponents<ModalButton>();
        foreach (ModalButton component in components)
        {
            Object.Destroy(component);
        }
    }

    //sets the current component of the cancel button to the one class given as string parameter
    private static void prepareGenericCancelButton(string cancelClass)
    {
        // Debug.Log(string.Format("ModalManager prepareGenericCancelButton({0})", cancelClass));

        string usedCancelClass = string.IsNullOrEmpty(cancelClass) ? _cancelModalClassName : cancelClass;

        // Debug.Log(string.Format("ModalManager prepareGenericCancelButton({0}) = (usedCancelClass={1}; _instance._cancelButtonClass={2})",
        //                                    cancelClass, 
        //                                    usedCancelClass, 
        //                                    _instance._cancelButtonClass));

        _instance._cancelButtonClass = usedCancelClass;
        safeAddComponent(_instance.genericCancelButton, usedCancelClass);

        //defensive programming
        // Debug.Log(string.Format("ModalManager prepareGenericCancelButton({0}) - set _instance._cancelButton to {1}", cancelClass, _instance.genericCancelButton.name));
        _instance._cancelButton = _instance.genericCancelButton;
    }

    //sets the cancel button to its initial state
    private static void resetGenericCancelButton()
    {
        // Debug.Log("ModalManager resetGenericCancelButton");
        prepareGenericCancelButton(_cancelModalClassName);
    }

    public void setInfoSprite(UISprite infoSprite)
    {
        if(null != infoSprite)
        {
            _infoSprite = infoSprite;
            _maxXScale = infoSprite.transform.localScale.x;
            _maxYScale = infoSprite.transform.localScale.y;
        }
    }

    private static void resetRatio()
    {
        // Debug.Log("ModalManager resetRatio "
        // + " scale=" + _instance._infoSprite.transform.localScale
        // + " _maxXScale=" + _maxXScale
        // + " _maxYScale=" + _maxYScale        
        // );
        if (
            (_instance._infoSprite.transform.localScale.x > _maxXScale)
            ||
            (_instance._infoSprite.transform.localScale.y > _maxYScale)
        )
        {
            float ratioX = _maxXScale / _instance._infoSprite.transform.localScale.x;
            float ratioY = _maxYScale / _instance._infoSprite.transform.localScale.y;
            float ratio = ratioX < ratioY ? ratioX : ratioY;

            // Debug.Log("ModalManager resetRatio ratioX=" + ratioX + " ratioY=" + ratioY + " ratio=" + ratio);

            Vector3 newScale = new Vector3(
                ratio * _instance._infoSprite.transform.localScale.x,
                ratio * _instance._infoSprite.transform.localScale.y,
                _instance._infoSprite.transform.localScale.z
            );

            // Debug.Log("ModalManager resetRatio newScale=" + newScale);

            _instance._infoSprite.transform.localScale = newScale;
        }
    }

    private static bool fillInFieldsFromCode(string code)
    {
        // Debug.Log("ModalManager fillInFieldsFromCode("+code+")");
        StandardInfoWindowInfo info = retrieveFromDico(code);

        if (null != info)
        {
            string generic = _genericPrefix + code.ToUpper();

            _instance.titleLabel.key = generic + _genericTitle;
            _instance._infoSprite.spriteName = info._texture;
            _instance._infoSprite.MakePixelPerfect();
            resetRatio();
            _instance._infoSprite.transform.localPosition = new Vector3(_instance._infoSprite.transform.localPosition.x, _instance._infoSprite.transform.localPosition.y, -1);
            _instance.explanationLabel.key = generic + _genericExplanation;

            if (!string.IsNullOrEmpty(info._next))
            {
                //affect class of action after validation
                _instance._validateButtonClass = info._next;

                bool doesNeedsCancelButton = needsCancelButton(info._next, info._cancel);

                //set active buttons according to 'validate/cancel' pattern
                _instance.genericValidateButton.gameObject.SetActive(doesNeedsCancelButton);
                _instance.genericCenteredValidateButton.gameObject.SetActive(!doesNeedsCancelButton);
                _instance.genericCancelButton.gameObject.SetActive(doesNeedsCancelButton);

                if (doesNeedsCancelButton)
                {
                    //choose validation button
                    _instance._validateButton = _instance.genericValidateButton;


                    //update cancel button component if necessary
                    prepareGenericCancelButton(info._cancel);
                }
                else
                {
                    //choose validation button button
                    _instance._validateButton = _instance.genericCenteredValidateButton;

                    //reset cancel button - isActive is used to test whether the button should respond to keys or not
                    resetGenericCancelButton();
                }

                //add class for action after validation
                safeAddComponent(_instance._validateButton, _instance._validateButtonClass);
            }
            else
            {
                return false;
            }

            return true;
        }
        else
        {
            Debug.LogWarning("ModalManager fillInFieldsFromCode(" + code + ") - no info");
            return false;
        }
    }

    private static void safeAddComponent(GameObject button, string modalClass)
    {
        // Debug.Log(string.Format("ModalManager safeAddComponent({0},{1})", button, modalClass));
        removeAllModalButtonClasses(button);

        System.Type modalType = System.Type.GetType(modalClass);
        if (null != modalType)
        {
            button.AddComponent(modalType);
        }
        else
        {
            modalType = System.Type.GetType(modalClass + completeNameSuffix);
            if (null != modalType)
            {
                button.AddComponent(modalType);
            }
        }

        //UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(button, "Assets/Scripts/GUI/ModalManager.cs", modalClass);
        //button.AddComponent(Type.GetType(modalClass, modalClass));
        //button.AddComponent<modalClass>();
        //UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(button, "Assets/Scripts/GUI/ModalManager.cs (202,9)", modalClass);
    }

    public static bool isCancelButtonActive()
    {
        return (null != _instance._cancelButton) && _instance._cancelButton.gameObject.activeInHierarchy;
    }

    private static void prepareButton(GameObject button, string modalButtonClass)
    {
        // Debug.Log(string.Format("ModalManager prepareButton({0},{1})", button, modalButtonClass));
        if (null != button)
        {
            if (!string.IsNullOrEmpty(modalButtonClass))
            {
                //if(null==(ModalButton)button.GetComponent(modalButtonClass)) {
                if (null == button.GetComponent(System.Type.GetType(modalButtonClass)))
                {
                    safeAddComponent(button, modalButtonClass);
                }
                //if(null==(ModalButton)button.GetComponent(modalButtonClass)) {
                if (null == button.GetComponent(System.Type.GetType(modalButtonClass)))
                {
                    Debug.LogError(string.Format("ModalManager prepareButton error: couldn't get ModalButton component from {0} with class={1}", button, modalButtonClass));
                }
                else
                {
                    // Debug.Log(string.Format("ModalManager prepareButton successful adding of class {0}", modalButtonClass));
                }

            }
            else
            {
                Debug.LogWarning("ModalManager prepareButton string.IsNullOrEmpty(modalButtonClass)");
            }
        }
        else
        {
            Debug.LogWarning("ModalManager prepareButton null==button");
        }
    }

    private static void setValidateButton(GameObject vb, string vbClass)
    {
        // Debug.Log(string.Format("ModalManager setValidateButton({0},{1})", vb, vbClass));
        if (null != vb && !string.IsNullOrEmpty(vbClass))
        {
            prepareButton(vb, vbClass);
        }
        _instance._validateButton = vb;
        _instance._validateButtonClass = vbClass;
    }

    private static void setCancelButton(GameObject cb, string cbClass)
    {
        // Debug.Log(string.Format("ModalManager setCancelButton({0},{1})", cb, cbClass));
        if (null != cb && !string.IsNullOrEmpty(cbClass))
        {
            prepareButton(cb, cbClass);
        }
        _instance._cancelButton = cb;
        _instance._cancelButtonClass = cbClass;
    }

    //Sets a guiComponent as Modal
    //Can handle custom validate and cancel buttons
    //as long as they inherit ModalButton
    //Adds ModalButton components to GameObjects if they don't have them beforehand
    public static void setModal(GameObject guiComponent,
                                bool lockPause = true,
                                GameObject validateButton = null, string validateButtonClass = null,
                                GameObject cancelButton = null, string cancelButtonClass = null
                                )
    {
        //hide previous modal component
        if (null != _instance._currentModalElement)
        {
            // Debug.Log(string.Format("ModalManager setModal there was previous modal element {0}!", _instance._currentModalElement));
            unsetModal(true);
        }

        // Debug.Log(string.Format("ModalManager setModal({0},{1},{2},{3},{4},{5}) - set _instance._cancelButton to {6}", 
        //                                guiComponent,
        //                                lockPause,
        //                                validateButton,
        //                                validateButtonClass,
        //                                cancelButton,
        //                                cancelButtonClass,
        //                                cancelButton
        //                                ));
        if (null != guiComponent)
        {
            Vector3 position = guiComponent.transform.localPosition;
            _instance._previousZ = position.z;
            guiComponent.transform.localPosition = new Vector3(position.x, position.y, _instance.foregroundZ);
            _instance._currentModalElement = guiComponent;

            _instance._currentModalElement.SetActive(true);
            _instance.modalBackground.SetActive(true);

            setValidateButton(validateButton, validateButtonClass);
            setCancelButton(cancelButton, cancelButtonClass);

            if (lockPause)
            {
                GameStateController.get().tryLockPause();
            }
        }
    }

    public static bool setModal(string code, bool lockPause = true)
    {
#if QUICKTEST
        return true;
#endif
        // Debug.Log("ModalManager setModal("+code+")");
        if (null != _instance.genericModalWindow && fillInFieldsFromCode(code))
        {
            setModal(_instance.genericModalWindow,
                         lockPause,
                         _instance._validateButton,
                         _instance._validateButtonClass,
                         _instance._cancelButton,
                         _instance._cancelButtonClass
                         );

            return true;
        }
        else
        {
            Debug.LogWarning("ModalManager displayInfoWindow(" + code + ") failed");
            return false;
        }
    }

    //TODO manage stack of modal elements
    public static void unsetModal(bool backgroundActive = false)
    {
        if (null != _instance._currentModalElement)
        {

            Vector3 position = _instance._currentModalElement.transform.localPosition;
            _instance._currentModalElement.transform.localPosition = new Vector3(position.x, position.y, _instance._previousZ);

            if (!string.IsNullOrEmpty(_instance._validateButtonClass))
            {
                resetGenericValidateButtons();
                resetGenericCancelButton();
            }

            _instance._currentModalElement.SetActive(false);
            _instance.modalBackground.SetActive(backgroundActive);

            _instance._currentModalElement = null;
        }
    }

    public static void resetGenericValidateButtons()
    {
        Object.Destroy(_instance.genericValidateButton.GetComponent(System.Type.GetType(_instance._validateButtonClass)));
        Object.Destroy(_instance.genericCenteredValidateButton.GetComponent(System.Type.GetType(_instance._validateButtonClass)));
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
        if (Input.anyKeyDown)
        {
            //equivalent to: "consumed action", "did something", and so on
            bool keyPressedEventConsumed = false;

            //getting out of Pause
            if ((Input.GetKeyDown(KeyCode.Escape) || GameStateController.isShortcutKeyDown(GameStateController._pauseKey)) && (0 == GameStateController.getPausesInStackCount()))
            {
                // Debug.Log("ModalManager getting out of pause");
                ModalManager.unsetModal();
                return GameStateTarget.Game;
            }
            else
            {
                //pressing "validate" or "cancel" buttons
                if (null != _instance._currentModalElement)
                {
                    //Modal windows key presses
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
                    {
                        keyPressedEventConsumed = manageValidateButton();
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (isCancelButtonActive())
                        {
                            keyPressedEventConsumed = manageCancelButton();
                        }
                        else
                        {
                            keyPressedEventConsumed = manageValidateButton();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Space) && (!isCancelButtonActive()))
                    {
                        keyPressedEventConsumed = manageValidateButton();
                    }

                    if (!keyPressedEventConsumed)
                    {
                        //no action was performed yet
                        if (InfoWindowManager.hasActivePanel())
                        {
                            //info windows key presses
                            return manageInfoWindows();
                        }
                        else
                        {
                            //no action was performed at all
                            return GameStateTarget.NoAction;
                        }
                    }
                    else
                    {
                        // Debug.Log("ModalManager manageKeyPresses no need for manageInfoWindows()");
                        //keyPressedEventConsumed but no specific game state was specified as target
                        return GameStateTarget.NoTarget;
                    }
                }
                else
                {
                    // Debug.Log("ModalManager manageKeyPresses no current modal");
                }
            }
        }
        return GameStateTarget.NoAction;
    }

    private static GameStateTarget manageInfoWindows()
    {
        // Debug.Log("ModalManager manageInfoWindows");
        return InfoWindowManager.manageKeyPresses();
    }

    private static bool manageValidateButton()
    {
        // Debug.Log(string.Format("ModalManager manageValidateButton() with vb={0} and vbc={1}", _instance._validateButton, _instance._validateButtonClass));
        return manageModalButton(_instance._validateButton, _instance._validateButtonClass);
    }

    private static bool manageCancelButton()
    {
        //string cancelButtonDebug = null == _instance._cancelButton?"null":_instance._cancelButton;
        // Debug.Log(string.Format("ModalManager manageCancelButton() with cb={0} and cbc={1}", _instance._cancelButton, _instance._cancelButtonClass));
        return manageModalButton(_instance._cancelButton, _instance._cancelButtonClass);
    }

    private static bool manageModalButton(GameObject modalButton, string modalButtonClass)
    {
        // Debug.Log(string.Format("ModalManager manageModalButton({0}, {1})", modalButton, modalButtonClass));
        if (null != modalButton && modalButton.activeInHierarchy)
        {
            //TODO check need for getting component with class name "modalButtonClass"
            ModalButton button = (ModalButton)modalButton.GetComponent(System.Type.GetType(modalButtonClass));
            if (null != button)
            {
                button.press();
                return true;
            }
            else
            {
                Debug.LogWarning(string.Format("ModalManager manageModalButton({0}, {1}) - button does not have required component!", modalButton, modalButtonClass));
            }
        }
        // Debug.Log(string.Format("ModalManager manageModalButton({0}, {1}) returns false", modalButton, modalButtonClass));
        return false;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class EquipedDisplayedDeviceWithMolecules : MonoBehaviour
{
    [SerializeField]
    private GameObject _displayedDeviceDummy;
    private GameObject _displayedDevice;
    private DisplayedDevice _displayedDeviceScript;
    private List<GameObject> graphicalComponents = new List<GameObject>();

    public Device device;
    public UILabel namesLabel;
    public UILabel valuesLabel;
    public GameObject background, deviceIcon, level, moleculeName;

    private DisplayedMolecule _displayedMolecule;

    public void initialize(DisplayedDevice displayedDeviceScript)
    {
        if (null != _displayedDeviceDummy)
        {
            _displayedDeviceDummy.SetActive(false);
            
            _displayedDevice        = displayedDeviceScript.gameObject;
            _displayedDeviceScript  = displayedDeviceScript;
            device                  = displayedDeviceScript._device;
            
            setDisplayedDevice();
            
            background      = displayedDeviceScript.deviceBackgroundSprite.gameObject;
            deviceIcon      = displayedDeviceScript._sprite.gameObject;
            level           = displayedDeviceScript.levelSprite.gameObject;
            moleculeName    = displayedDeviceScript.moleculeOverlay.gameObject;
            
            graphicalComponents = new List<GameObject>{namesLabel.gameObject, valuesLabel.gameObject, background, deviceIcon, level, moleculeName};
            
            doOnGameObjects(setParent);
            
            moleculeName.transform.position = new Vector3(moleculeName.transform.position.x, moleculeName.transform.position.y, -0.3f); 
        }
        else
        {
            Logger.Log("EquipedDisplayedDeviceWithMolecules::initialize has null parameter", Logger.Level.WARN);
        }
    }

    private delegate void GOEditor(GameObject go);
    private void doOnGameObjects(GOEditor edit)
    {
        foreach (GameObject go in graphicalComponents)
        {
            if (null != go)
            {
                edit(go);
            }
        }
    }
    private void setParent(GameObject go)
    {
        go.transform.parent = this.transform.parent;
    }

    private void moveGameObject(Vector3 delta, GameObject go)
    {
        go.transform.localPosition = go.transform.localPosition + delta;
    }

    private GOEditor moveGameObject(Vector3 delta)
    {
        return (GameObject go) => moveGameObject(delta, go);
    }

    public void setLocalPosition(Vector3 newLocalPosition)
    {
        //TODO clean
        Vector3 delta = newLocalPosition - this.transform.localPosition;

        this.transform.localPosition = newLocalPosition;
        doOnGameObjects(moveGameObject(delta));
    }

    public void setDisplayedDevice()
    {
        _displayedDevice.transform.parent = this.transform;
        _displayedDevice.transform.localPosition = _displayedDeviceDummy.transform.localPosition;
        _displayedDevice.transform.localScale = new Vector3(1f, 1f, 0);
        _displayedDevice.transform.localRotation = _displayedDeviceDummy.transform.localRotation;
    }

  //TODO allow multiple protein management
  public void addDisplayedMolecule(DisplayedMolecule molecule)
  {
    _displayedMolecule = molecule;
    molecule.setDisplayType(DisplayedMolecule.DisplayType.DEVICEMOLECULELIST);
  }

  public DisplayedMolecule getDisplayedMolecule()
  {
    return _displayedMolecule;
  }

  //TODO implement & allow multiple protein management
  public void removeDisplayedMolecule(string molecule)
  {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::removedDisplayedMolecule not implemented", Logger.Level.WARN);
  }
  
  void OnEnable() {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::OnEnable", Logger.Level.INFO);
    //background.SetActive(true);
  }
  
  void OnDisable() {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::OnDisable", Logger.Level.INFO);
    //background.SetActive(false);
  }
  
  void OnDestroy()
  {
      doOnGameObjects(Destroy);
  }
  
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("EquipedDisplayedDeviceWithMolecules::OnPress() "+getDebugInfos(), Logger.Level.INFO);
      if(device == null)
      {
        Logger.Log("EquipedDisplayedDeviceWithMolecules::OnPress _device == null", Logger.Level.INFO);
        return;
      }
    }
  }
  
  // Use this for initialization
  void Start () {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::Start", Logger.Level.INFO);
    
    graphicalComponents = new List<GameObject>{namesLabel.gameObject, valuesLabel.gameObject, background, deviceIcon, level, moleculeName};
    
    namesLabel.text = "";
    valuesLabel.text = "";
  }

  void Update()
  {
    if(null != _displayedMolecule)
    {
      namesLabel.text = _displayedMolecule.getRealName();
      valuesLabel.text = _displayedMolecule.getVal();
    }
  }

  public void releaseMoleculeDisplay()
  {
    if(null != _displayedMolecule)
    {
      _displayedMolecule.setDisplayType(DisplayedMolecule.DisplayType.MOLECULELIST);
    }
  }

  protected string getDebugInfos() {
        return "EquipedDisplayedDeviceWithMolecules inner device="+device+", inner displayedDeviceScript type="+_displayedDeviceScript+", time="+Time.realtimeSinceStartup;
  }
}
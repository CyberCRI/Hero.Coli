using UnityEngine;

public class EquipedDisplayedDeviceWithMolecules : MonoBehaviour
{
    [SerializeField]
    private GameObject _displayedDevice;
    private DisplayedDevice _displayedDeviceScript;

    public Device device;
    public UILabel namesLabel;
    public UILabel valuesLabel;

    private DisplayedMolecule _displayedMolecule;

    public void initialize(DisplayedDevice displayedDeviceScript)
    {
        if (null != _displayedDevice)
        {
            _displayedDevice.SetActive(true);
            _displayedDeviceScript = _displayedDevice.GetComponent<DisplayedDevice>();
            _displayedDeviceScript.Initialize(displayedDeviceScript._device);
            device = displayedDeviceScript._device;
        }
        else
        {
            Debug.LogWarning (this.GetType() + " initialize has null parameter");
        }
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
    Debug.LogWarning (this.GetType() + " removedDisplayedMolecule not implemented");
  }
  
  void OnEnable() {
    Debug.Log(this.GetType() + " OnEnable");
    //background.SetActive(true);
  }
  
  void OnDisable() {
    Debug.Log(this.GetType() + " OnDisable");
    //background.SetActive(false);
  }
  
  void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " OnPress() "+getDebugInfos());
      if(device == null)
      {
        Debug.Log(this.GetType() + " OnPress _device == null");
        return;
      }
    }
  }
  
  // Use this for initialization
  void Start () {
    Debug.Log(this.GetType() + " Start");
    
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
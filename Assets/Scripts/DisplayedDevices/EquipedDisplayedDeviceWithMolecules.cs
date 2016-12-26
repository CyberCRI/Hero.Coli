using UnityEngine;

public class EquipedDisplayedDeviceWithMolecules : MonoBehaviour
{
    // used only to position the externally created displayed device
    [SerializeField]
    private GameObject _displayedDevice;

    public Device device;
    public UILabel namesLabel;
    public UILabel valuesLabel;

    private DisplayedMolecule _displayedMolecule;

    public void initialize(DisplayedDevice displayedDeviceScript)
    {
        // Debug.Log(this.GetType() + " initialize starts");
        if (null != _displayedDevice)
        {
            // Debug.Log(this.GetType() + " initialize null != _displayedDevice");

            GameObject importedDisplayeDevice = displayedDeviceScript.gameObject;
            importedDisplayeDevice.transform.parent = _displayedDevice.transform.parent;
            importedDisplayeDevice.transform.localPosition = _displayedDevice.transform.localPosition;
            importedDisplayeDevice.transform.localScale = _displayedDevice.transform.localScale;
            importedDisplayeDevice.SetActive(true);
            // Debug.Log(this.GetType() + " initialize set imported");

            Destroy(_displayedDevice);
            // Debug.Log(this.GetType() + " initialize destroyed");

            _displayedDevice = importedDisplayeDevice;
            device = displayedDeviceScript._device;
            // displayedDeviceScript.Initialize(device);
            // Debug.Log(this.GetType() + " initialize done");     
        }
        else
        {
            Debug.LogWarning(this.GetType() + " initialize has null parameter");
        }
    }

    //TODO allow multiple protein management
    public void addDisplayedMolecule(DisplayedMolecule molecule)
    {
        _displayedMolecule = molecule;
        molecule.setDisplayType(DisplayedMolecule.DisplayType.DEVICEMOLECULELIST);

        if (null != _displayedMolecule)
        {
            string previousName = namesLabel.text;
            namesLabel.text = _displayedMolecule.getRealName();
            valuesLabel.text = _displayedMolecule.getVal();
            // Debug.Log(this.GetType() + " addDisplayedMolecule changed name from old=" + previousName + " to new " + namesLabel.text);
        }
    }

    public DisplayedMolecule getDisplayedMolecule()
    {
        return _displayedMolecule;
    }

    //TODO implement & allow multiple protein management
    public void removeDisplayedMolecule(string molecule)
    {
        Debug.LogWarning(this.GetType() + " removedDisplayedMolecule not implemented");
    }

    /*
        void OnEnable()
        {
            // Debug.Log(this.GetType() + " OnEnable");
            //background.SetActive(true);
        }

        void OnDisable()
        {
            // Debug.Log(this.GetType() + " OnDisable");
            //background.SetActive(false);
        }

        void OnPress(bool isPressed)
        {
            if (isPressed)
            {
                // Debug.Log(this.GetType() + " OnPress() "+getDebugInfos());
                if (device == null)
                {
                    // Debug.Log(this.GetType() + " OnPress _device == null");
                    return;
                }
            }
        }
        */

    // Use this for initialization
    void Start()
    {
        // Debug.Log(this.GetType() + " Start as " + namesLabel.text);

        //string previousName = namesLabel.text;
        // namesLabel.text = "";
        // valuesLabel.text = "";
        // Debug.Log(this.GetType() + " Start changed name from old=" + previousName + " to new " + namesLabel.text);
    }

    void Update()
    {
        if (null != _displayedMolecule)
        {
            valuesLabel.text = _displayedMolecule.getVal();
        }
    }

    public void releaseMoleculeDisplay()
    {
        if (null != _displayedMolecule)
        {
            _displayedMolecule.setDisplayType(DisplayedMolecule.DisplayType.MOLECULELIST);
        }
    }

    protected string getDebugInfos()
    {
        return "EquipedDisplayedDeviceWithMolecules inner device=" + device + ", inner displayedDeviceScript type=" + _displayedDevice.GetComponent<DisplayedDevice>() + ", time=" + Time.realtimeSinceStartup;
    }

    public void onLanguageChanged()
    {
        namesLabel.text = _displayedMolecule.getRealName();
    }
}
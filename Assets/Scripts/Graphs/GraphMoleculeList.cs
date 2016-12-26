using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphMoleculeList : MonoBehaviour, ILocalizable
{
    // compulsory fields
    private ReactionEngine _reactionEngine;
    // this medium id is 1 for the Cell graph and changes for the external medium graph
    [SerializeField]
    private int mediumId;
    [SerializeField]
    private UILabel namesLabel;
    [SerializeField]
    private UILabel valuesLabel;
    [SerializeField]
    private bool displayAll;

    // optional fields
    // grid of EquipedDisplayedDeviceWithMolecules of the scroll view
    private Transform _eddwmGridTransform;
    [SerializeField]
    private UIGrid _eddwmGridComponent;
    [SerializeField]
    private UITable _moleculesAndDevicesTableComponent;

    // list of all displayed molecules, either in molecule list or in eddwms
    private LinkedList<DisplayedMolecule> _displayedMolecules = new LinkedList<DisplayedMolecule>();

    // list of eddwms
    private List<EquipedDisplayedDeviceWithMolecules> _equipedDevices = new List<EquipedDisplayedDeviceWithMolecules>();

    // when molecule or eddwm added, removed
    // ie _molecules or _displayedMolecules or _equipedDevices edited
    private bool _isMoleculesEdited = false;

    // when eddwm added, removed
    // ie _equipedDevices edited
    private bool _isEDDWMsEdited = false;

    private LinkedList<DisplayedMolecule> _toRemove = new LinkedList<DisplayedMolecule>();

    private string _realName, _codeName, _namesToDisplay, _valuesToDisplay;
    private float _concentration;
    private Molecule _molecule;
    private DisplayedMolecule _displayedMolecule, _createdDisplayedMolecule;
    private ArrayList _moleculesArrayList;
    private Dictionary<Molecule, DisplayedMolecule> _molecules = null;
    private List<KeyValuePair<Molecule, DisplayedMolecule>> _toAdd = new List<KeyValuePair<Molecule, DisplayedMolecule>>();
    private List<EquipedDisplayedDeviceWithMolecules> _containers;
    private List<Molecule> _toReset = new List<Molecule>();

    public bool positionDeviceAndMoleculeComponentsNow = false;
    public bool positionDeviceAndMoleculeComponentsOnNextFrame = false;
    public bool positionDeviceAndMoleculeComponentsOnEnable = false;
    public bool useTableFirst = false;

    public void setLanguageChanged(bool changed)
    {
        Debug.Log(this.GetType() + " setLanguageChanged");
        _isMoleculesEdited |= changed;
        Debug.Log(this.GetType() + " setLanguageChanged => _isMoleculesEdited = " + _isMoleculesEdited);
    }
    public void setDisplayTypeChanged(bool changed)
    {
        Debug.Log(this.GetType() + " setDisplayTypeChanged");
        _isMoleculesEdited |= changed;
        Debug.Log(this.GetType() + " setDisplayTypeChanged => _isMoleculesEdited = " + _isMoleculesEdited);
    }

    public void setMediumId(int newMediumId)
    {
        mediumId = newMediumId;
        initializeMolecules();
    }

    void Awake()
    {
        Debug.Log(this.GetType() + " Awake");

        namesLabel.text = "";
        valuesLabel.text = "";

        if (null != _eddwmGridComponent)
        {
            Debug.Log(this.GetType() + " destroying all of eddwm's children");
            _eddwmGridTransform = _eddwmGridComponent.transform;
            for (int index = 0; index < _eddwmGridTransform.childCount; index++)
            {
                Destroy(_eddwmGridTransform.GetChild(index).gameObject);
            }
        }
    }

    void Start()
    {
        Debug.Log(this.GetType() + " Start");

        _reactionEngine = ReactionEngine.get();
        I18n.register(this);
    }

    private void resetMoleculeList()
    {
        // Debug.Log(this.GetType() + " resetMoleculeList");
        foreach (DisplayedMolecule molecule in _displayedMolecules)
        {
            molecule.reset();
        }
    }

    private void removeUnusedMolecules()
    {
        // Debug.Log(this.GetType() + " removeUnusedMolecules");
        _toRemove.Clear();
        foreach (DisplayedMolecule molecule in _displayedMolecules)
        {
            if (!molecule.isUpdated())
            {
                _toRemove.AddLast(molecule);
            }
        }
        foreach (DisplayedMolecule molecule in _toRemove)
        {
            // useless?
            _toReset.Clear();
            foreach (KeyValuePair<Molecule, DisplayedMolecule> kvp in _molecules)
            {
                if (kvp.Value == molecule)
                {
                    _toReset.Add(kvp.Key);
                }
            }
            foreach (Molecule toReset in _toReset)
            {
                _molecules.Remove(toReset);
                _molecules.Add(toReset, null);
            }

            _displayedMolecules.Remove(molecule);
            _isMoleculesEdited = true;
            Debug.Log(this.GetType() + " removeUnusedMolecules => _isMoleculesEdited = " + _isMoleculesEdited);
        }
    }

    public void addDeviceAndMoleculesComponent(DisplayedDevice displayedDeviceScript)
    {
        Debug.Log(this.GetType() + " addDeviceAndMoleculesComponent with device=" + displayedDeviceScript._device);

        if (displayedDeviceScript == null || _eddwmGridTransform == null)
        {
            Debug.LogWarning(this.GetType() + " addDeviceAndMoleculesComponent device == null || _eddwmGridTransform == null");
        }
        else
        {
            //displayedDevice is "EquipedDevicePrefabPos" object

            bool newEquiped = (!_equipedDevices.Exists(equiped => equiped.device == displayedDeviceScript._device));
            if (newEquiped)
            {
                Debug.Log(this.GetType() + " addDeviceAndMoleculesComponent newEquiped");
                GameObject prefab = Resources.Load(DisplayedDevice.equipedWithMoleculesPrefabURI) as GameObject;
                GameObject equipedDisplayedDeviceWithMoleculesGameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, _eddwmGridTransform) as GameObject;
                equipedDisplayedDeviceWithMoleculesGameObject.transform.localScale = Vector3.one;
                equipedDisplayedDeviceWithMoleculesGameObject.transform.localPosition =
                new Vector3(equipedDisplayedDeviceWithMoleculesGameObject.transform.localPosition.x,
                      equipedDisplayedDeviceWithMoleculesGameObject.transform.localPosition.y,
                      0f);
                Debug.Log(this.GetType() + " addDeviceAndMoleculesComponent newEquiped will extract script");
                EquipedDisplayedDeviceWithMolecules eddwm = equipedDisplayedDeviceWithMoleculesGameObject.GetComponent<EquipedDisplayedDeviceWithMolecules>();
                eddwm.initialize(displayedDeviceScript);

                _equipedDevices.Add(eddwm);
                _isMoleculesEdited = true;
                Debug.Log(this.GetType() + " addDeviceAndMoleculesComponent => _isMoleculesEdited = " + _isMoleculesEdited);
                _isEDDWMsEdited = true;
                bool found = false;
                string protein = eddwm.device.getFirstGeneProteinName();

                Debug.Log(this.GetType() + " addDeviceAndMoleculesComponent newEquiped will match " + protein);
                //search if there's already in the cell a molecule that this device produces
                foreach (DisplayedMolecule molecule in _displayedMolecules)
                {
                    Debug.Log(this.GetType() + " addDeviceAndMoleculesComponent newEquiped tries " + molecule.getCodeName() + "...");
                    if (molecule.getCodeName() == protein)
                    {
                        displayMoleculeInDevice(molecule, eddwm);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.Log(this.GetType() + " addDeviceAndMoleculesComponent newEquiped couldn't find in _displayedMolecules a match for " + protein);
                }

                // Debug.Log("call to positionDeviceAndMoleculeComponents() by addDeviceAndMoleculesComponent");
                // positionDeviceAndMoleculeComponents();
            }
            else
            {
                Debug.Log(this.GetType() + " addDevice failed: newEquiped=" + newEquiped);
            }
        }
    }

    public void removeDeviceAndMoleculesComponent(Device device)
    {
        Debug.Log(this.GetType() + " removeDeviceAndMoleculesComponent");
        //TODO test BioBricks equality (cf next line)
        EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.Equals(device));
        //EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.getInternalName() == device.getInternalName());
        if (null != eddwm)
        {
            displayMoleculeInList(eddwm);

            _equipedDevices.Remove(eddwm);
            _isMoleculesEdited = true;
            Debug.Log(this.GetType() + " removeDeviceAndMoleculesComponent => _isMoleculesEdited = " + _isMoleculesEdited);
            _isEDDWMsEdited = true;

            Destroy(eddwm.gameObject);

            // Debug.Log("call to positionDeviceAndMoleculeComponents() by removeDeviceAndMoleculesComponent");
            // positionDeviceAndMoleculeComponents();
        }
        else
        {
            Debug.LogWarning(this.GetType() + " removeDeviceAndMoleculesComponent failed to remove eddwm");
        }
    }

    bool isAlreadyDisplayedInADevice(string moleculeCodeName)
    {
        return null != _equipedDevices.Find(equiped => equiped.device.getFirstGeneProteinName() == moleculeCodeName);
    }

    void displayMoleculeInList(EquipedDisplayedDeviceWithMolecules eddwm)
    {
        Debug.Log(this.GetType() + " displayMoleculeInList");
        string moleculeCodeName = eddwm.device.getFirstGeneProteinName();
        if (isAlreadyDisplayedInADevice(moleculeCodeName))
        {
            Debug.Log(this.GetType() + " displayMoleculeInList isAlreadyDisplayedInADevice");
            // to check: several devices may control same molecule
            // _displayedMolecules.Remove(eddwm.getDisplayedMolecule());
        }
        else
        {
            Debug.Log(this.GetType() + " displayMoleculeInList !isAlreadyDisplayedInADevice");
            // eddwm.releaseMoleculeDisplay();
        }
        eddwm.releaseMoleculeDisplay();
        _isMoleculesEdited = true;
        Debug.Log(this.GetType() + " displayMoleculeInList => _isMoleculesEdited = " + _isMoleculesEdited);
    }

    //change a display type of a molecule from molecule list to deviceWithMolecules list
    void displayMoleculeInDevice(DisplayedMolecule molecule, EquipedDisplayedDeviceWithMolecules eddwm)
    {
        Debug.Log(this.GetType() + " displayMoleculeInDevice");

        _isMoleculesEdited |= ((molecule.getDisplayType() == DisplayedMolecule.DisplayType.MOLECULELIST)
                                || !isAlreadyDisplayedInADevice(molecule.getCodeName())
                                );
        Debug.Log(this.GetType() + " displayMoleculeInDevice => _isMoleculesEdited = " + _isMoleculesEdited);

        eddwm.addDisplayedMolecule(molecule);
    }

    void positionDeviceAndMoleculeComponents()
    {
        Debug.Log(this.GetType() + " positionDeviceAndMoleculeComponents");

        positionDeviceAndMoleculeComponentsNow = false;

        Debug.Log(this.GetType() + " positionDeviceAndMoleculeComponents => _isMoleculesEdited = " + _isMoleculesEdited);
        _isEDDWMsEdited = false;

        if (null != _eddwmGridComponent && null != _moleculesAndDevicesTableComponent)
        {
            if (useTableFirst)
            {
                _moleculesAndDevicesTableComponent.repositionNow = true;
                _eddwmGridComponent.repositionNow = true;
                _moleculesAndDevicesTableComponent.repositionNow = true;
            }
            else
            {
                _eddwmGridComponent.repositionNow = true;
                _moleculesAndDevicesTableComponent.repositionNow = true;
                _eddwmGridComponent.repositionNow = true;
            }
        }
    }

    private void initializeMolecules()
    {
        Debug.Log(this.GetType() + " initializeMolecules");

        _molecules = new Dictionary<Molecule, DisplayedMolecule>();
        _moleculesArrayList = _reactionEngine.getMoleculesFromMedium(mediumId);

        foreach (System.Object molecule in _moleculesArrayList)
        {
            Debug.Log(this.GetType() + " initializeMolecules treating " + ((Molecule)molecule).getName());
            _molecule = (Molecule)molecule;
            _molecule.refreshTranslation();
            _molecules.Add(_molecule, null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        resetMoleculeList();

        if (null == _molecules)
        {
            Debug.Log(this.GetType() + " Update => null == _molecules");
            initializeMolecules();
        }

        if (0 != _toAdd.Count)
        {
            _toAdd.Clear();
        }

        if ((mediumId != 1) && Input.GetKeyDown(KeyCode.Return))
        {
            string debug = "";
            foreach (KeyValuePair<Molecule, DisplayedMolecule> kvp in _molecules)
            {
                _concentration = kvp.Key.getConcentration();
                if (0 != _concentration)
                {
                    debug += "[" + kvp.Key.getName() + ":" + _concentration + "];";
                }
            }
            if (!string.IsNullOrEmpty(debug))
            {
                debug.Remove(debug.Length - 1, 1);
            }
            debug = "medium " + mediumId + " [" + debug + "]";
            Debug.Log(debug);
        }

        foreach (KeyValuePair<Molecule, DisplayedMolecule> kvp in _molecules)
        {
            _molecule = kvp.Key;
            _displayedMolecule = kvp.Value;
            _concentration = _molecule.getConcentration();

            if (displayAll || (0 != _concentration))
            {
                if (null != _displayedMolecule)
                {
                    _displayedMolecule.update(_concentration);
                }
                else
                // molecule is not displayed yet
                {
                    Debug.Log(this.GetType() + " Update => molecule is not displayed yet");

                    _realName = _molecule.getRealName();
                    _codeName = _molecule.getName();

                    _createdDisplayedMolecule = new DisplayedMolecule(_codeName, _realName, _concentration, this, DisplayedMolecule.DisplayType.MOLECULELIST);

                    // search if molecule should be displayed in a Device/molecule component
                    _containers = _equipedDevices.FindAll(eddwm => eddwm.device.getFirstGeneProteinName() == _codeName);
                    if (_containers.Count != 0)
                    {
                        _createdDisplayedMolecule.setDisplayType(DisplayedMolecule.DisplayType.DEVICEMOLECULELIST);
                        foreach (EquipedDisplayedDeviceWithMolecules container in _containers)
                        {
                            container.addDisplayedMolecule(_createdDisplayedMolecule);
                        }
                    }
                    // anyway add it to molecule list
                    _displayedMolecules.AddLast(_createdDisplayedMolecule);
                    _isMoleculesEdited = true;
                    Debug.Log(this.GetType() + " Update => _isMoleculesEdited = " + _isMoleculesEdited);
                    _toAdd.Add(new KeyValuePair<Molecule, DisplayedMolecule>(_molecule, _createdDisplayedMolecule));
                }
            }
        }

        foreach (KeyValuePair<Molecule, DisplayedMolecule> kvp in _toAdd)
        {
            _molecules.Remove(kvp.Key);
            _molecules.Add(kvp.Key, kvp.Value);
        }

        removeUnusedMolecules();

        manageMoleculeLabels();

        if (_isMoleculesEdited || _isEDDWMsEdited)
        {
            Debug.Log("call to positionDeviceAndMoleculeComponents() by Update at t=" + Time.realtimeSinceStartup.ToString("F4") + " because _isMoleculesEdited || _isEDDWMsEdited");
            positionDeviceAndMoleculeComponents();
            positionDeviceAndMoleculeComponentsOnNextFrame = true;
            positionDeviceAndMoleculeComponentsOnEnable = true;
        }

        _isMoleculesEdited = false;

        if (positionDeviceAndMoleculeComponentsNow)
        {
            Debug.Log("call to positionDeviceAndMoleculeComponents() by Update at t=" + Time.realtimeSinceStartup.ToString("F4") + " because positionDeviceAndMoleculeComponentsNow");
            positionDeviceAndMoleculeComponents();
        }

        if (positionDeviceAndMoleculeComponentsOnNextFrame)
        {
            positionDeviceAndMoleculeComponentsNow = true;
            positionDeviceAndMoleculeComponentsOnNextFrame = false;
        }
    }

    private void manageMoleculeLabels()
    {
        if (_isMoleculesEdited)
        {
            Debug.Log(this.GetType() + " manageMoleculeLabels(true) with #_displayedMolecules=" + _displayedMolecules.Count);
        }

        if (_isMoleculesEdited) _namesToDisplay = "";
        _valuesToDisplay = "";

        foreach (DisplayedMolecule molecule in _displayedMolecules)
        {
            // Debug.LogWarning(this.GetType() + " manageMoleculeLabels(true) browses _displayedMolecules");
            if (molecule.getDisplayType() == DisplayedMolecule.DisplayType.MOLECULELIST)
            {
                // Debug.LogWarning(this.GetType() + " manageMoleculeLabels(true) browses _displayedMolecules with DisplayedMolecule.DisplayType.MOLECULELIST with " + molecule.getCodeName());
                if (_isMoleculesEdited)
                {
                    _namesToDisplay += molecule.getRealName() + ":\n";
                    Debug.Log(this.GetType() + " manageMoleculeLabels(true) adding " + molecule.getCodeName());
                }
                _valuesToDisplay += molecule.getVal() + "\n";
            }
        }
        // remove trailing new line characters '\n'
        if (!string.IsNullOrEmpty(_valuesToDisplay))
        {
            if (_isMoleculesEdited) _namesToDisplay.Remove(_namesToDisplay.Length - 1, 1);
            _valuesToDisplay.Remove(_valuesToDisplay.Length - 1, 1);
        }
        if (_isMoleculesEdited) namesLabel.text = _namesToDisplay;
        valuesLabel.text = _valuesToDisplay;
    }

    void OnEnable()
    {
        if (positionDeviceAndMoleculeComponentsOnEnable)
        {
            Debug.Log("call to positionDeviceAndMoleculeComponents() by OnEnable at t=" + Time.realtimeSinceStartup.ToString("F4") + " because positionDeviceAndMoleculeComponentsOnEnable");
            positionDeviceAndMoleculeComponents();
            positionDeviceAndMoleculeComponentsOnEnable = false;
            positionDeviceAndMoleculeComponentsOnNextFrame = true;
        }
    }

    public void onLanguageChanged()
    {
        Debug.Log(this.GetType() + " onLanguageChanged");
        foreach (KeyValuePair<Molecule, DisplayedMolecule> kvp in _molecules)
        {
            kvp.Key.refreshTranslation();
            if (null != kvp.Value)
            {
                kvp.Value.onLanguageChanged();
            }
        }
        foreach (EquipedDisplayedDeviceWithMolecules eddwm in _equipedDevices)
        {
            eddwm.onLanguageChanged();
        }
        _isMoleculesEdited = true;
    }
}

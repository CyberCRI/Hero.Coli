using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphMoleculeList : MonoBehaviour {

	private ReactionEngine   _reactionEngine;
	public int               mediumId;
  [SerializeField]
	private UILabel           namesLabel;
  [SerializeField]
  private UILabel           valuesLabel;
  [SerializeField]
	private bool              displayAll;

  [SerializeField]
  private GameObject        unfoldingMoleculeList;

  private const int               pixelsPerMoleculeLine = 15;
  private const int               pixelsPerDeviceLine = 80;
      
  [SerializeField]
  private UILabel           topLabels;
  [SerializeField]
  private UILabel           topValues;

  [SerializeField]
  private Vector3           topLabelsShift;
  [SerializeField]
  private Vector3           topValuesShift;
  public Vector3           currentHeight;

  private LinkedList<DisplayedMolecule> _displayedMolecules = new LinkedList<DisplayedMolecule>();
  private int                           _displayedListMoleculesCount = 0;
  private LinkedList<DisplayedMolecule> _toRemove = new LinkedList<DisplayedMolecule>();
  private List<EquipedDisplayedDeviceWithMolecules> _equipedDevices = new List<EquipedDisplayedDeviceWithMolecules>();
  private Vector3 _initialScale;
  private int _previousListedCount, _previousTotalCount;
  private string _realName, _codeName, _namesToDisplay, _valuesToDisplay; 
  private float _concentration;
  private Molecule _molecule;
  private DisplayedMolecule _displayedMolecule, _createdDisplayedMolecule;
  private ArrayList _moleculesArrayList;
  private Dictionary<Molecule, DisplayedMolecule> _molecules = null; 
  private List<KeyValuePair<Molecule, DisplayedMolecule>> _toAdd = new List<KeyValuePair<Molecule, DisplayedMolecule>>();
  private List<EquipedDisplayedDeviceWithMolecules> _containers;
  private List<Molecule> _toReset = new List<Molecule>();

  // grid of EquipedDisplayedDeviceWithMolecules of the scroll view
  [SerializeField]
  private Transform _eddwmGridTransform;
  [SerializeField]
  private Transform _moleculesAndDevicesTableTransform;
  [SerializeField]
  private UIGrid _eddwmGridComponent;
  [SerializeField]
  private UITable _moleculesAndDevicesTableComponent;

  public void setMediumId(int newMediumId)
  {
    mediumId = newMediumId;
  }

    void safeInitialization()
    {
        if (null != _eddwmGridTransform)
        {
            // Debug.Log(this.GetType() + " destroying all of eddwm's children");
            for (int index = 0; index < _eddwmGridTransform.childCount; index++)
            {
                Destroy(_eddwmGridTransform.GetChild(index).gameObject);
            }
        }
    }
   
  void Start() {
    _reactionEngine = ReactionEngine.get();

    safeInitialization();
  }

  private void resetMoleculeList()
  {
    foreach(DisplayedMolecule molecule in _displayedMolecules)
    {
      molecule.reset();
    }
  }

  private void removeUnusedMolecules()
  {
    _toRemove.Clear();
    foreach(DisplayedMolecule molecule in _displayedMolecules)
    {
      if(!molecule.isUpdated())
      {
        _toRemove.AddLast(molecule);
      }
    }
    foreach(DisplayedMolecule molecule in _toRemove)
    {
      // useless?
      _toReset.Clear();
      foreach(KeyValuePair<Molecule, DisplayedMolecule> kvp in _molecules)
      {
        if(kvp.Value == molecule)
        {
          _toReset.Add(kvp.Key);
        }
      }
      foreach(Molecule toReset in _toReset)
      {
          _molecules.Remove(toReset);
          _molecules.Add(toReset, null);
      }

      _displayedMolecules.Remove(molecule);
      if(molecule.getDisplayType() == DisplayedMolecule.DisplayType.MOLECULELIST)
      {
        _displayedListMoleculesCount--;
      }
    }
  }

    public void addDeviceAndMoleculesComponent(DisplayedDevice displayedDeviceScript)
    {
        if (displayedDeviceScript == null)
        {
            Debug.LogWarning(this.GetType() + " addDeviceAndMoleculesComponent device == null");
        }
        else
        {
            //displayedDevice is "EquipedDevicePrefabPos" object
            GameObject displayedDevice = displayedDeviceScript.gameObject;

            bool newEquiped = (!_equipedDevices.Exists(equiped => equiped.device == displayedDeviceScript._device));
            if (newEquiped)
            {

                GameObject prefab = Resources.Load(DisplayedDevice.equipedWithMoleculesPrefabURI) as GameObject;
                GameObject equipedDisplayedDeviceWithMoleculesGameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, _eddwmGridTransform) as GameObject;
                equipedDisplayedDeviceWithMoleculesGameObject.transform.localScale = Vector3.one;
                equipedDisplayedDeviceWithMoleculesGameObject.transform.localPosition =
                new Vector3(equipedDisplayedDeviceWithMoleculesGameObject.transform.localPosition.x,
                equipedDisplayedDeviceWithMoleculesGameObject.transform.localPosition.y,
                0f);  
                
                EquipedDisplayedDeviceWithMolecules eddwm = equipedDisplayedDeviceWithMoleculesGameObject.GetComponent<EquipedDisplayedDeviceWithMolecules>();
                eddwm.initialize(displayedDeviceScript);

                _equipedDevices.Add(eddwm);

                //search if there's already in the cell a molecule that this device produces
                foreach (DisplayedMolecule molecule in _displayedMolecules)
                {
                    if (molecule.getCodeName() == eddwm.device.getFirstGeneProteinName())
                    {
                        displayMoleculeInDevice(molecule, eddwm);
                    }
                }

                positionDeviceAndMoleculeComponents();
            }
            else
            {
                // Debug.Log(this.GetType() + " addDevice failed: newEquiped=" + newEquiped);
            }
        }
    }

    public void removeDeviceAndMoleculesComponent(Device device)
    {
        //TODO test BioBricks equality (cf next line)
        EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.Equals(device));
        //EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.getInternalName() == device.getInternalName());
        if(null != eddwm)
        {
            displayMoleculeInList(eddwm);

            _equipedDevices.Remove(eddwm);
            Destroy(eddwm.gameObject);

            positionDeviceAndMoleculeComponents();
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
    string moleculeCodeName = eddwm.device.getFirstGeneProteinName();
    if(isAlreadyDisplayedInADevice(moleculeCodeName))
    {
      _displayedMolecules.Remove(eddwm.getDisplayedMolecule());
    }
    else
    {
      eddwm.releaseMoleculeDisplay();
      _displayedListMoleculesCount++;
    }
  }
    
  //change a display type of a molecule from molecule list to deviceWithMolecules list
  void displayMoleculeInDevice(DisplayedMolecule molecule, EquipedDisplayedDeviceWithMolecules eddwm)
  {
      if((molecule.getDisplayType() == DisplayedMolecule.DisplayType.MOLECULELIST)
         || !isAlreadyDisplayedInADevice(molecule.getCodeName()))
      {
          _displayedListMoleculesCount--;
      }
      eddwm.addDisplayedMolecule(molecule);
  }

  void updateDisplayedListMoleculesCount()
  {
    int res = 0;
    foreach(DisplayedMolecule molecule in _displayedMolecules)
    {
      if(molecule.getDisplayType() == DisplayedMolecule.DisplayType.MOLECULELIST)
      {
          res++;
      }
    }
    _displayedListMoleculesCount = res;
   }

  void positionDeviceAndMoleculeComponents()
  {
    // Debug.LogWarning("positionDeviceAndMoleculeComponents");
    if(null != _eddwmGridComponent && null != _moleculesAndDevicesTableComponent)
    {
      _eddwmGridComponent.repositionNow = true;
      _moleculesAndDevicesTableComponent.repositionNow = true;
    }
  }

	// Update is called once per frame
	void Update()
  {
        
    _previousListedCount = _displayedListMoleculesCount;
    _previousTotalCount = _displayedMolecules.Count;

    resetMoleculeList();

    if (null == _molecules)
    {
      _molecules = new Dictionary<Molecule, DisplayedMolecule>();
		  _moleculesArrayList = _reactionEngine.getMoleculesFromMedium(mediumId);

      foreach(System.Object molecule in _moleculesArrayList)
      {
        _molecules.Add((Molecule)molecule, null);
      }
    }

    if(0 != _toAdd.Count)
    {
      _toAdd.Clear();
    }

		foreach(KeyValuePair<Molecule, DisplayedMolecule> kvp in _molecules) {
      _molecule = kvp.Key;
      _displayedMolecule = kvp.Value;
			_concentration = _molecule.getConcentration();
      if(displayAll || (0 != _concentration))
      {
        if(null != _displayedMolecule)
        {
          _displayedMolecule.update(_concentration);
        }
        else
        //molecule is not displayed yet
        {
          _realName = _molecule.getRealName();
          _codeName = _molecule.getName();

          _createdDisplayedMolecule = new DisplayedMolecule(_codeName, _realName, _concentration, DisplayedMolecule.DisplayType.MOLECULELIST);

          //search if molecule should be displayed in a Device/molecule component
          _containers = _equipedDevices.FindAll(eddwm => eddwm.device.getFirstGeneProteinName() == _codeName);
          if(_containers.Count != 0)
          {                        
            _createdDisplayedMolecule.setDisplayType(DisplayedMolecule.DisplayType.DEVICEMOLECULELIST);
            foreach(EquipedDisplayedDeviceWithMolecules container in _containers)
            {
              container.addDisplayedMolecule(_createdDisplayedMolecule);
            }
          }
          else
          {
            _displayedListMoleculesCount++;
          }
          //anyway add it to molecule list
          _displayedMolecules.AddLast(_createdDisplayedMolecule);
          _toAdd.Add(new KeyValuePair<Molecule, DisplayedMolecule>(_molecule, _createdDisplayedMolecule));
        }
      }
		}

    foreach(KeyValuePair<Molecule, DisplayedMolecule> kvp in _toAdd)
    {
      _molecules.Remove(kvp.Key);
      _molecules.Add(kvp.Key, kvp.Value);
    }

    removeUnusedMolecules();

    if(_displayedMolecules.Count != _previousTotalCount
       || _previousListedCount != _displayedListMoleculesCount)
    {
      //rearrange devices
      positionDeviceAndMoleculeComponents();
    }
		
		_namesToDisplay = "";
    _valuesToDisplay = "";

		foreach(DisplayedMolecule molecule in _displayedMolecules) {
      if(molecule.getDisplayType() == DisplayedMolecule.DisplayType.MOLECULELIST)
      {
          _namesToDisplay+=molecule.getRealName()+":\n";
          _valuesToDisplay+=molecule.getVal()+"\n";
      }
		}
		if(!string.IsNullOrEmpty(_namesToDisplay)) {
			_namesToDisplay.Remove(_namesToDisplay.Length-1, 1);
      _valuesToDisplay.Remove(_valuesToDisplay.Length-1, 1);
		}
    namesLabel.text = _namesToDisplay;
    valuesLabel.text = _valuesToDisplay;
	}
}

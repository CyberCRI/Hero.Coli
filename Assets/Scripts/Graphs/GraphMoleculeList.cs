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
            Debug.Log("destroying all of eddwm's children");
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
            Logger.Log("GraphMoleculeList::addDeviceAndMoleculesComponent device == null", Logger.Level.WARN);
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
                Logger.Log("addDevice failed: newEquiped=" + newEquiped, Logger.Level.TRACE);
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
            Logger.Log("GraphMoleculeList::removeDeviceAndMoleculesComponent failed to remove eddwm", Logger.Level.WARN);
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
    Debug.LogWarning("positionDeviceAndMoleculeComponents");
    if(null != _eddwmGridComponent && null != _moleculesAndDevicesTableComponent)
    {
      _eddwmGridComponent.repositionNow = true;
      _moleculesAndDevicesTableComponent.repositionNow = true;
    }
  }

	// Update is called once per frame
	void Update()
  {
        
    int previousListedCount = _displayedListMoleculesCount;
    int previousTotalCount = _displayedMolecules.Count;

    resetMoleculeList();

		ArrayList molecules = _reactionEngine.getMoleculesFromMedium(mediumId);
		foreach(System.Object molecule in molecules) {
      Molecule castMolecule = (Molecule)molecule;
      string realName = castMolecule.getRealName();
      string codeName = castMolecule.getName();
			float concentration = castMolecule.getConcentration();
      if(displayAll || (0 != concentration))
      {
        DisplayedMolecule found = LinkedListExtensions.Find(
                    _displayedMolecules
                    , m => m.getCodeName() == codeName
                    , false
                    , " GraphMoleculeList::Update()"
                    );
        if(null != found)
        {
          found.update(concentration);
        }
        else
        //molecule is not displayed yet
        {
          DisplayedMolecule created = new DisplayedMolecule(codeName, realName, concentration, DisplayedMolecule.DisplayType.MOLECULELIST);

          //search if molecule should be displayed in a Device/molecule component
          List<EquipedDisplayedDeviceWithMolecules> containers = _equipedDevices.FindAll(eddwm => eddwm.device.getFirstGeneProteinName() == codeName);
          if(containers.Count != 0)
          {                        
            created.setDisplayType(DisplayedMolecule.DisplayType.DEVICEMOLECULELIST);
            foreach(EquipedDisplayedDeviceWithMolecules container in containers)
            {
              container.addDisplayedMolecule(created);
            }
          }
          else
          {
            _displayedListMoleculesCount++;
          }
          //anyway add it to molecule list
          _displayedMolecules.AddLast(created);
        }
      }
		}

    removeUnusedMolecules();

    if(_displayedMolecules.Count != previousTotalCount
       || previousListedCount != _displayedListMoleculesCount)
    {
      //rearrange devices
      positionDeviceAndMoleculeComponents();
    }
		
		string namesToDisplay = "";
    string valuesToDisplay = "";

		foreach(DisplayedMolecule molecule in _displayedMolecules) {
      if(molecule.getDisplayType() == DisplayedMolecule.DisplayType.MOLECULELIST)
      {
          namesToDisplay+=molecule.getRealName()+":\n";
          valuesToDisplay+=molecule.getVal()+"\n";
      }
		}
		if(!string.IsNullOrEmpty(namesToDisplay)) {
			namesToDisplay.Remove(namesToDisplay.Length-1, 1);
      valuesToDisplay.Remove(valuesToDisplay.Length-1, 1);
		}
    namesLabel.text = namesToDisplay;
    valuesLabel.text = valuesToDisplay;
	}
}

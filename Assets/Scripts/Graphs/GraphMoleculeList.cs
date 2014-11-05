using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphMoleculeList : MonoBehaviour {

	private ReactionEngine   _reactionEngine;
	public int               mediumId;
	public UILabel           namesLabel;
  public UILabel           valuesLabel;
	public bool              displayAll;
  public GameObject        unfoldingMoleculeList;

  public GameObject        equipedWithMoleculesDeviceDummy;
  public GameObject        equipmentDeviceDummy;
  public GameObject        equipedDeviceDummy;
    
  public string            debugName;

  private int               pixelsPerMoleculeLine = 15;
  private int               pixelsPerDeviceLine = 80;
      
  public UILabel           topLabels;
  public UILabel           topValues;
  public Vector3           topLabelsShift;
  public Vector3           topValuesShift;
  public Vector3           currentHeight;

  private LinkedList<DisplayedMolecule> _displayedMolecules = new LinkedList<DisplayedMolecule>();
  private int                           _displayedListMoleculesCount = 0;
  private LinkedList<DisplayedMolecule> _toRemove = new LinkedList<DisplayedMolecule>();
  private List<EquipedDisplayedDeviceWithMolecules> _equipedDevices = new List<EquipedDisplayedDeviceWithMolecules>();
  private Vector3 _initialScale;

  public void setMediumId(int newMediumId)
  {
    mediumId = newMediumId;
  }

  void safeInitialization()
  {
    if(null == equipedWithMoleculesDeviceDummy)
    {
      EquipedDisplayedDeviceWithMolecules script = this.gameObject.GetComponentInChildren<EquipedDisplayedDeviceWithMolecules>() as EquipedDisplayedDeviceWithMolecules;
      equipedWithMoleculesDeviceDummy = script.gameObject;
    }
    if(null == equipedWithMoleculesDeviceDummy)
    {
      equipedWithMoleculesDeviceDummy = this.gameObject.transform.Find("DeviceMoleculesPanel").gameObject;
    }
    if(null == equipedWithMoleculesDeviceDummy)
    {
      equipedWithMoleculesDeviceDummy = GameObject.Find("DeviceMoleculesPanel");
    }
  }

  void Awake()
  {
    currentHeight = Vector3.zero;
    unfoldingMoleculeList.transform.localScale = new Vector3(unfoldingMoleculeList.transform.localScale.x, 20, unfoldingMoleculeList.transform.localScale.z);
    _initialScale = unfoldingMoleculeList.transform.localScale;
  }
   
  void Start() {
    _reactionEngine = ReactionEngine.get();

    safeInitialization();
    
    if(null != equipedWithMoleculesDeviceDummy)
    {
      equipedWithMoleculesDeviceDummy.SetActive(false);
    }
    else
    {
      Logger.Log("GraphMoleculeList::Start failed safeInitialization ", Logger.Level.WARN);
    }
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

  //TODO iTween this
  void setUnfoldingListBackgroundScale()
  {
    currentHeight = Vector3.up * (pixelsPerMoleculeLine * _displayedListMoleculesCount + pixelsPerDeviceLine * _equipedDevices.Count);
    unfoldingMoleculeList.transform.localScale = _initialScale + currentHeight;
  }

  public void addDeviceAndMoleculesComponent(DisplayedDevice equipedDeviceScript)
  {
    if(equipedDeviceScript == null)
    {
      Logger.Log ("GraphMoleculeList::addDeviceAndMoleculesComponent device == null", Logger.Level.WARN);
    }
    else
    {
      //equipedDevice is "EquipedDevicePrefabPos" object
      GameObject equipedDevice = equipedDeviceScript.gameObject;

      bool newEquiped = (!_equipedDevices.Exists(equiped => equiped.device == equipedDeviceScript._device)); 
      if(newEquiped) { 

        //EquipedDisplayedDeviceWithMolecules
        GameObject prefab = Resources.Load(DisplayedDevice.equipedWithMoleculesPrefabURI) as GameObject;        
        //deviceWithMoleculesComponent is "EquipedDisplayedDeviceWithMoleculesButtonPrefab" object
        //it needs an EquipmentDevice instance - it has only an EquipmentDeviceDummy object
        GameObject deviceWithMoleculesComponent = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        deviceWithMoleculesComponent.transform.parent = transform;
        deviceWithMoleculesComponent.transform.localScale = new Vector3(1f, 1f, 0);

        EquipedDisplayedDeviceWithMolecules eddwm = deviceWithMoleculesComponent.GetComponent<EquipedDisplayedDeviceWithMolecules>();
        

        //equipmentDevice
        GameObject equipmentDevicePrefab = Resources.Load(DisplayedDevice.equipmentPrefabURI) as GameObject;
        GameObject equipmentDeviceComponent = Instantiate(equipmentDevicePrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        eddwm.equipmentDevice = equipmentDeviceComponent;
        EquipmentDevice equipmentD = equipmentDeviceComponent.GetComponent<EquipmentDevice>() as EquipmentDevice;
        eddwm.equipmentDeviceScript = equipmentD;
                
        //equipedDevice
        GameObject equipedDeviceComponent = Instantiate(equipedDevice) as GameObject;
        eddwm.equipedDevice = equipedDeviceComponent;
        EquipedDisplayedDevice edd = equipedDeviceComponent.GetComponent<EquipedDisplayedDevice>() as EquipedDisplayedDevice;
        eddwm.equipedDeviceScript = edd;
        eddwm.device = equipedDeviceScript._device;
        edd._device = equipedDeviceScript._device;

        eddwm.initialize(equipmentDeviceDummy, equipedDeviceDummy);

        int previousEquipedDevicesCount = _equipedDevices.Count;
        _equipedDevices.Add(eddwm);

        //search if there's already in the cell a molecule that this device produces
        foreach(DisplayedMolecule molecule in _displayedMolecules)
        {
          if(molecule.getCodeName() == eddwm.device.getFirstGeneProteinName())
          {
            displayMoleculeInDevice(molecule, eddwm);
          }
        }

        //depends on displayed list of molecules
        //Vector3 localPosition = getNewPosition(previousEquipedDevicesCount);
        //deviceWithMoleculesComponent.transform.localPosition = localPosition;
        positionDeviceAndMoleculeComponents();
                    
      } else {
        Logger.Log("addDevice failed: newEquiped="+newEquiped, Logger.Level.TRACE);
      }
    }
  }

  Vector3 getNewPosition(int index = -1) {
    Vector3 res;
    int idx = index;
    if(idx == -1)
    {
      idx = _equipedDevices.Count;
    }
    
    res = equipedWithMoleculesDeviceDummy.transform.localPosition
            + new Vector3(
                0.0f,
                -_displayedListMoleculesCount*pixelsPerMoleculeLine -idx*pixelsPerDeviceLine,
                -0.1f
                );
    
    return res;
  }

  public void removeDeviceAndMoleculesComponent(Device device)
  {
    //TODO fix Equals method
    //EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.Equals(device));
    EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.getName() == device.getName());
    if(null != eddwm)
    {
      displayMoleculeInList(eddwm);

      int startIndex = _equipedDevices.IndexOf(eddwm);
      _equipedDevices.Remove(eddwm);
      Destroy(eddwm.gameObject);

      positionDeviceAndMoleculeComponents(0);
      setUnfoldingListBackgroundScale();
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

  //unused but works
  void shiftDeviceAndMoleculeComponents(int removedIndex)
  {
    for(int idx = removedIndex+1; idx < _equipedDevices.Count; idx++) {        
        Vector3 newLocalPosition = getNewPosition(idx-1);
        _equipedDevices[idx].gameObject.transform.localPosition = newLocalPosition;
    }
  }

  void positionDeviceAndMoleculeComponents(int startIndex = 0)
  {
    for(int idx = startIndex; idx < _equipedDevices.Count; idx++) {
      Vector3 newLocalPosition = getNewPosition(idx);
      _equipedDevices[idx].gameObject.transform.localPosition = newLocalPosition;
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

        /*
    if(null != topLabels)
    {
      topLabelsShift = Vector3.up * topLabels.relativeSize.y * topLabels.transform.localScale.y;
      namesLabel.transform.localPosition = topLabels.transform.localPosition + topLabelsShift;
    }
    */

    setUnfoldingListBackgroundScale();
	}
}

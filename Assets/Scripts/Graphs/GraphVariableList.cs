using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphVariableList : MonoBehaviour {

	//private ReactionEngine   _reactionEngine;
	//public int               mediumId;
  public WholeCell         wholeCell;
	public UILabel           namesLabel;
  public UILabel           valuesLabel;
	public bool              displayAll;
  public GameObject        unfoldingVariableList;

  //public GameObject        equipedWithMoleculesDeviceDummy;
  //public GameObject        equipmentDeviceDummy;
  //public GameObject        equipedDeviceDummy;
    
  public string            debugName;

  private int               pixelsPerMoleculeLine = 15;
  //private int               pixelsPerDeviceLine = 80;
      
  public UILabel           topLabels;
  public UILabel           topValues;
  public Vector3           topLabelsShift;
  public Vector3           topValuesShift;
  public Vector3           currentHeight;

  private LinkedList<DisplayedVariable> _displayedVariables = new LinkedList<DisplayedVariable>();
  private int                           _displayedListVariablesCount = 0;
  private LinkedList<DisplayedVariable> _toRemove = new LinkedList<DisplayedVariable>();
  //private List<EquipedDisplayedDeviceWithMolecules> _equipedDevices = new List<EquipedDisplayedDeviceWithMolecules>();
  private Vector3 _initialScale;

    /*
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
    */

  void Awake()
  {
    currentHeight = Vector3.zero;
    unfoldingVariableList.transform.localScale = new Vector3(unfoldingVariableList.transform.localScale.x, 20, unfoldingVariableList.transform.localScale.z);
    _initialScale = unfoldingVariableList.transform.localScale;
  }
   
  void Start() {
    //_reactionEngine = ReactionEngine.get();

    //safeInitialization();
    
        /*
    if(null != equipedWithMoleculesDeviceDummy)
    {
      equipedWithMoleculesDeviceDummy.SetActive(false);
    }
    else
    {
      Logger.Log("GraphVariableList::Start failed safeInitialization ", Logger.Level.WARN);
    }
    */
  }

  private void resetVariableList()
  {
    foreach(DisplayedVariable variable in _displayedVariables)
    {
      variable.reset();
    }
  }

  private void removeUnusedVariables()
  {
    _toRemove.Clear();
    foreach(DisplayedVariable variable in _displayedVariables)
    {
      if(!variable.isUpdated())
      {
        _toRemove.AddLast(variable);
      }
    }
    foreach(DisplayedVariable variable in _toRemove)
    {
      _displayedVariables.Remove(variable);
      //if(variable.getDisplayType() == DisplayedVariable.DisplayType.MOLECULELIST)
      //{
        _displayedListVariablesCount--;
      //}
    }
  }

  //TODO iTween this
  void setUnfoldingListBackgroundScale()
  {
    currentHeight = Vector3.up * (pixelsPerMoleculeLine * _displayedListVariablesCount /*+ pixelsPerDeviceLine * _equipedDevices.Count*/);
    unfoldingVariableList.transform.localScale = _initialScale + currentHeight;
  }

    /*
  public void addDeviceAndMoleculesComponent(DisplayedDevice equipedDeviceScript)
  {
    if(equipedDeviceScript == null)
    {
      Logger.Log ("GraphVariableList::addDeviceAndMoleculesComponent device == null", Logger.Level.WARN);
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

        //search if there's already in the cell a variable that this device produces
        foreach(DisplayedVariable variable in _displayedVariables)
        {
          if(variable.getCodeName() == eddwm.device.getFirstGeneProteinName())
          {
            displayMoleculeInDevice(variable, eddwm);
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
  */

  Vector3 getNewPosition(int index = -1) {
    Vector3 res;
    int idx = index;

        /*
    if(idx == -1)
    {
      idx = _equipedDevices.Count;
    }
    */
    
    res = /*equipedWithMoleculesDeviceDummy.transform.localPosition
            +*/ new Vector3(
                0.0f,
                -_displayedListVariablesCount*pixelsPerMoleculeLine /*-idx*pixelsPerDeviceLine*/,
                -0.1f
                );
    
    return res;
  }

    /*
    public void removeDeviceAndMoleculesComponent(Device device)
    {
        //TODO test BioBricks equality (cf next line)
        EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.Equals(device));
        //EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device.getInternalName() == device.getInternalName());
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
            Logger.Log("GraphVariableList::removeDeviceAndMoleculesComponent failed to remove eddwm", Logger.Level.WARN);
        }
    }
    */

    /*
  bool isAlreadyDisplayedInADevice(string moleculeCodeName)
  {
    return null != _equipedDevices.Find(equiped => equiped.device.getFirstGeneProteinName() == moleculeCodeName);
  }
  */

    /*
  void displayMoleculeInList(EquipedDisplayedDeviceWithMolecules eddwm)
  {
    string moleculeCodeName = eddwm.device.getFirstGeneProteinName();
    if(isAlreadyDisplayedInADevice(moleculeCodeName))
    {
      _displayedVariables.Remove(eddwm.getDisplayedMolecule());
    }
    else
    {
      eddwm.releaseMoleculeDisplay();
      _displayedListVariablesCount++;
    }
  }
  */

    /*
  //change a display type of a variable from variable list to deviceWithMolecules list
  void displayMoleculeInDevice(DisplayedVariable variable, EquipedDisplayedDeviceWithMolecules eddwm)
  {
      if((variable.getDisplayType() == DisplayedVariable.DisplayType.MOLECULELIST)
         || !isAlreadyDisplayedInADevice(variable.getCodeName()))
      {
          _displayedListVariablesCount--;
      }
      eddwm.addDisplayedMolecule(variable);
  }
  */

    /*
  void updateDisplayedListMoleculesCount()
  {
    int res = 0;
    foreach(DisplayedVariable variable in _displayedVariables)
    {
      //if(variable.getDisplayType() == DisplayedVariable.DisplayType.MOLECULELIST)
      //{
          res++;
      //}
    }
    _displayedListVariablesCount = res;
   }
   */

    /*
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
*/

	// Update is called once per frame
	void Update()
  {
        
    int previousListedCount = _displayedListVariablesCount;
    int previousTotalCount = _displayedVariables.Count;

    resetVariableList();

		ArrayList molecules = _reactionEngine.getMoleculesFromMedium(mediumId);
		foreach(System.Object variable in molecules) {
      Molecule castMolecule = (Molecule)variable;
      string realName = castMolecule.getRealName();
      string codeName = castMolecule.getName();
			float concentration = castMolecule.getConcentration();
      if(displayAll || (0 != concentration))
      {
        DisplayedVariable found = LinkedListExtensions.Find(
                    _displayedVariables
                    , m => m.getCodeName() == codeName
                    , false
                    , " GraphVariableList::Update()"
                    );
        if(null != found)
        {
          found.update(concentration);
        }
        else
        //variable is not displayed yet
        {
          DisplayedVariable created = new DisplayedVariable(codeName, realName, concentration, DisplayedVariable.DisplayType.MOLECULELIST);

          //search if variable should be displayed in a Device/variable component
          List<EquipedDisplayedDeviceWithMolecules> containers = _equipedDevices.FindAll(eddwm => eddwm.device.getFirstGeneProteinName() == codeName);
          if(containers.Count != 0)
          {                        
            created.setDisplayType(DisplayedVariable.DisplayType.DEVICEMOLECULELIST);
            foreach(EquipedDisplayedDeviceWithMolecules container in containers)
            {
              container.addDisplayedMolecule(created);
            }
          }
          else
          {
            _displayedListVariablesCount++;
          }
          //anyway add it to variable list
          _displayedVariables.AddLast(created);
        }
      }
		}

    removeUnusedVariables();

    if(_displayedVariables.Count != previousTotalCount
       || previousListedCount != _displayedListVariablesCount)
    {
      //rearrange devices
      positionDeviceAndMoleculeComponents();
    }
		
		string namesToDisplay = "";
    string valuesToDisplay = "";

		foreach(DisplayedVariable variable in _displayedVariables) {
      if(variable.getDisplayType() == DisplayedVariable.DisplayType.MOLECULELIST)
      {
          namesToDisplay+=variable.getRealName()+":\n";
          valuesToDisplay+=variable.getVal()+"\n";
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

﻿using UnityEngine;
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
  public string            debugName;

  private int               pixelsPerMoleculeLine = 15;
  private int               pixelsPerDeviceLine = 80;
      
  public Vector3           currentDownShift;

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
      Logger.Log("GraphMoleculeList::safeInitialization (null == equipedWithMoleculesDeviceDummy) 1 "+debugName, Logger.Level.WARN);
      EquipedDisplayedDeviceWithMolecules script = this.gameObject.GetComponentInChildren<EquipedDisplayedDeviceWithMolecules>() as EquipedDisplayedDeviceWithMolecules;
      equipedWithMoleculesDeviceDummy = script.gameObject;
    }
    if(null == equipedWithMoleculesDeviceDummy)
    {
      Logger.Log("GraphMoleculeList::safeInitialization (null == equipedWithMoleculesDeviceDummy) 2 "+debugName, Logger.Level.WARN);
      equipedWithMoleculesDeviceDummy = this.gameObject.transform.Find("DeviceMoleculesPanel").gameObject;
    }
    if(null == equipedWithMoleculesDeviceDummy)
    {
      Logger.Log("GraphMoleculeList::safeInitialization (null == equipedWithMoleculesDeviceDummy) 3 "+debugName, Logger.Level.WARN);
      equipedWithMoleculesDeviceDummy = GameObject.Find("DeviceMoleculesPanel");
    }
  }

  void Awake()
  {
    currentDownShift = Vector3.zero;
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
    currentDownShift = Vector3.up * (pixelsPerMoleculeLine * _displayedListMoleculesCount + pixelsPerDeviceLine * _equipedDevices.Count);
    unfoldingMoleculeList.transform.localScale = _initialScale + currentDownShift;
  }

  public void addDeviceAndMoleculesComponent(DisplayedDevice equipedDeviceScript)
  {
    if(equipedDeviceScript == null)
    {
      Logger.Log ("GraphMoleculeList::addDeviceAndMoleculesComponent device == null", Logger.Level.WARN);
    }
    else
    {
      GameObject equipedDevice = equipedDeviceScript.gameObject;
      bool newEquiped = (!_equipedDevices.Exists(equiped => equiped.device == equipedDeviceScript._device)); 
      if(newEquiped) { 

        GameObject clone = Instantiate(equipedDevice) as GameObject;

        GameObject prefab = Resources.Load(DisplayedDevice.equipedWithMoleculesPrefabURI) as GameObject;
        
        GameObject deviceWithMoleculesComponent = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        deviceWithMoleculesComponent.transform.parent = transform;
        deviceWithMoleculesComponent.transform.localScale = new Vector3(1f, 1f, 0);
        EquipedDisplayedDeviceWithMolecules eddwm = deviceWithMoleculesComponent.GetComponent<EquipedDisplayedDeviceWithMolecules>();

        eddwm.equipedDevice = clone;
        EquipedDisplayedDevice edd = clone.GetComponent<EquipedDisplayedDevice>() as EquipedDisplayedDevice;
        edd._device = equipedDeviceScript._device;

        eddwm.device = equipedDeviceScript._device;

        eddwm.equipedDeviceScript = equipedDeviceScript as EquipedDisplayedDevice;

        eddwm.initialize();

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
        Vector3 localPosition = getNewPosition();
        deviceWithMoleculesComponent.transform.localPosition = localPosition;

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
                -_displayedListMoleculesCount*pixelsPerMoleculeLine -(idx-1)*pixelsPerDeviceLine,
                -0.1f
                );
    
    return res;
  }

  public void removeDeviceAndMoleculesComponent(Device device)
  {
    EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device == device);
    if(null != eddwm)
    {
      displayMoleculeInList(eddwm);

      int startIndex = _equipedDevices.IndexOf(eddwm);
      _equipedDevices.Remove(eddwm);
      Destroy(eddwm.gameObject);

      positionDeviceAndMoleculeComponents(0);
      setUnfoldingListBackgroundScale();
    }
  }

  void displayMoleculeInList(EquipedDisplayedDeviceWithMolecules eddwm)
  {
    eddwm.releaseMoleculeDisplay();
    _displayedListMoleculesCount++;
  }

  //change a display type of a molecule from molecule list to deviceWithMolecules list
  void displayMoleculeInDevice(DisplayedMolecule molecule, EquipedDisplayedDeviceWithMolecules eddwm)
  {
    eddwm.addDisplayedMolecule(molecule);
    _displayedListMoleculesCount--;
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
	void Update () {
        
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
        DisplayedMolecule found = LinkedListExtensions.Find(_displayedMolecules, m => m.getRealName() == realName);
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

    setUnfoldingListBackgroundScale();
	}
}
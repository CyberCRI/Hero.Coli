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
  public string            debugName;

  //
  public int               pixelsPerLine;
  public int               equipedHeight = 60;
      
  public Vector3           currentDownShift;

  private LinkedList<DisplayedMolecule> _displayedMolecules = new LinkedList<DisplayedMolecule>();
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
    }
  }

  //TODO iTween this
  void setMoleculeListBackgroundScale()
  {
    currentDownShift = Vector3.up * pixelsPerLine * _displayedMolecules.Count;
    unfoldingMoleculeList.transform.localScale = _initialScale + currentDownShift;
  }

  public void addDeviceAndMoleculesComponent(DisplayedDevice equipedDeviceScript)
  {
    Debug.LogError("GraphMoleculeList::addDeviceAndMoleculesComponent("+equipedDeviceScript+")");
    Debug.LogError("including _device="+equipedDeviceScript._device);
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
        Debug.LogError("clone instantiated");

        Vector3 localPosition = getNewPosition();
        GameObject prefab = Resources.Load(DisplayedDevice.equipedWithMoleculesPrefabURI) as GameObject;
        
        GameObject deviceWithMoleculesComponent = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        Debug.LogError("EquipedDisplayedDeviceWithMolecules instantiated");
        deviceWithMoleculesComponent.transform.parent = transform;
        deviceWithMoleculesComponent.transform.localPosition = localPosition;
        deviceWithMoleculesComponent.transform.localScale = new Vector3(1f, 1f, 0);
        EquipedDisplayedDeviceWithMolecules script = deviceWithMoleculesComponent.GetComponent<EquipedDisplayedDeviceWithMolecules>();
        Debug.LogError("got EquipedDisplayedDeviceWithMolecules script");

        script.equipedDevice = clone;
        EquipedDisplayedDevice edd = clone.GetComponent<EquipedDisplayedDevice>() as EquipedDisplayedDevice;
        edd._device = equipedDeviceScript._device;
        Debug.LogError("assigned clone to EquipedDisplayedDeviceWithMolecules script with clone.device="+edd._device);

        script.device = equipedDeviceScript._device;
        Debug.LogError("assigned device="+script.device+" to EquipedDisplayedDeviceWithMolecules script");

        script.equipedDeviceScript = equipedDeviceScript;
        Debug.LogError("assigned equipedDeviceScript to EquipedDisplayedDeviceWithMolecules script");

        script.initialize();

        _equipedDevices.Add(script);

      } else {
        Logger.Log("addDevice failed: newEquiped="+newEquiped, Logger.Level.TRACE);
      }
    }
  }

  public Vector3 getNewPosition(int index = -1) {
    Vector3 res;
    int idx = index;
    if(idx == -1) idx = _equipedDevices.Count;
    res = equipedWithMoleculesDeviceDummy.transform.localPosition + new Vector3(0.0f, -idx*equipedHeight, -0.1f);
    return res;
  }

  public void removeDeviceAndMoleculesComponent(Device device)
  {
    Debug.LogError("GraphMoleculeList::removeDeviceAndMoleculesComponent");
    EquipedDisplayedDeviceWithMolecules eddwm = _equipedDevices.Find(elt => elt.device == device);
    int startIndex = _equipedDevices.IndexOf(eddwm);
    _equipedDevices.Remove(eddwm);
    Destroy(eddwm.gameObject);    
    for(int idx = startIndex; idx < _equipedDevices.Count; idx++) {
      Vector3 newLocalPosition = getNewPosition();
      _equipedDevices[idx].gameObject.transform.localPosition = newLocalPosition;
    }
  }

	// Update is called once per frame
	void Update () {

    resetMoleculeList();

		ArrayList molecules = _reactionEngine.getMoleculesFromMedium(mediumId);
		foreach(System.Object molecule in molecules) {
      Molecule castMolecule = (Molecule)molecule;
      string name = castMolecule.getRealName();
			float concentration = castMolecule.getConcentration();
      if(displayAll || (0 != concentration))
      {
        DisplayedMolecule found = LinkedListExtensions.Find(_displayedMolecules, m => m.getName() == name);
        if(null != found)
        {
          found.update(concentration);
        }
        else
        {
          DisplayedMolecule created = new DisplayedMolecule(name, concentration);
          _displayedMolecules.AddLast(created);
        }
      }
		}

    removeUnusedMolecules();

    setMoleculeListBackgroundScale();
		
		string namesToDisplay = "";
    string valuesToDisplay = "";
		foreach(DisplayedMolecule molecule in _displayedMolecules) {
			namesToDisplay+=molecule.getName()+":\n";
      valuesToDisplay+=molecule.getVal()+"\n";
		}
		if(!string.IsNullOrEmpty(namesToDisplay)) {
			namesToDisplay.Remove(namesToDisplay.Length-1, 1);
      valuesToDisplay.Remove(valuesToDisplay.Length-1, 1);
		}
    namesLabel.text = namesToDisplay;
    valuesLabel.text = valuesToDisplay;
	}
}

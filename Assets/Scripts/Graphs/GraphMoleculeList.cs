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

  void Awake()
  {
    currentDownShift = Vector3.zero;
    _initialScale = unfoldingMoleculeList.transform.localScale;
    equipedWithMoleculesDeviceDummy.SetActive(false);
  }
   
  void Start() {
    _reactionEngine = ReactionEngine.get();
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
    Debug.LogError("GraphMoleculeList::addDeviceAndMoleculesComponent");
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
        UnityEngine.Transform parent = unfoldingMoleculeList.transform;
        GameObject prefab = Resources.Load(DisplayedDevice.equipedWithMoleculesPrefabURI) as GameObject;
        
        GameObject deviceWithMoleculesComponent = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        Debug.LogError("EquipedDisplayedDeviceWithMolecules instantiated");
        deviceWithMoleculesComponent.transform.parent = parent;
        deviceWithMoleculesComponent.transform.localPosition = localPosition;
        deviceWithMoleculesComponent.transform.localScale = new Vector3(1f, 1f, 0);
        EquipedDisplayedDeviceWithMolecules script = deviceWithMoleculesComponent.GetComponent<EquipedDisplayedDeviceWithMolecules>();
        Debug.LogError("got EquipedDisplayedDeviceWithMolecules script");

        script.equipedDevice = clone;
        Debug.LogError("assigned clone to EquipedDisplayedDeviceWithMolecules script");

        script.device = equipedDeviceScript._device;
        Debug.LogError("assigned Device to EquipedDisplayedDeviceWithMolecules script");

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

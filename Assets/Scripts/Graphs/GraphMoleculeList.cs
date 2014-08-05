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
  public GameObject        equipedWithMoleculesDevice;

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

  public void addDeviceAndMoleculesComponent(Device device)
  {
    Debug.LogError("GraphMoleculeList::addDeviceAndMoleculesComponent");
    if(device == null)
    {
      Logger.Log ("GraphMoleculeList::addDeviceAndMoleculesComponent device == null", Logger.Level.WARN);
    }
    bool newEquiped = (!_equipedDevices.Exists(equiped => equiped._device == device)); 
    if(newEquiped) { 
        Vector3 localPosition = getNewPosition(DevicesDisplayer.DeviceType.Equiped);
            UnityEngine.Transform parent = unfoldingMoleculeList.transform;
        
        DisplayedDevice newDevice = 
            EquipedDisplayedDeviceWithMolecules.Create(
                parent,
                localPosition,
                null,
                device,
                this,
                DevicesDisplayer.DeviceType.Equiped
                );
        _equipedDevices.Add(newDevice);
    } else {
        Logger.Log("addDevice failed: alreadyEquiped="+newEquiped, Logger.Level.TRACE);
    }
  }

  public Vector3 getNewPosition(int index = -1) {
    Vector3 res;
    int idx = index;
    if(idx == -1) idx = _equipedDevices.Count;
    res = equipedWithMoleculesDevice.transform.localPosition + new Vector3(0.0f, -idx*equipedHeight, -0.1f);
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

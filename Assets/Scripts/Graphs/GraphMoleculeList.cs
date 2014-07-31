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

  //
  public int               pixelsPerLine;

  
  public Vector3           currentDownShift;

  private LinkedList<DisplayedMolecule> _displayedMolecules = new LinkedList<DisplayedMolecule>();
  private LinkedList<DisplayedMolecule> _toRemove = new LinkedList<DisplayedMolecule>();
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

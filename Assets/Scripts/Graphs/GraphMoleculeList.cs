using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphMoleculeList : MonoBehaviour {

	private ReactionEngine   _reactionEngine;
	public int               mediumId;
	public UILabel           namesLabel;
  public UILabel           valuesLabel;
	public bool              displayAll;

  private LinkedList<DebugMolecule> _debugMolecules = new LinkedList<DebugMolecule>();
  private LinkedList<DebugMolecule> _toRemove = new LinkedList<DebugMolecule>();

  private class DebugMolecule
  {
    private string _name;
    private string _val;
    private bool _updated;

    public string getName()
    {
      return _name;
    }

    public string getVal()
    {
      return _val;
    }

    public bool isUpdated()
    {
      return _updated;
    }

    public DebugMolecule(string name, string val)
    {
      _updated = true;
      _name = name;
      _val = val;
    }

    public DebugMolecule(string name, float val) : this(name, val.ToString())
    {
    }

    public void update(string val)
    {
      _val = val;
      _updated = true;
    }

    public void update(float val)
    {
      update(val.ToString());
    }

    public void reset()
    {
      _updated = false;
    }
  }

  public void setMediumId(int newMediumId)
  {
    mediumId = newMediumId;
  }

  void Start () {
    _reactionEngine = ReactionEngine.get();
  }

  private void resetMoleculeList()
  {
    foreach(DebugMolecule molecule in _debugMolecules)
    {
      molecule.reset();
    }
  }

  private void removeUnusedMolecules()
  {
    _toRemove.Clear();
    foreach(DebugMolecule molecule in _debugMolecules)
    {
      if(!molecule.isUpdated())
      {
        _toRemove.AddLast(molecule);
      }
    }
    foreach(DebugMolecule molecule in _toRemove)
    {
      _debugMolecules.Remove(molecule);
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
        DebugMolecule found = LinkedListExtensions.Find(_debugMolecules, m => m.getName() == name);
        if(null != found)
        {
          found.update(concentration);
        }
        else
        {
          DebugMolecule created = new DebugMolecule(name, concentration);
          _debugMolecules.AddLast(created);
        }
      }
		}

    removeUnusedMolecules();
		
		string namesToDisplay = "";
    string valuesToDisplay = "";
		foreach(DebugMolecule molecule in _debugMolecules) {
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoleculeDebug : MonoBehaviour {
	
	private ReactionEngine   _reactionEngine;
	public int               mediumId;
	public UILabel           label;
	public bool              displayAll;

  void Start () {
    _reactionEngine = ReactionEngine.get();
  }

	// Update is called once per frame
	void Update () {
		string inter = null;
		ArrayList molecules = _reactionEngine.getMoleculesFromMedium(mediumId);
		List<string> llMolecules = new List<string>();
		foreach(System.Object molecule in molecules) {
			inter = ((Molecule)molecule).ToShortString(displayAll);
			if(!string.IsNullOrEmpty(inter)) {
				llMolecules.Add(inter);
			}
		}
		llMolecules.Sort();
		
		string toDisplay = "";
		foreach(string molecule in llMolecules) {
			toDisplay+=molecule+"\n";
		}
		if(!string.IsNullOrEmpty(toDisplay)) {
			toDisplay.Remove(toDisplay.Length-1, 1);
		}	
		label.text = toDisplay;	
	}
}

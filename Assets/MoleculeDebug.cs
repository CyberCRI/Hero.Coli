using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoleculeDebug : MonoBehaviour {
	
	public ReactionEngine _engine;
	public int _mediumId;
	public UILabel _label;
	
	// Update is called once per frame
	void Update () {
		ArrayList molecules = _engine.getMoleculesFromMedium(_mediumId);
		List<string> llMolecules = new List<string>();
		foreach(System.Object molecule in molecules) {
			llMolecules.Add(((Molecule)molecule).ToShortString());
		}
		llMolecules.Sort();
		
		string toDisplay = "";
		foreach(string molecule in llMolecules) {
			toDisplay+=molecule+"\n";
		}
		toDisplay.Remove(toDisplay.Length-1, 1);
		_label.text = toDisplay;
	}
}

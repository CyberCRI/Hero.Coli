using System.Xml;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief Represent a Molecule set
  \details A MoleculeSet is assigned to a medium and so describe
  what quantity of which molecule is present in a medium.

A molecule set must be declared in molecule's files respecting this syntax:

    <Document>
      <molecules id="CelliaMolecules">
         [...] (molecules declarations)
      </molecules>
    <Document>

  
  
 */
public class MoleculeSet : CompoundLoadableFromXmlImplWithNew<Molecule>
{
	Dictionary<string, Molecule> _molecules;
	//!< The list of Molecule present in the set.
	public Dictionary<string, Molecule> molecules {
		get {
			if (_molecules == null) {
				var molecules = new Dictionary<string, Molecule> ();				
				foreach (Molecule molecule in elementCollection) {
					molecules.Add (molecule.getName (), molecule);
				}
				_molecules = molecules;
			}
			return _molecules;
		}
	}

	public override string getTag ()
	{
		return "molecule";
	}

	public MoleculeSet ()
	{
	}

	public override string ToString ()
	{
		string moleculeString = "";
		if (molecules != null) {
			foreach (object molecule in molecules) {
				if (!string.IsNullOrEmpty (moleculeString)) {
					moleculeString += ", ";
				}
				moleculeString += ((Molecule)molecule).ToString ();
			}
		}
		moleculeString = "Molecules[" + moleculeString + "]";
		return "MoleculeSet[id:" + _stringId + ", molecules=" + moleculeString + "]";
	}
}
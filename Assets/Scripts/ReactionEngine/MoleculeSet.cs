using System.Xml;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief Represent a Molecule set
  \details A MoleculeSet is assigned to a medium and so describe
  what quantity of which molecule is present in a medium.

A molecule set must be declared in molecule's files respecting this synthax :

    <Document>
      <molecules id="CelliaMolecules">
         [...] (molecules declarations)
      </molecules>
    <Document>

  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class MoleculeSet : LoadableFromXmlImpl
{
  private new string            _stringId;                     //!< The MoleculeSet id (string id).
  public ArrayList              molecules;              //!< The list of Molecule present in the set.
  public override string getTag() {return "";}

  //public MoleculeSet(){}

  //implementation of XMLLoadable interface
  public override string getStringId()
  {
    return _stringId;
  }
    
  public override MoleculeSet initFromLoad<MoleculeSet,L>(XmlNode node, FileLoader loader)
  {
        //public static bool loadMolecule(XmlNode node, ArrayList molecules)

    molecules = new ArrayList();

    switch (node.Attributes["type"].Value)
    {
      case "enzyme":
        FileLoader.storeMolecule(node, Molecule.eType.ENZYME, molecules);
        break;
      case "transcription_factor":
        FileLoader.storeMolecule(node, Molecule.eType.TRANSCRIPTION_FACTOR, molecules);
        break;
      case "other":
        FileLoader.storeMolecule(node, Molecule.eType.OTHER, molecules);
        break;
    }
    return this;
  }
    
  public override string ToString()
  {
    string moleculeString = "";
    foreach(object molecule in molecules)
    {
      if(!string.IsNullOrEmpty(moleculeString))
      {
        moleculeString += ", ";
      }
      moleculeString += ((Molecule)molecule).ToString();
    }
    moleculeString = "Molecules["+moleculeString+"]";
    return "MoleculeSet[id:"+_stringId+", molecules="+moleculeString+"]";
  }
}
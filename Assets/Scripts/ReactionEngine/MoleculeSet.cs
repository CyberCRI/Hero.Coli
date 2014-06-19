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
public class MoleculeSet : LoadableFromXml
{
  private string                _id;                     //!< The MoleculeSet id (string id).
  public ArrayList              molecules;              //!< The list of Molecule present in the set.
  public string getTag() {return "";}

  //implementation of XMLLoadable interface
  public string getId()
  {
    return _id;
  }

  //implementation of XMLLoadable interface
  public void initializeFromXml(XmlNode node, string id)
  {
    _id = id;

    molecules = new ArrayList();
    foreach (XmlNode mol in node)
    {
        
      if (mol.Name == "molecule")
      {
          FileLoader.loadMolecule(mol, molecules);
      }
    }
  }
    
  public T initFromLoad<T,L>(XmlNode node, L loader)
      where T: new()
  {
      return new T();
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
    return "MoleculeSet[id:"+_id+", molecules="+moleculeString+"]";
  }
}
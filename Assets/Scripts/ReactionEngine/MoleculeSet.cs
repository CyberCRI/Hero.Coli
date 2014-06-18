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
public class MoleculeSet : XMLLoadable
{
  new public string             id;                     //!< The MoleculeSet id (string id).
  public ArrayList              molecules;              //!< The list of Molecule present in the set.


  public new void init (XmlNode node, string id2)
  {
    molecules = new ArrayList();
    id = id2;
    foreach (XmlNode mol in node)
    {
        
      if (mol.Name == "molecule")
      {
          FileLoader.loadMolecule(mol, molecules);
      }
    }
  }
    
  public override string ToString()
  {
    return "MoleculeSet[id:"+id+", molecules="+molecules+"]";
  }
}
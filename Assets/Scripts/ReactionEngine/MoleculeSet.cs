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

  
  
 */
public class MoleculeSet : LoadableFromXmlImpl
{
  private new string            _stringId;                     //!< The MoleculeSet id (string id).
  public ArrayList              molecules;                     //!< The list of Molecule present in the set.
  public override string getTag() {return "molecule";}

  public MoleculeSet(){}

  //implementation of XMLLoadable interface
  public override string getStringId()
  {
    return _stringId;
  }
    
  //warning: assumes that node contains correct information
  protected override void innerInstantiateFromXml(XmlNode node, object loader)
  {
    Logger.Log ("MoleculeSet::innerInstantiateFromXml"
                  , Logger.Level.DEBUG);
    _stringId = node.Attributes["id"].Value;
    
    molecules = new ArrayList();

    foreach (XmlNode moleculeNode in node)
    {
      Molecule mol = new Molecule();
      if(mol.tryInstantiateFromXml(moleculeNode, null))
      {
        molecules.Add(mol);
      }
      else
      {
        Logger.Log ("MoleculeSet.innerInstantiateFromXml could not load molecule from "+Logger.ToString(moleculeNode), Logger.Level.WARN);
      }
    }
  }
    
  public override string ToString()
  {
    string moleculeString = "";
    if(molecules != null)
    {
      foreach(object molecule in molecules)
      {
        if(!string.IsNullOrEmpty(moleculeString))
        {
          moleculeString += ", ";
        }
        moleculeString += ((Molecule)molecule).ToString();
      }
    }
    moleculeString = "Molecules["+moleculeString+"]";
    return "MoleculeSet[id:"+_stringId+", molecules="+moleculeString+"]";
  }
}
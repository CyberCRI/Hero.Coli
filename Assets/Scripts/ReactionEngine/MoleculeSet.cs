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
  public ArrayList              molecules;                     //!< The list of Molecule present in the set.
  public override string getTag() {return "molecule";}

  public MoleculeSet(){}

  //implementation of XMLLoadable interface
  public override string getStringId()
  {
    return _stringId;
  }
    
  //warning: assumes that node contains correct information
  protected override void innerInstantiateFromXml(XmlNode node)
    {
        Logger.Log ("MoleculeSet::innerInstantiateFromXml"
                    , Logger.Level.DEBUG);
      _stringId = node.Attributes["id"].Value;
      initFromLoad(node, null);
  }

  //loader is not used here
  public override void initFromLoad(XmlNode setNode, object loader)
  {
    //public static bool loadMolecule(XmlNode node, ArrayList molecules)

    Logger.Log ("MoleculeSet.initFromLoad("+Logger.ToString(setNode)+", "+loader+")", Logger.Level.WARN);

    molecules = new ArrayList();

    foreach (XmlNode moleculeNode in setNode)
    {            
      storeMolecule(moleculeNode, molecules);        
    }
  }

    
    /*!
    \brief This function loads a molecule
    \param node The xml node to parse
    \param type The molecule type
    \param molecules The array of molecules where to add new molecules
    \return Return always true
   */
    public static bool storeMolecule(XmlNode node, ArrayList molecules)
    {
        
      Logger.Log ("MoleculeSet.storeMolecule("+Logger.ToString(node)
                  +", "+Logger.ToString<Molecule>("Molecule", molecules)
                  +")"
                  , Logger.Level.DEBUG);

      Molecule mol = new Molecule();

      mol.initFromLoad(node, null);

      molecules.Add(mol);

      Logger.Log ("MoleculeSet.storeMolecule(node"
                  +", "+Logger.ToString<Molecule>("Molecule", molecules)
                  +") loaded "+mol
                  , Logger.Level.TRACE);

      return true;
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
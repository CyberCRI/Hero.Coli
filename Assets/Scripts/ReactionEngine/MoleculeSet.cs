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

    public override void initializeFromXml(XmlNode node, string id)
    {
        Logger.Log ("MoleculeSet::initializeFromXml"
                    , Logger.Level.INFO);
        _stringId = id;
        initFromLoad(node, null);
    }


  //loader is not used here
  public override void initFromLoad(XmlNode setNode, object loader)
  {
    //public static bool loadMolecule(XmlNode node, ArrayList molecules)

    Logger.Log ("MoleculeSet.initFromLoad("+Logger.ToString(setNode)+", "+loader+")", Logger.Level.WARN);
                
    string typeString1 = "";
      if(null != setNode.Attributes["type"])
    {
      typeString1 = "type="+setNode.Attributes["type"].Value;
    }
    else
    {
      typeString1 = "type=null";
    }
    Logger.Log ("MoleculeSet.initFromLoad name="+setNode.Name+", "+typeString1, Logger.Level.ERROR);

    molecules = new ArrayList();

    foreach (XmlNode moleculeNode in setNode)
    {            
      string typeString = "";
        if(null != moleculeNode.Attributes["type"])
      {
        typeString = "type="+moleculeNode.Attributes["type"].Value;
      }
      else
      {
        typeString = "type=null";
      }
      Logger.Log ("MoleculeSet.initFromLoad inner name="+moleculeNode.Name+", inner "+typeString, Logger.Level.ERROR);

      FileLoader.storeMolecule(moleculeNode, molecules);        
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
using System.Xml;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief Represent a Reaction set
  \details A ReactionSet is assigned to a medium and so describe
  which reaction is present in each medium.

A reaction set musth be declare in molecule's files respecting this synthax :

    <Document>
      <reactions id="CelliaReactions">
         [...] (reactions declarations)
      </reactions>
    </Document>

  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class ReactionSet : LoadableFromXmlImpl
{
  public LinkedList<IReaction>   reactions;             //!< The list of reactions present in the set.

  //warning: assumes that node contains correct information
  protected override void innerInstantiateFromXml(XmlNode node)
  {
    _stringId = node.Attributes["id"].Value;
    reactions = new LinkedList<IReaction>();
    
    FileLoader loader = new FileLoader();
    loader.loadReactions(node, reactions);
  }
    
  public override void initFromLoad(XmlNode node, object loader)
  {
        Logger.Log ("ReactionSet::initFromLoad NOT IMPLEMENTED (btw loader="+loader+")"
                    , Logger.Level.ERROR);
  }
	
  public override string ToString()
	{
    return "ReactionSet[id:"+_stringId+", reactions="+Logger.ToString<IReaction>(reactions)+"]";
	}
}
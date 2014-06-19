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
public class ReactionSet : LoadableFromXml
{
  private string                 _id;                    //!< The ReactionSet id (string id),
  public LinkedList<IReaction>   reactions;             //!< The list of reactions present in the set.
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

		reactions = new LinkedList<IReaction>();

		FileLoader loader = new FileLoader();
		loader.loadReactions(node, reactions);
	}
    
  public T initFromLoad<T,L>(XmlNode node, L loader)
      where T: new()
  {
      return new T();
  }
	
  public override string ToString()
	{
    return "ReactionSet[id:"+_id+", reactions="+Logger.ToString<IReaction>(reactions)+"]";
	}
}
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
public class ReactionSet : XMLLoadable
{
  new public string              id;                    //!< The ReactionSet id (string id),
  public LinkedList<IReaction>   reactions;             //!< The list of reactions present in the set.


	public new void init (XmlNode node, string id2)
	{
		reactions = new LinkedList<IReaction>();
		id = id2;
		FileLoader loader = new FileLoader();
		loader.loadReactions(node, reactions);
		
	}
	
  public override string ToString()
	{
    return "ReactionSet[id:"+id+", reactions="+Logger.ToString<IReaction>(reactions)+"]";
	}
}
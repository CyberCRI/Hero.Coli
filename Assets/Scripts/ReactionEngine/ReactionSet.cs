using System.Xml;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief Represent a Reaction set
  \details A ReactionSet is assigned to a medium and so describe
  which reaction is present in each medium.

A reaction set musth be declare in molecule's files respecting this syntax :

    <Document>
      <reactions id="CelliaReactions">
         [...] (reactions declarations)
      </reactions>
    </Document>

  
  
 */
public class ReactionSet : CompoundLoadableFromXmlImpl<IReaction>
{
  //!< The list of reactions present in the set.
  //TODO setter/modifier/appender
  public LinkedList<IReaction> reactions {
    get
    {
        return new LinkedList<IReaction>(elementCollection);
    }
  }

  //TODO FIXME: one tag per reaction type
  //=> change xml with tag=reaction and type=promoter, allostery...
  //=> change logic of treatment
  public override string getTag() {return "reactions";}


  IReaction create(string reactionType)
  {
    switch(reactionType)
    {
      case "promoter":
        return new PromoterReaction();
        break;
      case "enzyme":
        return new EnzymeReaction();
        break;
      case "allostery":
        return new Allostery();
        break;
      case "instantReaction":
        return new InstantReaction();
        break;
      default:
        return new InstantReaction();
        break;
    }
  }

  //warning: assumes that node contains correct information
  protected override void innerInstantiateFromXml(XmlNode node)
  {
    Logger.Log ("ReactionSet::innerInstantiateFromXml("+Logger.ToString(node)+")"
                +" with elementCollection="+Logger.ToString<IReaction>("T", elementCollection)
                , Logger.Level.DEBUG);
    
    otherInitialize(node);

    elementCollection = new ArrayList();
    
    foreach (XmlNode eltNode in node)
    {
      IReaction elt = create(eltNode.Name);
      if(elt.tryInstantiateFromXml(eltNode))
      {
        elementCollection.Add(elt);
      }
      else
      {
        Logger.Log ("ReactionSet.innerInstantiateFromXml could not load elt from "+Logger.ToString(eltNode), Logger.Level.WARN);
      }
    }
  }

	
  public override string ToString()
	{
    return "ReactionSet[id:"+_stringId+", reactions="+Logger.ToString<IReaction>(reactions)+"]";
	}
}
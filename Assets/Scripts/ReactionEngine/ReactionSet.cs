using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*!
  \brief Represent a Reaction set
  \details A ReactionSet is assigned to a medium and so describe
  which reaction is present in each medium.

A reaction set musth be declare in molecule's files respecting this syntax:

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
  public ArrayList reactions {
    get
    {
        return elementCollection;
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
      /*
        Degradation
        PromoterReaction
        LawOfMassActionReaction
        Allostery
        EnzymeReaction
        ActiveTransportReaction
        FickReaction
        ATPProducer
        InstantReaction
      */
      case "promoter":
        return new PromoterReaction();
      case "enzyme":
        return new EnzymeReaction();
      case "allostery":
        return new Allostery();
      case "lawOfMassActionReaction":
        return new LawOfMassActionReaction();
      case "instantReaction":
        return new InstantReaction();
      default:
        return new InstantReaction();
    }
  }

  protected override IReaction construct(XmlNode node)
  {
    return create(node.Name);
  }
	
  public override string ToString()
	{
    return "ReactionSet[id:"+_stringId+", reactions="+Logger.ToString<IReaction>("IReaction", reactions)+"]";
	}
}
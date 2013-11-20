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
public class MoleculesSet
{
  public string                 id;                     //!< The MoleculeSet id (string id).
  public ArrayList              molecules;              //!< The list of Molecule present in the set.
}

/*!
  \brief Represent a Reaction set
  \details A ReactionsSet is assigned to a medium and so describe
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
public class ReactionsSet
{
  public string                  id;                    //!< The ReactionsSet id (string id),
  public LinkedList<IReaction>   reactions;             //!< The list of reactions present in the set.
	
  public override string ToString()
	{
		 return "ReactionsSet[id:"+id+", reactions="+Logger.ToString<IReaction>(reactions)+"]";
	}
}
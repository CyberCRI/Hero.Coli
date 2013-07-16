using System.Collections;
using System.Collections.Generic;

/*!
  \brief Represent a Molecule set
  \details A MoleculeSet is assigned to a medium and so describe
  what quantity of which molecule is present in a medium.
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
  \details A ReactionSet is assigned to a medium and so describe
  which reaction is present in each medium.
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class ReactionsSet
{
  public string                  id;                    //!< The ReactionSet id (string id),
  public LinkedList<IReaction>   reactions;             //!< The list of reactions present in the set.
}
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
using System.Xml;


public class MoleculesSet : Loadable
{
  public string                 id;                     //!< The MoleculeSet id (string id).
  public ArrayList              molecules;              //!< The list of Molecule present in the set.

	public MoleculesSet()
	{
	}
	public MoleculesSet (XmlNode node, string id)
	{
		molecules = new ArrayList();
		//MoleculesSet set = new MoleculesSet();

		foreach (XmlNode mol in node)
		{

			if (mol.Name == "molecule")
			{
				FileLoader.loadMolecule(mol, molecules);
			}
		}
		
		id = id;
	}
	public new void init (XmlNode node, string id2)
	{
		molecules = new ArrayList();
		id = id2;
		foreach (XmlNode mol in node)
		{
			
			if (mol.Name == "molecule")
			{
				FileLoader.loadMolecule(mol, molecules);
			}
		}
	}
	
	/*public static new void GLoad (XmlNode node, string id, LinkedList<MoleculesSet> AllSets)
	{
		Logger.Log ("MoleculesSet::GLoad",Logger.Level.DEBUG);
		ArrayList molecules = new ArrayList();
		MoleculesSet set = new MoleculesSet();

		foreach (XmlNode mol in node)
		{
			Logger.Log ("GLoad test ::"+GenericLoader.attributesName.molecules.ToString(),Logger.Level.WARN);
			if (mol.Name == "molecules")
				FileLoader.loadMolecule(mol, molecules);
		}

		set.id = id;
		set.molecules = molecules;
		AllSets.AddLast(set);
	}*/
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
public class ReactionsSet : Loadable
{
  public string                  id;                    //!< The ReactionsSet id (string id),
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
		 return "ReactionsSet[id:"+id+", reactions="+Logger.ToString<IReaction>(reactions)+"]";
	}
}
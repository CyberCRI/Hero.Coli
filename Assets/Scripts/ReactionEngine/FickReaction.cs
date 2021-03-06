using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*!
  \brief Represent a Fick's law 'reaction'.
  \details Manages Fick's law 'reactions'.
  This class manages a Fick's first law 'reaction', corresponding to the diffusion of
  small molecules across the cell membrane or between two different media.

  In this simulation, we model the rate of passive transport by diffusion across membranes.
  This rate depends on the concentration gradient between the two media and the (semi-)permeability
  of the membrane that separates them.  Diffusion moves materials from an area of higher concentration
  to the lower according to Fick's first law.
  A diffusion reaction, based on Fick's first law model, estimates the speed of diffusion of
  a molecule according to this formula : 

        dn/dt = (C1 - C2) * P * A

  - dn/dt: speed of diffusion
  - C1-C2: difference of concentration between both media
  - P: permeability coefficient
  - An exchange surface

  \reference http://books.google.fr/books?id=fXZW1REM0YEC&pg=PA636&lpg=PA636&dq=vitesse+de+diffusion+loi+de+fick&source=bl&ots=3eKv2NYYtx&sig=ciSW-RNAr0RTieE2oZfdBa73nT8&hl=en&sa=X&ei=bTufUcw4sI7sBqykgOAL&sqi=2&ved=0CD0Q6AEwAQ#v=onepage&q=vitesse%20de%20diffusion%20loi%20de%20fick&f=false
  
  
 */
public class FickReaction : Reaction
{
	private float _surface;
	//!< Contact surface size bwtween the two mediums
	private float _P;
	//!< Permeability coefficient
	private Medium _medium1;
	//!< The first Medium
	private Medium _medium2;
	//!< The second Medium
  
	//! Default constructor.
	public FickReaction ()
	{
		_surface = 0;
		_P = 0;
		_medium1 = null;
		_medium2 = null;
	}

	public FickReaction (FickReaction r) : base (r)
	{
		_surface = r._surface;
		_P = r._P;
		_medium1 = r._medium1;
		_medium2 = r._medium2;
	}

	public void setSurface (float surface)
	{
		_surface = surface;
	}

	public float getSurface ()
	{
		return _surface;
	}

	public void setPermCoef (float P)
	{
		_P = P;
	}

	public float getPermCoef ()
	{
		return _P;
	}

	public void setMedium1 (Medium medium)
	{
		_medium1 = medium;
	}

	public Medium getMedium1 ()
	{
		return _medium1;
	}

	public void setMedium2 (Medium medium)
	{
		_medium2 = medium;
	}

	public Medium getMedium2 ()
	{
		return _medium2;
	}
  
	/* !
    \brief Checks that two reactions have the same FickReaction field values.
    \param reaction The reaction that will be compared to 'this'.
   */
	protected override bool PartialEquals (Reaction reaction)
	{
		FickReaction fick = reaction as FickReaction;
		return (fick != null)
		&& base.PartialEquals (reaction)
		&& (_surface == fick._surface)
		&& (_P == fick._P)
		&& _medium1.Equals (fick._medium1)
		&& _medium2.Equals (fick._medium2);
		//TODO check Medium equality
	}

	public override bool hasValidData ()
	{
		return base.hasValidData ()
		&& 0 != _surface
		&& 0 != _P
		&& null != _medium1
		&& null != _medium2;
	}

	public static Reaction        buildFickReactionFromProps (FickProperties props, LinkedList<Medium> mediums)
	{
		FickReaction reaction = new FickReaction ();
		Medium med1 = ReactionEngine.getMediumFromId (props.MediumId1, mediums);
		Medium med2 = ReactionEngine.getMediumFromId (props.MediumId2, mediums);
    
		if (med1 == null || med2 == null) {
			// Debug.Log("Fick failed to build FickReaction from FickProperties beacause one or all the medium id don't exist");
			return null;
		}
		reaction.setSurface (props.surface);
		reaction.setPermCoef (props.P);
		reaction.setMedium1 (med1);
		reaction.setMedium2 (med2);
		reaction.setEnergyCost (props.energyCost);
    
		return reaction;
	}
  
  
	//! Return all the FickReactions possible from a Medium list.
	/*!
      \param mediums The list of mediums.

      \details
      This function return all the possible combinaisons of FickReaction in Medium list.

      Example :
        - Medium1 + Medium2 + Medium3 = FickReaction(1, 2) + FickReaction(1, 3) + FickReaction(2, 3)
  */
	public static LinkedList<FickReaction> getFickReactionsFromMediumList (LinkedList<Medium> mediums)
	{
		FickReaction newReaction;
		LinkedListNode<Medium> node;
		LinkedListNode<Medium> start = mediums.First;
		LinkedList<FickReaction> fickReactions = new LinkedList<FickReaction> ();
    
		while (start != null) {
			node = start.Next;
			while (node != null) {
				newReaction = new FickReaction ();
				newReaction.setMedium1 (start.Value);
				newReaction.setMedium2 (node.Value);
				fickReactions.AddLast (newReaction);
				node = node.Next;
			}
			start = start.Next;
		}
		return fickReactions;
	}
  
	//! Processing a reaction.
	/*!
      \param molecules A list of molecules (not usefull here)

A diffusion reaction based on fick model is calculated by using this formula :
 dn/dt = c1 - c2 * P * A
Where:
        - dn is the difference of concentration that will be applied
        - c1 and c2 the concentration the molecules in the 2 Mediums
        - P is the permeability coefficient
        - A is the contact surface size between the two Mediums
  */
	public override void react (Dictionary<string, Molecule> molecules)
	{
    
		var molMed1 = _medium1.getMolecules ();
		var molMed2 = _medium2.getMolecules ();
		Molecule mol2;
		float c1;
		float c2;
		float result;
    
		if (_P == 0f || _surface == 0f)
			return;
		foreach (Molecule mol1 in molMed1.Values) {
			c1 = mol1.getConcentration ();
			mol2 = ReactionEngine.getMoleculeFromName (mol1.getName (), molMed2);
			if (mol2 != null && mol2.getFickFactor () > 0f) {
				c2 = mol2.getConcentration ();
				result = (c2 - c1) * _P * _surface * mol2.getFickFactor () * _reactionSpeed * ReactionEngine.reactionSpeed * Time.deltaTime;
        
				if (enableSequential) {
					mol2.addConcentration (-result);
					mol1.addConcentration (result);
				} else {
					mol2.subNewConcentration (result);
					mol1.addNewConcentration (result);
				}
			}
		}
	}

	public override string ToString ()
	{
		return "[FickReaction m1=" + _medium1.getName () + " m2=" + _medium2.getName () + "]";
	}
  
}
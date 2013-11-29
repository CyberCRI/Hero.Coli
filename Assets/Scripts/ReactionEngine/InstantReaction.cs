using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief Describe an InstantReaction. This class can be loaded by the ReactionEngine
  \author Pierre COLLET
 */
public class InstantReactionProprieties
{
  public string name;                           //!< The name of the reaction
  public LinkedList<Product> reactants;         //!< The List of reactants
  public LinkedList<Product> products;          //!< The products of the reaction
  public float energyCost;                      //!< The cost in energy of the reaction
}

/*!
  This class represent all the reactions that are instantaneous
  The reactions that look like :

  2H + O = H2O should be managed by this class.
  see react() method for more details.

  \sa react()
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
  */
public class InstantReaction : IReaction
{
  private LinkedList<Product> _reactants;       //!< the list of reactants

  //! Default Constructor
  public InstantReaction()
  {
    _reactants = new LinkedList<Product>();
  }

  //! Default constructor
  public InstantReaction(InstantReaction r) : base(r)
  {
    _reactants = r._reactants;
  }

  /* !
    \brief Checks that two reactions have the same InstantReaction field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected override bool CharacEquals(IReaction reaction)
  {
    InstantReaction instant = reaction as InstantReaction;
    return (instant != null)
    && _reactants.Equals(instant._reactants);
  }

  /*!
    \brief Build an Instant reaction with a InstantReactionProprieties class
    \param props The proprieties
    \return Return a new reaction or null if it fail.
   */
  public static IReaction       buildInstantReactionFromProps(InstantReactionProprieties props)
  {
    if (props == null)
      return null;

    InstantReaction reaction = new InstantReaction();

    reaction.setName(props.name);
    reaction.setEnergyCost(props.energyCost);
    Product newReactant;
    foreach (Product r in props.reactants)
      {
        newReactant = new Product(r);
        reaction.addReactant(newReactant);
      }
    Product newProd;
    foreach (Product p in props.products)
      {
        newProd = new Product(p);
        reaction.addProduct(newProd);
      }
    return reaction;
  }


  public void addReactant(Product reactant) { if (reactant != null) _reactants.AddLast(reactant); }


  /*!
   Find the Limiting reactant in the attribute _reactant
   and return the factor as following :

        [MinReactant] / CoefReactant
  */
  private float getLimitantFactor(ArrayList molecules)
  {
    Product minReact = null;
    bool b = true;
    Molecule mol = null;
    Molecule molMin = null;

    foreach (Product r in _reactants)
      {
        mol = ReactionEngine.getMoleculeFromName(r.getName(), molecules);
        if (b && mol != null)
          {
            molMin = mol;
            minReact = r;
            b = false;
          }
        else if (mol != null)
          {
            if (molMin != null && ((mol.getConcentration() / r.getQuantityFactor()) < (molMin.getConcentration() / minReact.getQuantityFactor())))
              {
                molMin = mol;
                minReact = r;
              }
          }
      }
    if (minReact == null)
      return 0f;
    return (molMin.getConcentration() / minReact.getQuantityFactor());
  }


  /*!
   This function is called at each frame.
   It find the limiting reactant and consume as reactant and produce product
   as much as possible.

   The formula is :

        delta =  Min(Reactant_1 / Coef_1, Reactant_2 / Coef_2, ... , Reactant_n / Coef_n)
        for each product P : [P] += delta * Coef_P
        for each reactant R : [R] -= delta * Coef_R
  */
  public override void react(ArrayList molecules)
  {
    if (!_isActive)
      return;
    
    float delta = getLimitantFactor(molecules);

    float energyCoef;
    float energyCostTot;    
    if (delta > 0f && _energyCost > 0f && enableEnergy)
      {
        energyCostTot = _energyCost * delta;
        energyCoef = _medium.getEnergy() / energyCostTot;
        if (energyCoef > 1f)
          energyCoef = 1f;
        _medium.subEnergy(energyCostTot);
      }
    else
      energyCoef = 1f;

    delta *= energyCoef;
    Molecule mol;
    foreach (Product react in _reactants)
      {
        mol = ReactionEngine.getMoleculeFromName(react.getName(), molecules);
        if (enableSequential)
          mol.subConcentration(delta * react.getQuantityFactor());
        else
          mol.subNewConcentration(delta * react.getQuantityFactor());
      }
    foreach (Product prod in _products)
      {
        mol = ReactionEngine.getMoleculeFromName(prod.getName(), molecules);
        if (enableSequential)
          mol.addConcentration(delta * prod.getQuantityFactor());
        else
          mol.addNewConcentration(delta * prod.getQuantityFactor());
      }
  }
}
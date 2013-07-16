using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InstantReactionProprieties
{
  public string name;
  public LinkedList<Product> reactants;
  public LinkedList<Product> products;
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

  public static IReaction       buildInstantReactionFromProps(InstantReactionProprieties props)
  {
    InstantReaction reaction = new InstantReaction();

    reaction.setName(props.name);
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


  public string getName() { return _name; }
  public void setName(string str) { _name = str; }
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
    Molecule mol;

    foreach (Product react in _reactants)
      {
        mol = ReactionEngine.getMoleculeFromName(react.getName(), molecules);
        mol.subNewConcentration(delta * react.getQuantityFactor());
      }
    foreach (Product prod in _products)
      {
        mol = ReactionEngine.getMoleculeFromName(prod.getName(), molecules);
        mol.addNewConcentration(delta * prod.getQuantityFactor());
      }
  }

}
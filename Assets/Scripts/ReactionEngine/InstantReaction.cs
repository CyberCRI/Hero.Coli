using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/*!
  \brief Describe an InstantReaction. This class can be loaded by the ReactionEngine
  
 */
public class InstantReactionProperties
{
  public string name;                           //!< The name of the reaction
  public LinkedList<Product> reactants;         //!< The List of reactants
  public LinkedList<Product> products;          //!< The products of the reaction
  public float energyCost;                      //!< The cost in energy of the reaction
}

/*!
  This class represents all the reactions that are instantaneous
  The reactions that look like :

  2H + O = H2O should be managed by this class.
  see react() method for more details.

  \sa react()
  
  
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
  protected override bool PartialEquals(IReaction reaction)
  {
    InstantReaction instant = reaction as InstantReaction;
    return (instant != null)
    && base.PartialEquals(reaction)
    && _reactants.Equals(instant._reactants);
  }

  /*!
    \brief Build an Instant reaction with a InstantReactionProperties class
    \param props The properties
    \return Return a new reaction or null if it fail.
   */
  public static IReaction       buildInstantReactionFromProps(InstantReactionProperties props)
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





    /*
      An Instant reaction should respect this syntax :

              <instantReaction>
                <name>Water</name>                        -> Name of the reaction
                <EnergyCost>0.1</EnergyCost>              -> Energy cost of the reaction
                <reactants>
                  <reactant>
                    <name>O</name>                        -> Reactant name
                    <quantity>1</quantity>                -> Reactant coefficiant (how much of this to make products)
                  </reactant>
                  <reactant>
                    <name>H</name>
                    <quantity>2</quantity>
                  </reactant>
                </reactants>
                <products>
                  <product>
                    <name>H2O</name>                      -> Product Name
                    <quantity>1</quantity>                -> Product Quantity (how much is created)
                  </product>
                </products>
              </instantReaction>
     */

    /*!
    \brief Parse and load reactants of an InstantReaction
    \param node The xml node to parse
    \return return always true
   */
    private bool loadInstantReactionReactants(XmlNode node)
    {
      bool b = false;

      foreach (XmlNode attr in node)
      {
        if (attr.Name == "reactant")
        {
          if(!loadInstantReactionReactant(attr))
          {                    
            Logger.Log ("InstantReaction::loadInstantReactionReactants loadInstantReactionReactant failed"
                        , Logger.Level.ERROR);
            return false;
          }
          else
          {
              b = true;
          }
        }
        else
        {
          Logger.Log ("InstantReaction::loadInstantReactionReactants bad attr name:"+attr.Name
                            , Logger.Level.ERROR);
          return false;
        }
      }

      if(!b)
      {
        Logger.Log ("InstantReaction::loadInstantReactionReactants loaded nothing"
          , Logger.Level.ERROR);
        return false;
      }
      else
      {
        Logger.Log ("InstantReaction::loadInstantReactionReactants loaded successfully "+this
          , Logger.Level.DEBUG);
        return true;
      }

    }
    
    /*!
    \brief Parse and load reactant of an InstantReaction
    \param node The xml node to parse
    \return return always true
  */
    private bool loadInstantReactionReactant(XmlNode node)
    {
      Product prod = new Product();
      foreach (XmlNode attr in node)
      {
        if (attr.Name == "name")
        {
            if (String.IsNullOrEmpty(attr.InnerText))
            {
              Logger.Log("InstantReaction::loadInstantReactionReactant Empty name field in instant reaction reactant definition"
                               , Logger.Level.ERROR);
              return false;
            }
            prod.setName(attr.InnerText);
        }
        else if (attr.Name == "quantity")
        {
          if (String.IsNullOrEmpty(attr.InnerText))
          {                    
            Logger.Log("InstantReaction::loadInstantReactionReactant Empty quantity field in instant reaction reactant definition"
                       , Logger.Level.ERROR);
            return false;
          }
          prod.setQuantityFactor(float.Parse(attr.InnerText.Replace(",", ".")));
        }
      }
      addReactant(prod);
      return true;
    }
    
    /*!
    \brief Parse and load reactant of an InstantReaction
    \param node The xml node to parse
    \return return always true
  */
    private bool loadInstantReactionProducts(XmlNode node)
    {
      Boolean b = true;
      foreach (XmlNode attr in node)
      {
            //TODO should this be "if (b && (attr.Name == "product"))" ?
        if (attr.Name == "product")
        {
          b = b && loadInstantReactionProduct(attr);
        }
      }
      return b;
    }
    
    /*!
    \brief Parse and load products of an InstantReaction
    \param node The xml node to parse
    \return return always true
  */
    private bool loadInstantReactionProduct(XmlNode node)
    {
      Product prod = new Product();
      foreach (XmlNode attr in node)
      {
        if (attr.Name == "name")
        {
          if (String.IsNullOrEmpty(attr.InnerText))
          {
            Logger.Log("InstantReaction::loadInstantReactionProduct Empty name field in instant reaction product definition"
                               , Logger.Level.ERROR);
            return false;
          }
          prod.setName(attr.InnerText);
        }
        else if (attr.Name == "quantity")
        {
          if (String.IsNullOrEmpty(attr.InnerText))
          {
            Logger.Log("InstantReaction::loadInstantReactionProduct Empty quantity field in instant reaction product definition"
                         , Logger.Level.ERROR);
            return false;
          }
          prod.setQuantityFactor(float.Parse(attr.InnerText.Replace(",", ".")));
        }
      }
      addProduct(prod);
      return true;
    }
    
    /*!
    \brief Parse and load the energy cost of an InstantReaction
    \param value The value string
    \return return always true
  */
    private bool loadEnergyCost(string value)
    {
      if (String.IsNullOrEmpty(value))
      {
        Logger.Log("InstantReaction::loadInstantReactionProduct Empty EnergyCost field. default value = 0"
                       , Logger.Level.WARN);
        setEnergyCost(0f);
      }
      else
        setEnergyCost(float.Parse(value.Replace(",", ".")));
      return true;
    }


    public override bool tryInstantiateFromXml(XmlNode node)
    {
      Boolean b = true;
      foreach (XmlNode attr in node)
      {
        switch (attr.Name)
        {
          case "name":
            setName(attr.InnerText);
            break;
          case "reactants":
            b = b && loadInstantReactionReactants(attr);
            break;
          case "products":
            b = b && loadInstantReactionProducts(attr);
            break;
          case "EnergyCost":
            b = b && loadEnergyCost(attr.InnerText);
            break;
        }
      }

      return b
          && !string.IsNullOrEmpty(_name) 
          && (null != _reactants) && (0 != _reactants.Count)
          && (null != _products) && (0 != _products.Count);
  }
}
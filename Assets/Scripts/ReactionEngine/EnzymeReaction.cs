using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/*!
  \brief This class represents an EnzymeReaction and can be loaded by the ReactionEngine class
  
  \sa EnzymeReaction
 */
public class EnzymeReactionProperties
{
  public string name;
  public string substrate;
  public string enzyme;
  public string effector;
  public float Kcat;
  public float beta;
  public float alpha;
  public float Km;
  public float Ki;
  public LinkedList<Product> products;
  public float energyCost;
}

/*!
  \brief This class manage enzyme reactions
  \details

Manage enzymatic reactions.
This class manages enzymatic reactions that convert specific molecules, called substrates (S)
into different ones, called products (P.). An enzyme (E) is a highly selective catalyst for a defined substrate,
and accelerates both the rate and specificity of metabolic reactions. The enzyme activity can be affected by other
 molecules called activators (increased activity) or inhibitors (decreased activity).

In this simulation we have used a model that can describe enzymes kinetics (Michaelis-Menten)
 as well as common models of enzyme inhibition and activation by other molecules (different from the substrate).
The following scheme is a generalized model of inhibition that can describe competitive,
 uncompetitive, mixed and non-competitive inhibition, as well as heterotropic activation.
 
 
 */
public class EnzymeReaction : IReaction
{
  protected string _substrate;            //!< The substrate of the reaction
  protected string _enzyme;               //!< The enzyme of the reaction
  protected float _Kcat;                  //!< Reaction constant of enzymatic reaction
  protected string  _effector;            //!< The effector of the reaction
  protected float _alpha;                 //!< Alpha descriptor of the effector
  protected float _beta;                  //!< Beta descriptor of the effector
  protected float _Km;                    //!< Affinity coefficient between substrate and enzyme
  protected float _Ki;                    //!< Affinity coefficient between effector and enzyme

  public void setSubstrate(string str) { _substrate = str; }
  public string getSubstrate() { return _substrate; }
  public void setEnzyme(string str) { _enzyme = str;}
  public string getEnzyme() { return _enzyme; }
  public void setKcat(float value) { _Kcat = value;}
  public float getKcat() { return _Kcat; }
  public string getEffector() { return _effector; }
  public void setEffector(string str) { _effector = str; }
  public float getAlpha() { return _alpha; }
  public void setAlpha(float value) { _alpha = value;}
  public float getBeta() { return _beta; }
  public void setBeta(float value) { _beta = value;}
  public float getKm() { return _Km; }
  public void setKm(float value) { _Km = value;}
  public void setKi(float value) { _Ki = value;}
  public float getKi() { return _Ki; }

  //! Default Constructor
  public EnzymeReaction()
  {}

  //! Copy constructor
  public EnzymeReaction(EnzymeReaction r) : base(r)
  {
    _substrate = r._substrate;
    _enzyme = r._enzyme;
    _Kcat = r._Kcat;
    _effector = r._effector;
    _alpha = r._alpha;
    _beta = r._beta;
    _Km = r._Km;
    _Ki = r._Ki;
  }

  /* !
    \brief Checks that two reactions have the same EnzymeReaction field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected override bool PartialEquals(IReaction reaction)
  {
    EnzymeReaction enzyme = reaction as EnzymeReaction;
    return (enzyme != null)
    && base.PartialEquals(reaction)
    && (_substrate == enzyme._substrate)
    && (_enzyme    == enzyme._enzyme)
    && (_Kcat      == enzyme._Kcat)
    && (_effector  == enzyme._effector)
    && (_alpha     == enzyme._alpha)
    && (_beta      == enzyme._beta)
    && (_Km        == enzyme._Km)
    && (_Ki        == enzyme._Ki);
  }


  /*!
    \brief This function build a new EnzymeReaction based on the given EnzymeReactionProperties
    \param props The proprities class
    \return This function return a new EnzymeReaction or null if props is null.
   */
  public static IReaction       buildEnzymeReactionFromProps(EnzymeReactionProperties props)
  {
    if (props == null)
      return null;

    EnzymeReaction reaction = new EnzymeReaction();

    reaction.setName(props.name);
    reaction.setSubstrate(props.substrate);
    reaction.setEnzyme(props.enzyme);
    reaction.setKcat(props.Kcat);
    reaction.setEffector(props.effector);
    reaction.setAlpha(props.alpha);
    reaction.setBeta(props.beta);
    reaction.setKm(props.Km);
    reaction.setKi(props.Ki);
    reaction.setEnergyCost(props.energyCost);

    Product newProd;
    foreach (Product p in props.products)
      {
        newProd = new Product(p);
        reaction.addProduct(newProd);
      }
    return reaction;
  }


  /*!
    Execute an enzyme reaction.
    \details This function do all the calcul of an enzymatic reaction.
    The formula is :

                           [S]                      [S] * [I]
                   Vmax * ----  + beta * Vmax * ----------------
                           Km                    alpha * Km * Ki
          delta = -------------------------------------------------
                              [S]    [I]      [S] * [I]
                         1 + ---- + ---- + -----------------
                              Km     Ki     alpha * Km * Ki

          with : Vmax -> Maximal production
                 S -> Substrate
                 I -> Effector
                 Km -> affinity between substrate and enzyme
                 Ki -> affinity between effector and enzyme
                 alpha -> Describe the competitivity of the effector (I) with the substrate (S). a >> 1 = competitive inhibition
                                                                                                 a << 1 Uncompetitive inhibition
                                                                                                 a = 1 Noncompetitive inhibition
                 beta -> Describe the extend of inhibition (< 1) or the extend of activation (> 1)
                 others configuration of beta and alpha are mixed inhibition.

    \reference http://depts.washington.edu/wmatkins/kinetics/inhibition.html
    \return return the value that will be produce.
    \param molecules The list of molecules.
   */
  public float execEnzymeReaction(ArrayList molecules)
  {
    Molecule substrate = ReactionEngine.getMoleculeFromName(_substrate, molecules);
    Molecule enzyme = ReactionEngine.getMoleculeFromName(_enzyme, molecules);
    float Vmax = _Kcat * enzyme.getConcentration();
    float effectorConcentration = 0;

    if (_effector != "False")
      {
        Molecule effector = ReactionEngine.getMoleculeFromName(_effector, molecules);
        if (effector != null)
          effectorConcentration = effector.getConcentration();
      }
    if (_alpha == 0)
    {
      _alpha = 0.0000000001f;
      Logger.Log("_alpha == 0", Logger.Level.WARN);
    }
    if (_Ki == 0)
    {
      _Ki = 0.0000000001f;
      Logger.Log("_Ki == 0", Logger.Level.WARN);
    }
    if (_Km == 0)
    {
      _Km = 0.0000000001f;
      Logger.Log("_Km == 0", Logger.Level.WARN);
    }

    float denominator = _alpha * _Km * _Ki;


    float bigDenominator = 1f + (substrate.getConcentration() / _Km) + (effectorConcentration / _Ki) + (substrate.getConcentration() * effectorConcentration / denominator);
    if(bigDenominator == 0)
    {
      Logger.Log("big denominator == 0", Logger.Level.WARN);
      return 0;
    }

    float v = ((Vmax * (substrate.getConcentration() / _Km)) + (_beta * Vmax * substrate.getConcentration() * effectorConcentration / denominator))
      / bigDenominator;
    return v;
  }

  /*!
    \brief this function execute all the enzyme reactions
    \details It's call execEnzymeReaction and substract to the substrate concentration what this function return.
    This function also add this returned value to all the producted molecules.
    \param molecules The list of molecules
   */
  public override void react(ArrayList molecules)
  {
    if (!_isActive)
      return;
    
    Molecule substrate = ReactionEngine.getMoleculeFromName(_substrate, molecules);
    if (substrate == null)
      return ;

    //TODO introduce delta t here instead of 1f
    float delta = execEnzymeReaction(molecules) * 1f;

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

    if (enableSequential)
      substrate.subConcentration(delta);
    else
      substrate.subNewConcentration(delta);
    foreach (Product pro in _products)
    {
      Molecule mol = ReactionEngine.getMoleculeFromName(pro.getName(), molecules);
      if (enableSequential)
        mol.addConcentration(delta);
      else
        mol.addNewConcentration(delta);
    }
  }

    // loading
    /*
        An enzymatic reaction should be declared by respecting this convention:

        <enzyme>
          <name>ER</name>                   -> Name of the reaction
          <EnergyCost>0</EnergyCost>        -> Energy cost of the reaction
          <substrate>X</substrate>          -> Substrate molecule's name
          <enzyme>E</enzyme>                -> Enzyme molecule's name
          <Kcat>10</Kcat>                   -> Reaction constant of enzymatic reaction
          <effector>False</effector>        -> The name of the effector (or false if there is no effector)
          <alpha>1000</alpha>               -> Competitive parameter (see EnzymeReaction class for more infos)
          <beta>0</beta>                    -> Activation or inhibition character of the effector (see EnzymeReaction class for more infos)
          <Km>0.5</Km>                      -> Affinity between substrate and enzyme
          <Ki>0.05</Ki>                     -> Affinity between effector and enzyme
          <Products>
            <name>X*</name>                 -> Molecule's name of the product
          </Products>
        </enzyme>
    */

    public override bool tryInstantiateFromXml(XmlNode node)
    {
      bool b = true;
      foreach (XmlNode attr in node)
      {
        switch (attr.Name)
        {
          case "name":
            b = b && loadEnzymeString(attr.InnerText, setName);
            break;
          case "substrate":
            b = b && loadEnzymeString(attr.InnerText, setSubstrate);
            break;
          case "enzyme":
            b = b && loadEnzymeString(attr.InnerText, setEnzyme);
            break;
          case "Kcat":
            b = b && loadEnzymeFloat(attr.InnerText, setKcat);
            break;
          case "effector":
            b = b && loadEnzymeString(attr.InnerText, setEffector);
            break;
          case "alpha":
            b = b && loadEnzymeFloat(attr.InnerText, setAlpha);
            break;
          case "EnergyCost":
            b = b && loadEnzymeFloat(attr.InnerText, setEnergyCost);
            break;
          case "beta":
            b = b && loadEnzymeFloat(attr.InnerText, setBeta);
            break;
          case "Km":
            b = b && loadEnzymeFloat(attr.InnerText, setKm);
            break;
          case "Ki":
            b = b && loadEnzymeFloat(attr.InnerText, setKi);
            break;
          case "Products":
            b = b && loadEnzymeReactionProducts(attr);
            break;
        }
      }
      return b && hasValidData();
    }

    
    public override bool hasValidData()
    {
      bool valid = base.hasValidData()
        && !string.IsNullOrEmpty(_substrate)            //!< The substrate of the reaction
        && !string.IsNullOrEmpty(_enzyme)               //!< The enzyme of the reaction
      //protected float _Kcat;                  //!< Reaction constant of enzymatic reaction
        && !string.IsNullOrEmpty(_effector);            //!< The effector of the reaction
      //protected float _alpha;                 //!< Alpha descriptor of the effector
      //protected float _beta;                  //!< Beta descriptor of the effector
      //protected float _Km;                    //!< Affinity coefficient between substrate and enzyme
      //protected float _Ki;                    //!< Affinity coefficient between effector and enzyme;
      if(valid)
      {
        if((0 == _alpha)
           || (0 == _Ki)
           || (0 == _Km))
        {
          //TODO check also _Kcat, _beta
          Logger.Log ("EnzymeReaction::hasValidData please check values of "
            + "alpha="+_alpha
            + ", Ki="+_Ki
            + ", Km="+_Km
            +" for reaction "+this.getName()
            , Logger.Level.WARN);
        }
      }
      else
      {
        Debug.LogError(
                "EnzymeReaction::hasValidData base.hasValidData()="+base.hasValidData()
                +" & !string.IsNullOrEmpty(_substrate)="+!string.IsNullOrEmpty(_substrate)
                +" & !string.IsNullOrEmpty(_enzyme)="+!string.IsNullOrEmpty(_enzyme)
                +" & !string.IsNullOrEmpty(_effector)="+!string.IsNullOrEmpty(_effector)
                +" => valid="+valid);
      }
      return valid;
    }

    private delegate void  StrSetter(string dst);
    private delegate void  FloatSetter(float dst);
    
    
    /*!
    \brief Load and parse a string and give it to the given setter
    \param value The string to parse and load
    \param setter The delegate setter
   */
    private bool loadEnzymeString(string value, StrSetter setter)
    {
      if (String.IsNullOrEmpty(value))
        return false;
      setter(value);
      return true;    
    }
    
    /*!
    \brief Load and parse a string and give it to the given setter
    \param value The string to parse and load
    \param setter The delegate setter
   */
    private bool loadEnzymeFloat(string value, FloatSetter setter)
    {
      if (String.IsNullOrEmpty(value))
      {
        Logger.Log("EnzymeReaction::loadEnzymeReactionProducts : Empty productionMax field"
                       , Logger.Level.ERROR);
        return false;
      }
      setter(float.Parse(value.Replace(",", ".")));
      return true;    
    }
    
    /*!
    \brief Load products of an enzymatic reaction
    \param node The xml node to load
    \return Return always true
   */
    private bool loadEnzymeReactionProducts(XmlNode node)
    {
      foreach (XmlNode attr in node)
      {
        if (attr.Name == "name")
        {
          if (String.IsNullOrEmpty(attr.InnerText))
          {
            Logger.Log("EnzymeReaction::loadEnzymeReactionProducts : Empty name field in Enzyme Reaction definition"
                               , Logger.Level.ERROR);
            return false;
          }
          Product prod = new Product();
          prod.setName(node.InnerText);
          addProduct(prod);
        }
      }
      return true;
    }
}
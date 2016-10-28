using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/*!
  \brief This class represents an Allostery reaction and can be loaded by the ReactionEngine class.
  
  \sa Allostery
 */
public class AllosteryProperties
{
  public string name;
  public string effector;
  public string protein;
  public string product;
  public float K;
  public int n;
  public float energyCost;
}

/*!
 *  \brief     Manages "allostery" reactions
 *  \details   This class manages all the "allostery" reactions.

Manages allosteric regulations.

This class manages allosteric regulation of an enzyme, or any other protein, by
an effector molecule that binds to the protein's allosteric site.
This site is different from its active site, and the effector's binding can
enhance (allosteric activators) or decrease (allosteric inhibitors) the protein's activity.
Allosteric regulations allow the design of control loops within the cell's
activity (such as feedback or feedforward from upstream substrates).
In this simulation, the allosteric reaction between a protein (P.) and its effector (E)
 produces a new product (complex PE), and consumes the reactants (P and E).
 //TODO check next sentence
The complex PE can therefore be designed in order to have increased or decreased activity that the protein P.

        P + E -> PE (consume P (protein) and E(enzyme) in order to produce PE (product))

Biological example:
P: LacI (transcription factor that binds to pLac promoter)
E: IPTG
PE: LacI-IPTG (LacI is not longer able to bind to pLac with the same affinity)


 
Allosteric reactions should be describe in a reaction file inside a xml node named <reactions>

See example of definition :

        <allostery>
         <name>inhibitLacI</name>
         <effector>IPTG</effector>
         <K>0.1</K>
         <n>2</n>
         <protein>LacI</protein>
         <products>LacI*</products>
        </allostery>

    \attention All the molecules used in this reaction should be defined in a reaction file
    
    
 */
public class Allostery : IReaction
{
  private string _effector;             //! The name of the effector
  private float _K;                     //! The binding affinity between the effector and the protein
  private int _n;                       //! Steepness of the HillFunction
  private string _protein;              //! The name of the protein
  private string _product;              //! The name of the product


  public string getEffector() { return _effector; }
  public void setEffector(string str) { _effector = str; }
  public float getK() { return _K; }
  public void setK(float value) { _K = value;}
  public int getN() { return _n; }
  public void setN(int value) { _n = value;}
  public void setProtein(string str) { _protein = str;}
  public string getProtein() { return _protein; }
  public void setProduct(string str) { _product = str;}
  public string getProduct() { return _product; }

  //! Default constructor
  public Allostery()
  {}

  //! Copy constructor
  public Allostery(Allostery r) : base(r)
  {
    _effector = r._effector;
    _K = r._K;
    _n = r._n;
    _protein = r._protein;
    _product = r._product;
  }

  /* !
    \brief Checks that two reactions have the same Allostery field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected override bool PartialEquals(IReaction reaction)
  {
    Allostery allostery = reaction as Allostery;
    return (allostery != null)
    && base.PartialEquals(reaction) 
    && (_effector   == allostery._effector)
    && (_K          == allostery._K)
    && (_n          == allostery._n)
    && (_protein    == allostery._protein)
    && (_product    == allostery._product);
  }

  //FIXME : Create function that create prop with this reaction

  /*!
    \brief This function create a new Allostery reaction from an AllosteryProperties
    \param props Properties of the reaction
    \return This function return a new Allostery reaction or null if props is null
   */
  public static IReaction       buildAllosteryFromProps(AllosteryProperties props)
  {
    if (props == null)
      return null;

    Allostery reaction = new Allostery();

    reaction.setName(props.name);
    reaction.setEffector(props.effector);
    reaction.setK(props.K);
    reaction.setN(props.n);
    reaction.setProtein(props.protein);
    reaction.setProduct(props.product);
    reaction.setEnergyCost(props.energyCost);

    return reaction;
  }

  //! \brief Do all the allostery reaction for one tick
  /*! \details This function does this computation:
   
       delta = ( ([E] / K)^n ) / ( 1 + (([E] / K)^n) ) * [P]
       [P] -= delta
       [E] -= delta
       [Prod] += delta

       With:
                P -> Protein
                E -> Effector
                Prod -> Product
                

    Reference : http://2007.igem.org/wiki/index.php?title=ETHZ/Model#Mathematical_Model
    \param molecules Molecule list of the medium where the reaction take place
   */
  public override void react(ArrayList molecules)
  {
    if (!_isActive)
      return;

    float delta;
    float m;
    Molecule effector = ReactionEngine.getMoleculeFromName(_effector, molecules);
    Molecule protein = ReactionEngine.getMoleculeFromName(_protein, molecules);
    Molecule product = ReactionEngine.getMoleculeFromName(_product, molecules);

    if (effector == null)
      Debug.Log("Cannot find effector molecule named : " + effector);
    else if (protein == null)
      Debug.Log("Cannot find protein molecule named : " + protein);
    else if (product == null)
      Debug.Log("Cannot find product molecule named : " + product);
    else
      {
        m = (float)Math.Pow(effector.getConcentration() / _K, _n);
        delta =  (m / (1 + m)) * protein.getConcentration() * _reactionSpeed * ReactionEngine.reactionsSpeed;

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
          {
            product.addConcentration(delta);
            protein.subConcentration(delta);
            effector.subConcentration(delta);            
          }
        else
          {
            product.addNewConcentration(delta);
            protein.subNewConcentration(delta);
            effector.subNewConcentration(delta);
          }
      }
  }


    //loading
    /*
      An allostery reaction's declaration should respect this syntax:

    <allostery>
      <name>inhibitLacI</name>
      <effector>IPTG</effector>
      <EnergyCost>0.3</EnergyCost>
      <K>0.1</K>
      <n>2</n>
      <protein>LacI</protein>
      <products>LacI*</products>
    </allostery>
     */ 

    public override bool tryInstantiateFromXml(XmlNode node)
    {
      bool b = true;
      foreach (XmlNode attr in node)
      {
        switch (attr.Name)
        {
          case "name":
            b = b && loadAllosteryString(attr.InnerText, setName);
            break;
          case "effector":
            b = b && loadAllosteryString(attr.InnerText, setEffector);
            break;
          case "K":
            b = b && loadAllosteryFloat(attr.InnerText, setK);
            break;
          case "EnergyCost":
            b = b && loadAllosteryFloat(attr.InnerText, setEnergyCost);
            break;
          case "n":
            setN(Convert.ToInt32(attr.InnerText));
            break;
          case "protein":
            b = b && loadAllosteryString(attr.InnerText, setProtein);
            break;
          case "products":
            b = b && loadAllosteryString(attr.InnerText, setProduct);
            break;
        }
      }
        //TODO method that checks IReaction's loading
        // + method that checks child classes loading
        //done for Allostery, should do the same for other reactions
        b = b && hasValidData();

        if(b)
        {
            Debug.Log(this.GetType() + " Allostery::tryInstantiateFromXml success"
                        );
            return true;
        }
        else
        {
            Debug.LogError(this.GetType() + " tryInstantiateFromXml failed");
            return false;
        }
    }

    public override bool hasValidData()
    {
        return base.hasValidData() &&
            !string.IsNullOrEmpty(_effector)               //! The name of the effector
            //TODO private float _K;                       //! The binding affinity between the effector and the protein
            && (0 != _n) //TODO better check               //! Steepness of the HillFunction
            && !string.IsNullOrEmpty(_protein)             //! The name of the protein
            && !string.IsNullOrEmpty(_product);            //! The name of the product
    }
    
    private delegate void  StrSetter(string dst);
    private delegate void  FloatSetter(float dst);
    
    /*!
\brief This function load and parse a string and give it to the given setter
\param value The string to parse and load
\param setter The delegate setter
\return Return true is success false otherwise
  */
    private bool loadAllosteryString(string value, StrSetter setter)
    {
      if (String.IsNullOrEmpty(value))
      {
        Debug.LogError(this.GetType() + " loadAllosteryString empty name field");
        return false;
      }
      setter(value);
      return true;    
    }
    
    /*!
\brief This function load and parse a string and give it to the given setter
\param value The string to parse and load
\param setter The delegate setter
\return Return true is success false otherwise
  */
    private bool loadAllosteryFloat(string value, FloatSetter setter)
    {
      if (String.IsNullOrEmpty(value))
        {
            Debug.LogError(this.GetType() + " loadAllosteryString empty productionMax field");
        return false;
      }
      setter(float.Parse(value.Replace(",", ".")));
      return true;    
    }
}
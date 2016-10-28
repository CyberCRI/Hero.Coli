using System.Collections.Generic;
using System.Collections;
using System.Xml;
using UnityEngine;

/*!
  \brief This class is an interface that each reaction has to inherit.
  \details It contains a name, a list of products, and an activation boolean.
  The react() function should be implemented in child classes.
 */
public abstract class IReaction : LoadableFromXmlImpl
{
    protected string _name;                       //!< The name of the reaction
    protected LinkedList<Reactant> _reactants;    //!< The list of reactants
    protected LinkedList<Product> _products;      //!< The list of products
    protected bool _isActive;                     //!< Activation boolean
    protected Medium _medium;                     //!< The medium where the reaction will be executed
    protected float _reactionSpeed;               //!< Speed coefficient of the reaction
    protected float _energyCost;                  //!< Energy consumed by the reaction
    public bool enableSequential;
    public bool enableEnergy;

    protected const string nameTag = "name";
    protected const string reactantsTag = "reactants";
    protected const string productsTag = "products";
    protected const string reactantTag = "reactant";
    protected const string productTag = "product";
    protected const string energyCostTag = "EnergyCost";
    protected const string stoichiometricCoefficientTag = "v";
    protected const string orderTag = "n";

    //! Default constructor
    public IReaction()
    {
        _products = new LinkedList<Product>();
        _reactants = new LinkedList<Reactant>();

        _isActive = true;
        _reactionSpeed = 1f;
        _energyCost = 0f;

        //TODO CHECK THIS
        //enableSequential = true;
        //enableEnergy = false;
        enableSequential = ReactionEngine.get().enableSequential;
        enableEnergy = ReactionEngine.get().enableEnergy;
    }

    //! Copy Constructor
    public IReaction(IReaction r)
    {
        _products = new LinkedList<Product>();
        foreach (Product product in r._products)
        {
            _products.AddLast(product);
        }
        _reactants = new LinkedList<Reactant>();
        foreach (Reactant reactant in r._reactants)
        {
            _reactants.AddLast(reactant);
        }
        _isActive = r._isActive;
        _reactionSpeed = r._reactionSpeed;
        _energyCost = r._energyCost;
        enableSequential = r.enableSequential;
        enableEnergy = r.enableEnergy;
        _medium = r._medium;
    }

    public void setName(string name) { _name = Tools.epurStr(name); }
    public string getName() { return _name; }
    public void setReactionSpeed(float speed) { _reactionSpeed = speed; }
    public float getReactionSpeed() { return _reactionSpeed; }
    public float getEnergyCost() { return _reactionSpeed; }
    public void setEnergyCost(float energy) { _energyCost = energy; }
    public Medium getMedium() { return _medium; }
    public void setMedium(Medium med) { _medium = med; }


    /*!
      \brief This function copies a reaction by calling its real copy constructor
      (not IReaction constructor but for example PromoterReaction constructor)
      \param r Reaction to copy
      \return Return a reference on the new reaction or null if the given reaction is unknown.
     */
    public static IReaction copyReaction(IReaction r)
    {
        if (r as Degradation != null)
            return new Degradation(r as Degradation);
        if (r as PromoterReaction != null)
            return new PromoterReaction(r as PromoterReaction);
        if (r as LawOfMassActionReaction != null)
            return new LawOfMassActionReaction(r as LawOfMassActionReaction);
        if (r as Allostery != null)
            return new Allostery(r as Allostery);
        if (r as EnzymeReaction != null)
            return new EnzymeReaction(r as EnzymeReaction);
        if (r as ActiveTransportReaction != null)
            return new ActiveTransportReaction(r as ActiveTransportReaction);
        if (r as FickReaction != null)
            return new FickReaction(r as FickReaction);
        if (r as ATPProducer != null)
            return new ATPProducer(r as ATPProducer);
        if (r as InstantReaction != null)
            return new InstantReaction(r as InstantReaction);
        return null;
    }

    //! This function should be implemented by each reaction that inherit from this class.
    //! It's called at each tick of the game.
    public abstract void react(ArrayList molecules);

    /*! 
      \brief Add a Product to the product list.
      \param product The product to be added to the list
     */
    public void addProduct(Product product) { if (product != null) _products.AddLast(product); }
    /*! 
      \brief Add a Reactant to the reactant list.
      \param product The reactant to be added to the list
     */
    public void addReactant(Reactant reactant) { if (reactant != null) _reactants.AddLast(reactant); }

    public override string ToString()
    {
        string productString = "(null)";
        if (null != _products)
        {
            productString = _products.Count.ToString();
        }
        string mediumString = "(null)";
        if (null != _medium)
        {
            mediumString = "[" + _medium.getName() + ", id#" + _medium.getId() + "]";
        }

        return string.Format("IReaction[name:{0}, products:{1}, isActive:{2}, medium:{3}, "
                              + "reactionSpeed:{4}, energyCost:{5}, enableSequential:{6}, enableEnergy:{7} ]",
                              _name,                                     //!< The name of the reaction
                              productString,                             //!< The list of products
                              _isActive,                                 //!< Activation boolean
                              mediumString,                              //!< The medium where the reaction will be executed
                              _reactionSpeed,                            //!< Speed coefficient of the reaction
                              _energyCost,                               //!< Energy consumed by the reaction
                              enableSequential,
                              enableEnergy
                              );
    }

    public string ToStringDetailed()
    {
        string productString = "(null)";
        if (null != _products)
        {
            productString = Logger.ToString<Product>(_products);
        }
        string mediumString = "(null)";
        if (null != _medium)
        {
            mediumString = _medium.ToString();
        }

        return string.Format("IReaction[name:{0}, products:{1}, isActive:{2}, medium:{3}, "
                              + "reactionSpeed:{4}, energyCost:{5}, enableSequential:{6}, enableEnergy:{7} ]",
                              _name,                                     //!< The name of the reaction
                              productString,                             //!< The list of products
                              _isActive,                                 //!< Activation boolean
                              mediumString,                              //!< The medium where the reaction will be executed
                              _reactionSpeed,                            //!< Speed coefficient of the reaction
                              _energyCost,                               //!< Energy consumed by the reaction
                              enableSequential,
                              enableEnergy
                              );
    }


    /* !
      \brief Checks that two reactions are equal.
      \param reaction The reaction that will be compared to 'this'.
      \param nameMustMatch Whether the name must be taken into account or not.
     */
    public bool Equals(IReaction reaction, bool checkNameAndMedium)
    {
        if (checkNameAndMedium)
        {
            return Equals(reaction);
        }
        return PartialEquals(reaction);
    }


    /* !
      \brief Checks that two reactions have the same IReaction field values
      except for medium
      \param reaction The reaction that will be compared to 'this'.
     */
    protected virtual bool PartialEquals(IReaction reaction)
    {
        //TODO check this
    // if(!hasValidData() || !reaction.hasValidData())
    // {
    //     Debug.LogError(this.GetType() + " PartialEquals invalid reaction");
    //     return false;
    // }

        bool res =
             LinkedListExtensions.Equals(_products, reaction._products)
          && (_isActive == reaction._isActive)
          && (_reactionSpeed == reaction._reactionSpeed)
          && (_energyCost == reaction._energyCost)
          && (enableSequential == reaction.enableSequential)
          && (enableEnergy == reaction.enableEnergy)
          ;

        return res;
    }

    //TODO check chemistry
    public virtual bool hasValidData()
    {
        bool isValid =
          !string.IsNullOrEmpty(_name)           //!< The name of the reaction
          && 0 != _products.Count                //!< The list of products
                                                 //protected bool _isActive;              //!< Activation boolean
                                                 //? && null != _medium                     //!< The medium where the reaction will be executed
          && 0 != _reactionSpeed                 //!< Speed coefficient of the reaction
                                                 //? && 0 != _energyCost                   //!< Energy consumed by the reaction
                                                 //public bool enableSequential
                                                 //public bool enableEnergy
                ;

        if (!isValid)
        {
            Debug.LogError(this.GetType() + " hasValidData !string.IsNullOrEmpty(_name)=" + (!string.IsNullOrEmpty(_name))
              + " & 0 != _products.Count=" + (0 != _products.Count)
              + " & null != _medium=" + (null != _medium)
              + " & 0 != _reactionSpeed=" + (0 != _reactionSpeed)
              + " & 0 != _energyCost=" + (0 != _energyCost)
              + " => valid=" + isValid
              );
        }
        return isValid;
    }


    /*
      A reaction should respect this base syntax:

              <Reaction>
                <name>Water</name>                        -> Name of the reaction
                <EnergyCost>0.1</EnergyCost>              -> Energy cost of the reaction
                <reactants>
                  <reactant>
                    <name>O</name>                        -> Reactant name
                    <v>1</v>                              -> Reactant stoichiometric coefficient
                    <n>1</n>                              -> Reactant order in the rate equation
                  </reactant>
                  <reactant>
                    <name>H</name>
                    <v>2</v>                              
                    <n>2</n>                              
                  </reactant>
                </reactants>
                <products>
                  <product>
                    <name>H2O</name>                      -> Product name
                    <v>1</v>                              -> Product stoichiometric coefficient
                  </product>
                </products>
              </Reaction>
     */

    /*!
    \brief Parse and load reactants of a Reaction
    \param node The xml node to parse
    \return return always true
   */
    private bool loadReactionReactants(XmlNode node)
    {
        bool b = false;

        foreach (XmlNode attr in node)
        {
            if (attr.Name == reactantTag)
            {
                if (!loadReactionReactant(attr))
                {
                    Debug.LogError(this.GetType() + " loadReactionReactants loadReactionReactant failed");
                    return false;
                }
                else
                {
                    b = true;
                }
            }
            else
            {
                Debug.LogError(this.GetType() + " loadReactionReactants bad attr name:" + attr.Name);
                return false;
            }
        }

        if (!b)
        {
            Debug.LogError(this.GetType() + " loadReactionReactants loaded nothing");
            return false;
        }
        else
        {
            Logger.Log("Reaction::loadReactionReactants loaded successfully " + this
              );
            return true;
        }

    }

    /*!
    \brief Parse and load reactant of a Reaction
    \param node The xml node to parse
    \return return always true
  */
    private bool loadReactionReactant(XmlNode node)
    {
        Reactant reactant = new Reactant();
        foreach (XmlNode attr in node)
        {
            if (attr.Name == nameTag)
            {
                if (string.IsNullOrEmpty(attr.InnerText))
                {
                    Debug.LogError(this.GetType() + " loadReactionReactant Empty name field in instant reaction reactant definition");
                    return false;
                }
                reactant.setName(attr.InnerText);
            }
            else if (attr.Name == stoichiometricCoefficientTag)
            {
                if (string.IsNullOrEmpty(attr.InnerText))
                {
                    Debug.LogError(this.GetType() + " loadReactionReactant Empty quantity field in instant reaction reactant definition");
                    return false;
                }
                reactant.v = float.Parse(attr.InnerText.Replace(",", "."));
            }
        }
        addReactant(reactant);
        return true;
    }

    /*!
    \brief Parse and load reactant of a Reaction
    \param node The xml node to parse
    \return return always true
  */
    private bool loadReactionProducts(XmlNode node)
    {
        bool b = true;
        foreach (XmlNode attr in node)
        {
            //TODO should this be "if (b && (attr.Name == "product"))" ?
            if (attr.Name == productTag)
            {
                b = b && loadReactionProduct(attr);
            }
        }
        return b;
    }

    /*!
    \brief Parse and load products of a Reaction
    \param node The xml node to parse
    \return return always true
  */
    private bool loadReactionProduct(XmlNode node)
    {
        Product product = new Product();
        foreach (XmlNode attr in node)
        {
            if (attr.Name == nameTag)
            {
                if (string.IsNullOrEmpty(attr.InnerText))
                {
                    Debug.LogError(this.GetType() + " loadReactionProduct Empty name field in instant reaction product definition");
                    return false;
                }
                product.setName(attr.InnerText);
            }
            else if (attr.Name == stoichiometricCoefficientTag)
            {
                if (string.IsNullOrEmpty(attr.InnerText))
                {
                    Debug.LogError(this.GetType() + " loadReactionProduct Empty quantity field in instant reaction product definition");
                    return false;
                }
                product.v = float.Parse(attr.InnerText.Replace(",", "."));
            }
        }
        addProduct(product);
        return true;
    }

    /*!
    \brief Parse and load the energy cost of a Reaction
    \param value The value string
    \return return always true
  */
    private bool loadEnergyCost(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogWarning(this.GetType() + " loadReactionProduct Empty EnergyCost field. default value = 0");
            setEnergyCost(0f);
        }
        else
            setEnergyCost(float.Parse(value.Replace(",", ".")));
        return true;
    }


    public override bool tryInstantiateFromXml(XmlNode node)
    {
        bool b = true;
        foreach (XmlNode attr in node)
        {
            switch (attr.Name)
            {
                case nameTag:
                    setName(attr.InnerText);
                    break;
                case reactantsTag:
                    b = b && loadReactionReactants(attr);
                    break;
                case productsTag:
                    b = b && loadReactionProducts(attr);
                    break;
                case energyCostTag:
                    b = b && loadEnergyCost(attr.InnerText);
                    break;
            }
        }

        return b && hasValidData();
    }
}

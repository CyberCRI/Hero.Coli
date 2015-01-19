using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Xml;

/*!
  \brief This class is an interface that each reaction has to inherit.
  \details It contains a name, a list of products, and an activation boolean.
  The react() function should be implemented in child classes.
 */
public abstract class IReaction : LoadableFromXmlImpl
{
  protected string _name;                       //!< The name of the reaction
  protected LinkedList<Product> _products;      //!< The list of products
  protected bool _isActive;                     //!< Activation boolean
  protected Medium _medium;                     //!< The medium where the reaction will be executed
  protected float _reactionSpeed;               //!< Speed coefficient of the reaction
  protected float _energyCost;                  //!< Energy consumed by the reaction
  public bool enableSequential;
  public bool enableEnergy;

  //! Default constructor
  public IReaction()
  {
    _products = new LinkedList<Product>();
    _isActive = true;
    _reactionSpeed = 1f;
    _energyCost = 0f;

    //TODO CHECK THIS
    //enableSequential = true;
    //enableEnergy = false;
    enableSequential = ReactionEngine.get ().enableSequential;
    enableEnergy = ReactionEngine.get ().enableEnergy;
  }

  //! Copy Constructor
  public IReaction(IReaction r)
  {
    _products = new LinkedList<Product>();
    foreach(Product p in r._products)
      _products.AddLast(p);
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
  public static IReaction       copyReaction(IReaction r)
  {
    if (r as PromoterReaction != null)
      return new PromoterReaction(r as PromoterReaction);
    if (r as Allostery != null)
      return new Allostery(r as Allostery);
    if (r as InstantReaction != null)
      return new InstantReaction(r as InstantReaction);
    if (r as EnzymeReaction != null)
      return new EnzymeReaction(r as EnzymeReaction);
    if (r as ActiveTransportReaction != null)
      return new ActiveTransportReaction(r as ActiveTransportReaction);
    if (r as FickReaction != null)
      return new FickReaction(r as FickReaction);
    if (r as ATPProducer != null)
      return new ATPProducer(r as ATPProducer);
    if (r as Degradation != null)
      return new Degradation(r as Degradation);
    return null;
  }

  //! This function should be implemented by each reaction that inherit from this class.
  //! It's called at each tick of the game.
  public abstract void react(ArrayList molecules);

  /*! 
    \brief Add a Product to the product list.
    \param prod The product to be added to the list
   */
    public void addProduct(Product prod) { if (prod != null) _products.AddLast(prod); }
    
    public override string ToString ()
    {
        string productString = "(null)";
        if(null != _products)
        {
            productString = _products.Count.ToString();
        }
        string mediumString = "(null)";
        if(null != _medium)
        {
            mediumString = "["+_medium.getName()+", id#"+_medium.getId()+"]";
        }
        
        return string.Format ("IReaction[name:{0}, products:{1}, isActive:{2}, medium:{3}, "
                              +"reactionSpeed:{4}, energyCost:{5}, enableSequential:{6}, enableEnergy:{7} ]",
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
    
    public string ToStringDetailed ()
    {
        string productString = "(null)";
        if(null != _products)
        {
          productString = Logger.ToString<Product>(_products);
        }
        string mediumString = "(null)";
        if(null != _medium)
        {
            mediumString = _medium.ToString();
        }

        return string.Format ("IReaction[name:{0}, products:{1}, isActive:{2}, medium:{3}, "
                              +"reactionSpeed:{4}, energyCost:{5}, enableSequential:{6}, enableEnergy:{7} ]",
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
    if(checkNameAndMedium)
    {
      return Equals (reaction);
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
        /*
    if(!hasValidData() || !reaction.hasValidData())
    {
        Logger.Log("IReaction::PartialEquals invalid reaction"
                   , Logger.Level.ERROR);
        return false;
    }
    */

    bool res =
         LinkedListExtensions.Equals(_products,reaction._products)
      && (_isActive        == reaction._isActive)
      && (_reactionSpeed   == reaction._reactionSpeed)
      && (_energyCost      == reaction._energyCost)
      && (enableSequential == reaction.enableSequential)
      && (enableEnergy     == reaction.enableEnergy)
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

        if(!isValid)
        {
          Logger.Log("IReaction::hasValidData !string.IsNullOrEmpty(_name)="+(!string.IsNullOrEmpty(_name))
            +" & 0 != _products.Count="+(0 != _products.Count)
            +" & null != _medium="+(null != _medium)
            +" & 0 != _reactionSpeed="+(0 != _reactionSpeed)
            +" & 0 != _energyCost="+(0 != _energyCost)
            +" => valid="+isValid
            , Logger.Level.ERROR
            );
        }
        return isValid;
    }
}

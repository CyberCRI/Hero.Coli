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
public abstract class IReaction
{
  protected string _name;                       //!< The name of the reaction
  protected LinkedList<Product> _products;      //!< The list of products
  protected bool _isActive;                     //!< Activation booleen
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
		return string.Format ("IReaction[name:{0}, products:{1}, isActive:{2}, medium:{3}, "
			+"reactionSpeed:{4}, energyCost:{5}, enableSequential:{6}, enableEnergy:{7} ]",
		  _name,                                    //!< The name of the reaction
          Logger.ToString<Product>(_products),      //!< The list of products
          _isActive,                                //!< Activation booleen
          _medium,                                  //!< The medium where the reaction will be executed
          _reactionSpeed,                           //!< Speed coefficient of the reaction
          _energyCost,                              //!< Energy consumed by the reaction
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
    return BaseCharacEquals(reaction) && CharacEquals(reaction);
  }


  /* !
    \brief Checks that two reactions have the same IReaction field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  public bool BaseCharacEquals(IReaction reaction)
  {
    bool res =
         //_products.Equals(reaction._products)
         LinkedListExtensions.Equals(_products,reaction._products)
      && (_isActive        == reaction._isActive)
      //&& _medium.Equals(reaction._medium)
      && (_reactionSpeed   == reaction._reactionSpeed)
      && (_energyCost      == reaction._energyCost)
      && (enableSequential == reaction.enableSequential)
      && (enableEnergy     == reaction.enableEnergy)
      ;

    /*
      //bool productivity =   _products.Equals(reaction._products);
      bool productivity = LinkedListExtensions.Equals<Product>(_products,reaction._products);
      bool activity = (_isActive        == reaction._isActive);
      //&& _medium.Equals(reaction._medium)
      bool reactivity = (_reactionSpeed   == reaction._reactionSpeed);
      bool costity = (_energyCost      == reaction._energyCost);
      bool sequentiality = (enableSequential == reaction.enableSequential);
      bool energetity = (enableEnergy     == reaction.enableEnergy);

    Logger.Log ("IReaction::BaseCharacEquals "
      +", productivity ="+productivity
      +", activity ="+activity
      +", reactivity ="+reactivity
      +", costity ="+costity
      +", sequentiality ="+sequentiality
      +", energetity ="+energetity
      , Logger.Level.TRACE);
    */

    return res;
  }

  /* !
    \brief Checks that two reactions have the same child class field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected abstract bool CharacEquals(IReaction reaction);
}

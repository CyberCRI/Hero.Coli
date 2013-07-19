using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/*!
  \brief This class describe a Molecule
  \details A molecule is define by :
        - A name -> Used for identification
        - A type (Enzyme, transcription factor or other)
        - A description (optionnal)
        - A concentration
        - A degradation rate -> used for degradation reaction
        - A Size -> used for fick reaction (not implemented yet)

   \author Pierre COLLET
   \mail pierre.collet91@gmail.com
 */
public class Molecule
{
  //! Define Molecule type
  public enum eType
  {
    ENZYME,
    TRANSCRIPTION_FACTOR,
    OTHER
  }

  private string _name;                 //! The name of the molecule
  private eType _type;                  //! The type of the molecule
  private string _description;          //! The description of the molecule (optionnal)
  private float _concentration;         //! The concentration of the molecule
  private float _degradationRate;       //! The degradation rate of the molecule
  private float _size;                  //! The size of the molecule (not used for now)

  //! Default constructor
  public Molecule(Molecule mol = null)
  {
    if (mol != null)
      {
        _name = mol._name;
        _type = mol._type;
        _description = mol._description;
        _concentration = mol._concentration;
        _degradationRate = mol._degradationRate;
        _size = mol._size;
      }
  }

  public string getName() {return _name; }
  public eType getType() {return _type; }
  public string getDescription() {return _description; }
  public float getConcentration() {return _concentration; }
  public float getDegradationRate() {return _degradationRate; }
  public float getSize() { return _size; }

  public void setName(string name) { _name = name; }
  public void setType(eType type) { _type = type; }
  public void setDescription(string description) { _description = description; }
  public void setConcentration(float concentration) { _concentration = concentration; if (_concentration < 0) _concentration = 0;}
  public void setDegradationRate(float degradationRate) { _degradationRate = degradationRate; }
  public void setSize(float size) { _size = size; }
  /*!
    \brief Add molecule concentration
    \param concentration The concentration
   */
  public void addConcentration(float concentration) { _concentration += concentration; if (_concentration < 0) _concentration = 0;}
  /*!
    \brief Add molecule concentration
    \param concentration The concentration
   */
  public void subConcentration(float concentration) { _concentration -= concentration; if (_concentration < 0) _concentration = 0;}
}


/*!
  \brief This classe describe a Product
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class Product
{
  protected string      _name;                  //! The name of the molecule
  protected float       _quantityFactor;        //! The factor of production

  public Product() { }
  public Product(Product p)
  {
    _name = p._name;
    _quantityFactor = p._quantityFactor;
  }

  // FIXME : Potentially add a ptr to the molecule
  public void setName(string name) { _name = Tools.epurStr(name); }
  public string getName() { return _name; }
  public void setQuantityFactor(float quantity) { _quantityFactor = quantity; }
  public float getQuantityFactor() { return _quantityFactor; }
}

/*!
  \brief This class is an interface that each reaction has to inherit.
  \details It contains a name a list of products an a activation boolean.
  The react() function should be implemented in inheritant classes.
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public abstract class IReaction
{
  protected string _name;                       //! The name of the reaction.
  protected LinkedList<Product> _products;      //! The list of products
  protected bool _isActive;                     //! Activation booleen
  protected float _reactionSpeed;               //! Speed coefficient of the reaction

  //! Default constructor
  public IReaction()
  {
    _products = new LinkedList<Product>();
    _isActive = true;
    _reactionSpeed = 1f;
  }

  public void setName(string name) { _name = Tools.epurStr(name); }
  public string getName() { return _name; }
  public void setReactionSpeed(float speed) { _reactionSpeed = speed; }
  public float getReactionSpeed() { return _reactionSpeed; }

  //! This function should be implemented by each reaction that inherit from this class.
  //! It's called at each tick of the game.
  public abstract void react(ArrayList molecules);
  /*! 
    \brief Add a Product to the product list.
    \param prod The product to be added to the list
   */
  public void addProduct(Product prod) { if (prod != null) _products.AddLast(prod); }
}

// ========================== DEGRADATION ================================

/*!
  \brief This class manages degradation reactions
  \details
  Manage the degradation of a specific molecule inside the cell or in a specific medium.
  In this simulation, the degradation is determined by the degradation constant (half-life) of a specific chemical or protein.
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class Degradation : IReaction
{
  private float _degradationRate;               //! The degradation speed
  private string _molName;                      //! Molecule name's

  //! Default constructor
  public Degradation(float degradationRate, string molName)
  {
    _degradationRate = degradationRate;
    _molName = molName;
  }

  // FIXME : use _degradationRate
  /*!
    \details The degradation reaction following the formula above:

                [X] = degradationRate * [X]

    \param molecules The list of molecules
   */
  public override void react(ArrayList molecules)
  {
    if (!_isActive)
      return;
    Molecule mol = ReactionEngine.getMoleculeFromName(_molName, molecules);
    mol.subConcentration(mol.getDegradationRate() * mol.getConcentration() * _reactionSpeed * ReactionEngine.reactionsSpeed  * 0.01f);
    //     mol.setConcentration(mol.getConcentration() - mol.getDegradationRate() * mol.getConcentration() * 1f//  * 0.05f
    //                          );
  }
}
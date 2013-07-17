using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AllosteryProprieties
{
  public string name;
  public string effector;
  public string protein;
  public string product;
  public float K;
  public int n;
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

    \attention All the molecules use in this reaction should be defined in a reaction file
    \author    Pierre COLLET
    \mail      pierre.collet91@gmail.com
 */
public class Allostery : IReaction
{
  private string _effector;             //! The name of the effector
  private float _K;                     //! The binding affinity between the effector and the protein
  private int _n;                       //! Stepness of the HillFunction
  private string _protein;              //! The name of the protein
  private string _product;              //! The name of the product


  public string getName() { return _name; }
  public void setName(string str) { _name = str; }
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


  
  public static IReaction       buildAllosteryFromProps(AllosteryProprieties props)
  {
    Allostery reaction = new Allostery();

    reaction.setName(props.name);
    reaction.setEffector(props.effector);
    reaction.setK(props.K);
    reaction.setN(props.n);
    reaction.setProtein(props.protein);
    reaction.setProduct(props.product);
    return reaction;
  }

  //! \brief Do all the allostery reaction for one tick
  /*! \details This function do this calculus :
   
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
        if (enableNoise)
          {
            float noise = _numberGenerator.getNumber();
            delta += noise;
          }
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

}
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;


/*!
  \brief This class represent a PromoterReaction and can be loaded by the simulator.
  
  \sa PromoterReaction
 */
public class PromoterProperties
{
  public string name;                           //!< The name of the reaction
  public float beta;                            //!< The maximal production rate of the promoter
  public float terminatorFactor;                //!< The Coefficient that represent the terminator
  public string formula;                        //!< The formula that drive the promoter behaviour
  public LinkedList<Product> products;          //!< The list of products
  public float energyCost;                      //!< The cost in energy
	
	
/*!
 *  \brief     ToString method
 *  \details   ToString method, with all fields, including detailed internal products
 */
  public override string ToString() {
    return "PromoterProperties["+
      "name:"+name+
      ", beta:"+beta+
      ", terminatorFactor:"+terminatorFactor+
      ", formula:"+formula+
      ", products:"+Logger.ToString<Product>(products)+
      ", energyCost:"+energyCost+"]";
  }
}

/*!
 *  \brief     Manage promoter reactions
 *  \details   This class manage all the promoter reactions

 A promoter reaction represent the behaviour of a promoter and of the transcription that it manage (Device).
 The promoter responds to a logic input function that should respect the synthax below.

 Input function:
 ==============

        Grammar :

                EXPR ::= ANDEXPR [OP_OR OREXPR]
                ANDEXPR ::= PAREXPR [OP_AND ANDEXPR]
                PAREXPR ::= (NOTEXPR | OP_LPAR OREXPR OP_RPAR)
                NOTEXPR ::= [OP_NOT] (OPERANDEXPR | BOOL_EXPR)
                BOOL_EXPR :: = (OP_TRUE | OP_FALSE)
                OPERANDEXPR ::= CONSTANTEXPR WORD
                CONSTANTEXPR ::= OP_LHOOK FLOATNUMBER OP_COMMA FLOATNUMBER OP_RHOOK
                WORD ::= CHAR [CHAR | NUMBER]
                NUMBER ::= (0|1|2|3|4|5|6|7|8|9) [NUMBER]
                CHAR ::= (a-z,A-Z)
                FLOATNUMBER ::= NUMBER [OP_DOT NUMBER]
                
        Default Operators:

                OP_OR ::= "|"
                OP_AND ::= "*"
                OP_LPAR ::= "("
                OP_RPAR ::= ")"
                OP_NOT ::= "!"
                OP_TRUE :: = "T"
                OP_FALSE ::= "F"
                OP_LHOOK ::= "["
                OP_RHOOK ::= "]"
                OP_COMMA ::= ","
                OP_DOT ::= "."
                

        Examples :

                - T                             Always true
                - F                             Always false
                - [1.2,1]X                      Activated when  [X] >= 1.2 with Stepness = 1

                - [1.3,1]X*([2.4,2]Y|[2.5,1]Z)  Activated when  [X] >= 1.3 with Stepness = 1 AND
                                                                 ([Y] >= 2.4 with Stepness = 2 OR
                                                                  [Z] >= 2.5 with stepness = 1)

                - !([1.3,2]X|[1.4,1]Y)          Activated when  [X] <= 1.3 with stepness = 2 OR
                                                                [Y] <= 1.4 with stepness = 1
                                                                ... But not only. See below to understand

                - ![0.8,1]LacI*[3.4,2]GFP       Activated when  [LacI] <= 0.8 with stepness = 1 AND
                                                                [GFP] >= 3.4 with stepness = 2
        

Synthax tree and execution:
==========================

A tree is build from the grammar above.

        Example :

                - [1.3,2]X*![1.4,1]Y create the tree below:


                                 AND
                                  |
                        ---------------------
                        |                   |
                        C                  NOT
                  ------------              |
                  |          |              C
                  1.4        X        ------------                 
                  |                   |          |
                  2                   1.4        Y
                                      |
                                      1

Execution :

Here, the tree is left recursive executed.
Each node is executed by a specific function.

See below how each kind of node ares executed :

        - AND (*) node:                         return Min(leftNode, RightNode)
        - OR (+) node:                          return Max(leftNode, rightNode)
        - Not (!) node:                         return 1 - leftNode
        - constant (C) node:                    return hill_function with  K parameter = leftNode  and  n parameter = leftNode of leftNode
        - Transcription factor:                 return concentration of concerned transcription factor
          (X or Y in the tree above) node
        - Value (1.4, 2, 1.4 and 1 above) node: return the value that it contain.
        
        /!\ hill_function = [X]^n / (K + [X]^n)
        This function can be replaced by a stepfunction that correspond to a Hill function with n = +inf



A Device will transcript all the operon and so increase concentration of molecules that it produces.
In order to do this, it needs this parameters:

                - Beta -> maximal production rate
                - Terminator factor -> between 0-1 that describe the probability that the terminator stop the transcription
                - formula -> the result value of the tree above
                - Operon :      - The Molecule that is transcripted
                                - The RBS factor (RBSf), between 0-1 that correspond to the RBS affinity with the ribosomes

To see how the calculus is done, refer you to the react() function of this class.


\attention To understand how to build a PromoterReaction refer you to the PromoterLoader class



 */
public class PromoterReaction : IReaction
{
  private float _terminatorFactor;                      //! Determine the fiability of the terminator (0-1 which correspond to 0% to 100%)
  private TreeNode<PromoterNodeData> _formula;          //! The formula describe in the detailled description
  protected float _beta;                                //! The maximal production of the promoter

  public void setBeta(float beta) { _beta = beta; }
  public float getBeta() { return _beta; }
  public void setTerminatorFactor(float v) { _terminatorFactor = v; }
  public float getTerminatorFactor() { return _terminatorFactor; }
  public void setFormula(TreeNode<PromoterNodeData> tree) { _formula = tree; }
  public TreeNode<PromoterNodeData> getFormula() { return _formula; }
	
  private bool _debug = false;

  //! Default Constructor
  public PromoterReaction()
  {
  }

  //! Copy constructor
  public PromoterReaction(PromoterReaction r) : base(r)
  {
    _terminatorFactor = r._terminatorFactor;
    _formula = r._formula;
    _beta = r._beta;
  }

  //TODO improve this
  private bool formulaEquals(TreeNode<PromoterNodeData> formula1, TreeNode<PromoterNodeData> formula2)
  {
    string f1 = Logger.ToString<PromoterNodeData>(formula1);
    string f2 = Logger.ToString<PromoterNodeData>(formula2);
    Logger.Log("PromoterReaction::formulaEquals (f1==f2)="+(f1==f2)+"f1="+f1+", f2="+f2, Logger.Level.DEBUG);
    return f1 == f2;
  }

  /* !
    \brief Checks that two reactions have the same PromoterReaction field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected override bool CharacEquals(IReaction reaction)
  {
    PromoterReaction promoter = reaction as PromoterReaction;

    bool bnullProm = (promoter != null);
    bool btermFac = (_terminatorFactor == promoter._terminatorFactor);
    bool bformula = formulaEquals(_formula, promoter._formula);
    bool bbeta = (_beta == promoter._beta);

    Logger.Log("PromoterReaction::CharacEquals"
      +", bnullProm="+bnullProm
      +", btermFac="+btermFac
      +", bformula="+bformula
      +", bbeta="+bbeta
      , Logger.Level.DEBUG);

    return (promoter != null)
    && (_terminatorFactor == promoter._terminatorFactor)
    //&& _formula.Equals(promoter._formula)
    && formulaEquals(_formula, promoter._formula)
    && (_beta == promoter._beta);
  }
  /*
  protected override bool CharacEquals(IReaction reaction)
  {
    PromoterReaction promoter = reaction as PromoterReaction;
    if (promoter != null)
    {
      PromoterReaction copy = new PromoterReaction(promoter);
      copy.setName(_name);
      copy.setMedium(_medium);
      
      bool res = Equals(copy);
      return res;
    }
    return false;

  }
  */

  /*!
    \brief This reaction build a PromoterReaction reaction from a PromoterProperties class
    \param props The PromoterProperties which will serve to create the reaction
    \return Return the new reaction or null if it fail.
   */
  public static IReaction       buildPromoterFromProps(PromoterProperties props)
  {
    if (props == null)
      return null;

    PromoterParser parser = new PromoterParser();
    PromoterReaction reaction = new PromoterReaction();

    reaction.setName(props.name);
    reaction.setBeta(props.beta);
    reaction.setTerminatorFactor(props.terminatorFactor);
    reaction.setEnergyCost(props.energyCost);
    TreeNode<PromoterNodeData> formula = parser.Parse(props.formula);
    reaction.setFormula(formula);
    Product newProd;
    foreach (Product p in props.products)
      {
        newProd = new Product(p);
        reaction.addProduct(newProd);
      }
		
    return reaction;
  }

  /*! 
    Implementation of a Hill function
    \param K Threshold value
    \param concentration Quantity of the molecule
    \param n Stepness parameter
  */
  public static float hillFunc(float K, float concentration, double n)
  {
    return (float)(Math.Pow(concentration, n) / (K + Math.Pow(concentration, n)));
  }

  /*! 
    Implementation of a step function function
    \param K Threshold value
    \param concentration Quantity of the molecule
  */
  public static float stepFunc(float K, float concentration)
  {
    if (concentration > K)
      return 1f;
    return 0f;
  }

  /*! 
    Execute a Node of type : Constant
    \param node The node of the tree to execute
    \param molecules The list of molecules
    \return The result of the hill function.
  */
  private float execConstant(TreeNode<PromoterNodeData> node, ArrayList molecules)
  {
    if (node == null)
      return 0f;

    if (node.getRightNode().getData().token == PromoterParser.eNodeType.BOOL)
      return execBool(node.getRightNode());
    Molecule mol = execWord(node.getRightNode(), molecules);
    float K = execNum(node.getLeftNode(), molecules);
    float n = 1f;
    if (node.getLeftNode() != null && node.getLeftNode().getLeftNode() != null)
      n = execNum(node.getLeftNode().getLeftNode(), molecules);
    return hillFunc(K, mol.getConcentration(), n);
  }

  /*! 
    Execute a Node of type : Word
    \param node The node of the tree to execute
    \param molecules The list of molecules
    \return return the concentration of the molecule in the node.
  */
  private Molecule execWord(TreeNode<PromoterNodeData> node, ArrayList molecules)
  {
    if (node == null || molecules == null)
      return null;
    return ReactionEngine.getMoleculeFromName(node.getData().value, molecules);
  }

  /*! 
    Execute a Node of type : Bool
    \param node The node of the tree to execute
    \return 1 if the value of the node is True, 0 else
  */
  private float execBool(TreeNode<PromoterNodeData> node)
  {
    if (node == null)
      return 0f;
    if (node.getData().value == "T")
      return 1f;
    return 0f;
  }

  /*! 
    Execute a Node of type : Num
    \param node The node of the tree to execute
    \param molecules The list of molecules
    \return The value that contain the node
  */
  private float execNum(TreeNode<PromoterNodeData> node, ArrayList molecules)
  {
    if (node == null || molecules == null)
      return 0f;
    return float.Parse(node.getData().value.Replace(",", "."));
  }

  /*! 
    Execute a Node.
    \param node The node of the tree to execute
    \param molecules The list of molecules
    \return The result of the function
  */
  private float execNode(TreeNode<PromoterNodeData> node, ArrayList molecules)
  {
    if (node != null)
      {
        if (node.getData().token == PromoterParser.eNodeType.OR)
          return Math.Max(execNode(node.getLeftNode(), molecules), execNode(node.getRightNode(), molecules));
        else if (node.getData().token == PromoterParser.eNodeType.AND)
          return Math.Min(execNode(node.getLeftNode(), molecules), execNode(node.getRightNode(), molecules));
        else if (node.getData().token == PromoterParser.eNodeType.NOT)
          return 1f - execNode(node.getLeftNode(), molecules);
        else if (node.getData().token == PromoterParser.eNodeType.CONSTANT)
          return execConstant(node, molecules);
        else if (node.getData().token == PromoterParser.eNodeType.BOOL)
          return execBool(node);
        else if (node.getData().token == PromoterParser.eNodeType.WORD)
          {
            Molecule mol = ReactionEngine.getMoleculeFromName(node.getData().value, molecules);
            if (mol != null)
              return mol.getConcentration();
          }
        else if (node.getData().token == PromoterParser.eNodeType.NUM)
          return float.Parse(node.getData().value.Replace(",", "."));
      }
    return 1.0f;
  }

  /*! 
    \brief Execute a promoter reaction as describe in the detailled reaction
    \details Once the tree is executed, the result is put in delta and used as follow :
    
    For each Product P in the operon :
    
                [P] += delta * RBSf * TerminatorFactor * beta(Maximal production)
    
    \param molecules The list of molecules
  */
  public override void react(ArrayList molecules)
  {
    if (!_isActive) {
	  if(_debug) Logger.Log("PromoterReaction::react !_isActive", Logger.Level.TRACE);
      return;
	}
    float delta = execNode(_formula, molecules);

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
	
    foreach (Product pro in _products)
      {
	    if(_debug) Logger.Log("PromoterReaction::react product="+pro, Logger.Level.TRACE);
        Molecule mol = ReactionEngine.getMoleculeFromName(pro.getName(), molecules);
			
        if( mol == null) Debug.Log("mol is null, pro.getName()="+pro.getName()+", molecules="+molecules.ToString());
        if( pro == null) Debug.Log("pro is null");
			
		float increase = delta * pro.getQuantityFactor() * _terminatorFactor * _beta
                           * ReactionEngine.reactionsSpeed * _reactionSpeed;
		
		if(Logger.isLevel(Logger.Level.TRACE)) {
		  if(_debug) Logger.Log("PromoterReaction::react increase="+increase
					+", delta:"+delta
					+", qFactor:"+pro.getQuantityFactor()
					+", tFactor:"+_terminatorFactor
					+", beta:"+_beta
                    +", reactionsSpeed:"+ReactionEngine.reactionsSpeed
					+", reactionSpeed:"+_reactionSpeed
					, Logger.Level.TRACE
					);
		}
			
        if (enableSequential) {
		  float oldCC = mol.getConcentration();
		  mol.addConcentration(increase);
		  float newCC = mol.getConcentration();
		  if(_debug) Logger.Log("PromoterReaction::react ["+mol.getName()+"]old="+oldCC
					+" ["+mol.getName()+"]new="+newCC
					, Logger.Level.TRACE
					);
        } else {
		  mol.addNewConcentration(increase);
		  if(_debug) Logger.Log("PromoterReaction::react ["+mol.getName()+"]="+mol.getConcentration()+" addNewConcentration("+increase+")"
					, Logger.Level.TRACE
					);
	    }
				
      }
  }

  public override string ToString ()
  {
    return string.Format ("Promoter[name:"+_name
			+", beta:"+_beta
			+", formula:"+Logger.ToString<PromoterNodeData>(_formula)
			+", products:"+Logger.ToString<Product>(_products)
			+", active:"+_isActive
			+", medium:"+_medium
			+", reactionSpeed:"+_reactionSpeed
			+", energyCost:"+_energyCost
			+", enableSequential:"+enableSequential
			+", enableEnergy:"+enableEnergy
			+"]");
  }

}

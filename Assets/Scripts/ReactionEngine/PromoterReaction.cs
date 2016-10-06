using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using System.Xml;

/*!
 *  \brief     Manage promoter reactions
 *  \details   This class manage all the promoter reactions

 A promoter reaction represents the behaviour of a promoter and of the transcription that it manage (Device).
 The promoter responds to a logic input function that should respect the syntax below.

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
                                                                  [Z] >= 2.5 with steepness = 1)

                - !([1.3,2]X|[1.4,1]Y)          Activated when  [X] <= 1.3 with steepness = 2 OR
                                                                [Y] <= 1.4 with steepness = 1
                                                                ... But not only. See below to understand

                - ![0.8,1]LacI*[3.4,2]GFP       Activated when  [LacI] <= 0.8 with steepness = 1 AND
                                                                [GFP] >= 3.4 with steepness = 2
        

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
                - Terminator factor -> between 0-1 that describes the probability that the terminator stop the transcription
                - formula -> the result value of the tree above
                - Operon :      - The Molecule that is transcripted
                                - The RBS factor (RBSf), between 0-1 that corresponds to the RBS affinity with the ribosomes

To see how the calculus is done, refer you to the react() function of this class.


\attention To understand how to build a PromoterReaction refer you to the PromoterLoader class



 */
public class PromoterReaction : IReaction
{
  private float _terminatorFactor;                      //! Determine the fiability of the terminator (0-1 which correspond to 0% to 100%)
  private TreeNode<PromoterNodeData> _formula;          //! The formula described in the detailed description
  protected float _beta;                                //! The maximal production of the promoter

  public void setBeta(float beta) { _beta = beta; }
  public float getBeta() { return _beta; }
  public void setTerminatorFactor(float v) { _terminatorFactor = v; }
  public float getTerminatorFactor() { return _terminatorFactor; }
  public void setFormula(TreeNode<PromoterNodeData> tree) { _formula = tree; }
  public TreeNode<PromoterNodeData> getFormula() { return _formula; }
	
  private static PromoterParser _parser = new PromoterParser();               //!< The Formula Parser

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
  protected override bool PartialEquals(IReaction reaction)
  {
    PromoterReaction promoter = reaction as PromoterReaction;

    bool bnullProm = (promoter != null);
    bool btermFac = (_terminatorFactor == promoter._terminatorFactor);
    bool bformula = formulaEquals(_formula, promoter._formula);
    bool bbeta = (_beta == promoter._beta);

    Logger.Log("PromoterReaction::PartialEquals"
      +", bnullProm="+bnullProm
      +", btermFac="+btermFac
      +", bformula="+bformula
      +", bbeta="+bbeta
      , Logger.Level.DEBUG);

    return (promoter != null)
    && base.PartialEquals(reaction)
    && (_terminatorFactor == promoter._terminatorFactor)
    //&& _formula.Equals(promoter._formula)
    && formulaEquals(_formula, promoter._formula)
    && (_beta == promoter._beta);
   }

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
    \return The value that contains the node
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
			
		float increase = delta * pro.v * _terminatorFactor * _beta
                           * ReactionEngine.reactionsSpeed * _reactionSpeed;
		
		if(Logger.isLevel(Logger.Level.TRACE)) {
		  if(_debug) Logger.Log("PromoterReaction::react increase="+increase
					+", delta:"+delta
					+", qFactor:"+pro.v
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


    // Xml loading
    
    /*!
  \brief This class loads promoters reactions from xml files
  \details

A PromoterReaction should respect this syntax:

        <promoter>
          <name>ptet</name>                           -> The name of the reaction
          <productionMax>100</productionMax>          -> The maximal production speed of the promoter
          <terminatorFactor>1</terminatorFactor>      -> between 0 and 1, represents the Terminator
          <formula>![0.8,3]tetR</formula>             -> The formula that manage the behaviour of the promoter (see PromoterReaction class for more infos)
          <EnergyCost>0.1</EnergyCost>                -> The cost in energy
          <operon>
            <gene>
              <name>RFP</name>                        -> The molecule name of a product
              <RBSFactor>0.12</RBSFactor>             -> The RBS factor that represents the affinity between Ribosome and RBS
            </gene>
            <gene>
              <name>LacI</name>
              <RBSFactor>0.12</RBSFactor>
            </gene>
          </operon>
        </promoter>

  \sa PromoterReaction
  
 */
  public override bool tryInstantiateFromXml(XmlNode node)
  {
    bool b = true;
    foreach (XmlNode attr in node)
    {
      switch (attr.Name)
      {
        case "name":
          b = b && loadPromoterName(attr.InnerText);
          break;
        case "productionMax":
          b = b && loadPromoterProductionMax(attr.InnerText);
          break;
        case "terminatorFactor":
          b = b && loadPromoterTerminatorFactor(attr.InnerText);
          break;
        case "EnergyCost":
          b = b && loadEnergyCost(attr.InnerText);
          break;
        case "formula":
          b = b && loadPromoterFormula(attr.InnerText);
          break;
        case "operon":
          b = b && loadPromoterOperon(attr);
          break;
      }
    }
    return b && hasValidData();
  }

    public override bool hasValidData()
    {        
      bool valid = base.hasValidData()
          && 0 <= _terminatorFactor                      //! Determine the fiability of the terminator (0-1 which correspond to 0% to 100%)
          && 1 >= _terminatorFactor
          && null != _formula;                           //! The formula described in the detailed description
      
      if(valid)
      {
        if(0 == _beta)                                 //! The maximal production of the promoter
        {
          Logger.Log ("PromoterReaction::hasValidData please check that you really intended a max production rate (beta) of 0 " +
                      "for promoter reaction "+this.getName()
                      , Logger.Level.WARN);
        }
      }
      else
      {
            Logger.Log(
                 "PromoterReaction::hasValidData base.hasValidData()="+(base.hasValidData())
                +" & 0 <= _terminatorFactor="+(0 <= _terminatorFactor)
                +" & 1 >= _terminatorFactor="+(1 >= _terminatorFactor)
                +" & null != _formula="+(null != _formula)
                +" => valid="+valid
                , Logger.Level.ERROR
                );
      }
      return valid;
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


    ///////////////////////////////////////////////////////////////////////////
    /// loading ///////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    
    /*!
    \brief Load promoter name by checking the validity of the given string
    \param value The given name
    \return Return true if succeeded and false if value parameter is invalid.
   */
    private bool loadPromoterName(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            Debug.Log("Error: Empty name field");
            return false;
        }
        setName(value);
        return true;
    }
    
    /*!
    \brief Load promoter maximal production speed by checking the validity of the given string
    \param value The given maximal production
    \return Return true if succeeded and false if value parameter is invalid.
   */
    private bool loadPromoterProductionMax(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            Debug.Log("Error: Empty productionMax field");
            return false;
        }
        setBeta(float.Parse(value.Replace(",", ".")));
        return true;
    }
    
    /*!
    \brief Load promoter terminator factor by checking the validity of the given string
    \param value The given terminator factor
    \return Return true if succeeded and false if value parameter is invalid.
   */
    private bool loadPromoterTerminatorFactor(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            Debug.Log("Error: Empty TerminatorFactor field");
            return false;
        }
        setTerminatorFactor(float.Parse(value.Replace(",", ".")));
        return true;
    }
    
    /*!
    \brief Load promoter energy cost by checking the validity of the given string
    \param value The given energy cost
    \return Return true if succeeded and false if value parameter is invalid.
   */
    private bool loadEnergyCost(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            Debug.Log("Error: Empty EnergyCost field. default value = 0");
            setEnergyCost(0f);
        }
        else
            setEnergyCost(float.Parse(value.Replace(",", ".")));
        return true;
    }
    
    /*!
    \brief Load promoter gene by checking the validity of the given strings
    \param name The name of the molecule that the gene will produce
    \param RBSf The Ribosome Binding Site factor string
    \return Return true if succeeded and false if value parameter is invalid.
   */
    private bool loadGene(string name, string RBSf)
    {
        if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(RBSf))
        {
            Debug.Log("Error: Empty Gene name field");
            return false;
        }

        Product gene = new Product(name, float.Parse(RBSf.Replace(",", ".")));
        addProduct(gene);
        return true;
    }
    
    /*!
    \brief Load promoter operon
    \param node the xml node
    \return Return true if succeeded and false if value parameter is invalid.
   */
    private bool loadPromoterOperon(XmlNode node)
    {
        string name = null;
        string RBSf = null;
        bool n = false;
        bool rbsf = false;
        bool b = true;
        
        foreach (XmlNode gene in node)
        {
            n = false;
            rbsf = false;
            foreach(XmlNode attr in gene)
            {
                switch (attr.Name)
                {
                    case "name":
                        name = attr.InnerText;
                        n = true;
                        break;
                    case "RBSFactor":
                        RBSf = attr.InnerText;
                        rbsf = true;
                        break;
                }
            }
            if (n && rbsf)
                b = b && loadGene(name, RBSf);
            if (!n)
                Debug.Log("Error : Missing Gene name in operon");
            if (!rbsf)
                Debug.Log("Error : Missing RBSfactor in operon");
        }
        return b;
    }
    
    /*!
    \brief Load promoter formula by checking the validity of the given string
    \param formula The given formula
    \return Return true if succeeded and false if value parameter is invalid.
  */
    private bool loadPromoterFormula(string formula)
    {
        TreeNode<PromoterNodeData> tree = _parser.Parse(formula);
        
        if (tree == null)
        {
            Debug.Log("Syntax Error in promoter Formula");
            return false;
        }
        setFormula(tree);
        return true;
    }

}

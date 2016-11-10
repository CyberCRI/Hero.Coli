using UnityEngine;

public abstract class BioBrick: DNABit
{
  public enum Type
  {
    PROMOTER,
    RBS,
    GENE,
    TERMINATOR,
    UNKNOWN
  }
  
  public enum Status
  {
      STRANDED,
      STORED,
      CRAFTED
  }

  protected string _name;
  protected int _size;
  protected Type _type;

  public static bool isUnlimited = false;
  public void setName(string name) { _name = name; }
  public string getName() { return _name; }
  public override string getInternalName () { return _name; }
  public void setSize(int size)
  {
      _size = size;

        if(0 == size)
        {
            Debug.LogWarning(this.GetType() + " size set to 0 in " + _name);
        }
  }
  public int getSize() { return _size; }
  public Type getType() { return _type; }
  
  protected double _amount = 0;
  public double amount {get{return _amount;}} 
  
  public void addAmount(double increase)
  {
      _amount += increase;
  }

  public BioBrick(Type type, double _count = -1)
  {
    _type = type;
    isUnlimited = _amount==0?MemoryManager.get().configuration.getMode()==GameConfiguration.GameMode.SANDBOX:isUnlimited;
    _amount = (0 > _count) ? (isUnlimited?double.PositiveInfinity:1) : _count;
  }

  public abstract BioBrick copy(double _count = -1);
    
    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }
        
        BioBrick b = obj as BioBrick;
        if ((System.Object)b == null)
        {
            return false;
        }

        bool result;

        // check type, name, length
        return (this._type == b._type) && (this._name == b._name) && (this._size == b._size);
    }
}

public class PromoterBrick : BioBrick
{
    
    public enum Regulation
    {
        CONSTANT,
        ACTIVATED,
        REPRESSED,
        BOTH
    }

    private float _beta;
    private string _formula;
    public string formula
    {
        get { return _formula; }
        set
        {
            _formula = value;

            _regulation = Regulation.CONSTANT;
            
            if (!string.IsNullOrEmpty(value) && value != "T")
            {
                
                PromoterParser parser = new PromoterParser();
                TreeNode<PromoterNodeData> formulaTree = parser.Parse(value);
                bool activated = false, repressed = false;
                
                recGetPromoterType(formulaTree, out activated, out repressed);
                
                if (repressed && !activated)
                {
                    _regulation = Regulation.REPRESSED;                
                }
                else if(!repressed && activated)
                {
                    _regulation = Regulation.ACTIVATED;
                }
                else if (repressed && activated)
                {
                    _regulation = Regulation.BOTH;
                }
            }
        }
    }
    
    private void recGetPromoterType(TreeNode<PromoterNodeData> formulaTree, out bool activated, out bool repressed, TreeNode<PromoterNodeData> parentFormulaTree = null)
    {
        TreeNode<PromoterNodeData> leftFormulaTree = formulaTree.getLeftNode(), rightFormulaTree = formulaTree.getRightNode();

        if((null == leftFormulaTree) && (null == rightFormulaTree))
        {
            activated = false;
            repressed = false;
        }
        else
        {
            bool leftActivated = false, rightActivated = false, leftRepressed = false, rightRepressed = false;
        
            if (null != leftFormulaTree)
            {
                recGetPromoterType(leftFormulaTree, out leftActivated, out leftRepressed, formulaTree);
            }

            if (null != rightFormulaTree && (!leftActivated || !leftRepressed))
            {
                recGetPromoterType(rightFormulaTree, out rightActivated, out rightRepressed, formulaTree);
            }
            
            activated = leftActivated || rightActivated;
            repressed = leftRepressed || rightRepressed;
            
            if(null != parentFormulaTree)
            {
                activated = activated || isActivated(parentFormulaTree, formulaTree);
                repressed = repressed || isRepressed(parentFormulaTree, formulaTree);
            }
            else
            {
                activated = activated || formulaTree.getData().token == PromoterParser.eNodeType.CONSTANT;
            }
        }
    }

    private bool isActivated(TreeNode<PromoterNodeData> parentFormulaTree, TreeNode<PromoterNodeData> childFormulaTree)
    {
        return ((childFormulaTree.getData().token == PromoterParser.eNodeType.CONSTANT)
         && parentFormulaTree.getData().token != PromoterParser.eNodeType.NOT);
    }

    private bool isRepressed(TreeNode<PromoterNodeData> parentFormulaTree, TreeNode<PromoterNodeData> childFormulaTree)
    {
        return ((childFormulaTree.getData().token == PromoterParser.eNodeType.CONSTANT)
         && parentFormulaTree.getData().token == PromoterParser.eNodeType.NOT);
    }
    
  private Regulation _regulation = Regulation.CONSTANT;

  public void setBeta(float v) { _beta = v; }
  public float getBeta() { return _beta; }
  public void setFormula(string v) { formula = v; }
      
  public string getFormula() { return formula; }
  public Regulation getRegulation() { return _regulation; }

  public PromoterBrick() : base(BioBrick.Type.PROMOTER)
  {
  }
	
    public PromoterBrick(string name, float beta, string __formula, int size, double _count = -1) : base(BioBrick.Type.PROMOTER, _count)
    {
        _name = name;
        _beta = beta;
        formula = __formula;
        _size = size;

        if (0 == size)
        {
            Debug.LogWarning(this.GetType() + " size = 0 in " + name);
        }
    }

    public PromoterBrick(PromoterBrick p, double _count) : this(p._name, p._beta, p.formula, p._size, _count)
    {
    }

    public PromoterBrick(PromoterBrick p) : this(p, p.amount)
    {
    }
	
    public override BioBrick copy(double _count = -1)
    {
        return new PromoterBrick(this, _count);
    }

    public override bool Equals(System.Object obj)
    {
        PromoterBrick pb = obj as PromoterBrick;

        return base.Equals(obj) && (_beta == pb._beta) && (formula == pb.formula);
    }

  public override string ToString ()
  {
	return string.Format ("[PromoterBrick: name: {0}, beta: {1}, formula: {2}, size: {3}, amount: {4}]", _name, _beta, formula, _size, amount);
  }
}

public class RBSBrick : BioBrick
{
  private float _RBSFactor;

  public void setRBSFactor(float v) { _RBSFactor = v; }
  public float getRBSFactor() { return _RBSFactor; }

  public RBSBrick() : base(BioBrick.Type.RBS)
  {
  }

  public RBSBrick(string name, float RBSFactor, int size, double _count = -1) : base(BioBrick.Type.RBS, _count)
  {
        _name = name;
		_RBSFactor = RBSFactor;
        _size = size;

        if(0 == size)
        {
            Debug.LogWarning(this.GetType() + " size = 0 in " + name);
        }
  }
	
  public RBSBrick(RBSBrick r, double _count) : this(r._name, r._RBSFactor, r._size, _count)
  {
  }
	
  public RBSBrick(RBSBrick r) : this(r, r.amount)
  {
  }

    public override BioBrick copy(double _count = -1)
    {
        return new RBSBrick(this, _count);
    }
    
    public override bool Equals(System.Object obj)
    {
        RBSBrick rbsb = obj as RBSBrick;
        return base.Equals(obj) && (_RBSFactor == rbsb._RBSFactor);
    }

  public override string ToString ()
  {
	return string.Format ("[RBSBrick: name: {0}, RBSFactor: {1}, amount: {2}]", _name, _RBSFactor, amount);
  }
}

public class GeneBrick : BioBrick
{
  private string _proteinName;
           
  public void setProteinName(string name) { _proteinName = name; }
  public string getProteinName() { return _proteinName; }

  public GeneBrick() : base(BioBrick.Type.GENE)
  {
  }

  public GeneBrick(string name, string proteinName, int size, double _count = -1) : base(BioBrick.Type.GENE, _count)
  {
    _name = name;
		_proteinName = proteinName;
        _size = size;

        if(0 == size)
        {
            Debug.LogWarning(this.GetType() + " size = 0 in " + name);
        }
  }

  public GeneBrick(GeneBrick g, double _count) : this(g._name, g._proteinName, g._size, _count)
  {
  }

  public GeneBrick(GeneBrick g) : this(g, g.amount)
  {
  }

    public override BioBrick copy(double _count = -1)
    {
        return new GeneBrick(this, _count);
    }
    
    public override bool Equals(System.Object obj)
    {
        GeneBrick gb = obj as GeneBrick;
        return base.Equals(obj) && (_proteinName == gb._proteinName);
    }
	
  public override string ToString ()
  {
	return string.Format ("[GeneBrick: name: {0}, proteinName: {1}, amount: {2}]", _name, _proteinName, amount);
  }
}

public class TerminatorBrick : BioBrick
{
  protected float _terminatorFactor;

  public void setTerminatorFactor(float v) { _terminatorFactor = v; }
  public float getTerminatorFactor() { return _terminatorFactor; }

  public TerminatorBrick() : base(BioBrick.Type.TERMINATOR)
  {
  }

  public TerminatorBrick(string name, float terminatorFactor, int size, double _count = -1) : base(BioBrick.Type.TERMINATOR, _count)
  {
        _name = name;
		_terminatorFactor = terminatorFactor;
        _size = size;

        if(0 == size)
        {
            Debug.LogWarning(this.GetType() + " size = 0 in " + name);
        }
  }

  public TerminatorBrick(TerminatorBrick t, double _count) : this(t._name, t._terminatorFactor, t._size, _count)
  {
  }

  public TerminatorBrick(TerminatorBrick t) : this(t, t.amount)
  {
  }

    public override BioBrick copy(double _count = -1)
    {
        return new TerminatorBrick(this, _count);
    }
    
    public override bool Equals(System.Object obj)
    {
        TerminatorBrick tb = obj as TerminatorBrick;
        return base.Equals(obj) && (_terminatorFactor == tb._terminatorFactor);
    }
	
  public override string ToString ()
  {
	  return string.Format ("[TerminatorBrick: name: {0}, terminatorFactor: {1}, amount: {2}]", _name, _terminatorFactor, amount);
  }
}
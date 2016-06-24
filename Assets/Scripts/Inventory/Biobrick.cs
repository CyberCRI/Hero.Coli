using System;
using System.Collections;
using System.Collections.Generic;


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

  public void setName(string name) { _name = name; }
  public string getName() { return _name; }
  public override string getInternalName () { return _name; }
  public void setSize(int size) { _size = size; }
  public int getSize() { return _size; }
  public Type getType() { return _type; }
  
  private double _amount = 1;//double.PositiveInfinity;
  public double amount {get{return _amount;}} 
  
  public void addAmount(double increase)
  {
      _amount += increase;
  }

  public BioBrick(Type type)
  {
    _type = type;
  }

  public abstract BioBrick copy();
    
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

        //check type, name, length
        result = (this._type == b._type) && (this._name == b._name) && (this._size == b._size);

        return result;
    }
}

public class PromoterBrick : BioBrick
{
  private float _beta;
  private string _formula;

  public void setBeta(float v) { _beta = v; }
  public float getBeta() { return _beta; }
  public void setFormula(string v) { _formula = v; }
  public string getFormula() { return _formula; }

  public PromoterBrick() : base(BioBrick.Type.PROMOTER)
  {
  }
	
  public PromoterBrick(string name, float beta, string formula) : base(BioBrick.Type.PROMOTER)
  {
        _name = name;
		_beta = beta;
		_formula = formula;
  }

  public PromoterBrick(PromoterBrick p) : this(p._name, p._beta, p._formula)
  {
  }
	
    public override BioBrick copy()
    {
        return new PromoterBrick(this);
    }

    public override bool Equals(System.Object obj)
    {
        PromoterBrick pb = obj as PromoterBrick;
        return base.Equals(obj) && (_beta == pb._beta) && (_formula == pb._formula);
    }

  public override string ToString ()
  {
	return string.Format ("[PromoterBrick: name: {0}, beta: {1}, formula: {2}]", _name, _beta, _formula);
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

  public RBSBrick(string name, float RBSFactor) : base(BioBrick.Type.RBS)
  {
    _name = name;
		_RBSFactor = RBSFactor;
  }
	
  public RBSBrick(RBSBrick r) : this(r._name, r._RBSFactor)
  {
  }

    public override BioBrick copy()
    {
        return new RBSBrick(this);
    }
    
    public override bool Equals(System.Object obj)
    {
        RBSBrick rbsb = obj as RBSBrick;
        return base.Equals(obj) && (_RBSFactor == rbsb._RBSFactor);
    }

  public override string ToString ()
  {
	return string.Format ("[RBSBrick: name: {0}, RBSFactor: {1}]", _name, _RBSFactor);
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

  public GeneBrick(string name, string proteinName) : base(BioBrick.Type.GENE)
  {
    _name = name;
		_proteinName = proteinName;
  }

  public GeneBrick(GeneBrick g) : this(g._name, g._proteinName)
  {
  }

    public override BioBrick copy()
    {
        return new GeneBrick(this);
    }
    
    public override bool Equals(System.Object obj)
    {
        GeneBrick gb = obj as GeneBrick;
        return base.Equals(obj) && (_proteinName == gb._proteinName);
    }
	
  public override string ToString ()
  {
	return string.Format ("[GeneBrick: name: {0}, proteinName: {1}]", _name, _proteinName);
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

  public TerminatorBrick(string name, float terminatorFactor) : base(BioBrick.Type.TERMINATOR)
  {
    _name = name;
		_terminatorFactor = terminatorFactor;
  }

  public TerminatorBrick(TerminatorBrick t) : this(t._name, t._terminatorFactor)
  {
  }

    public override BioBrick copy()
    {
        return new TerminatorBrick(this);
    }
    
    public override bool Equals(System.Object obj)
    {
        TerminatorBrick tb = obj as TerminatorBrick;
        return base.Equals(obj) && (_terminatorFactor == tb._terminatorFactor);
    }
	
  public override string ToString ()
  {
	  return string.Format ("[TerminatorBrick: name: {0}, terminatorFactor: {1}]", _name, _terminatorFactor);
  }
}
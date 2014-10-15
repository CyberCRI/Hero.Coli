using System;
using System.Collections;
using System.Collections.Generic;


public class BioBrick: DNABit
{
  public enum Type
  {
    PROMOTER,
    RBS,
    GENE,
    TERMINATOR,
    UNKNOWN
  }

  protected string _name;
  protected int _size;
  protected Type _type;

  public void setName(string name) { _name = name; }
  public string getName() { return _name; }
  public void setSize(int size) { _size = size; }
  public int getSize() { return _size; }
  public Type getType() { return _type; }

  public BioBrick(Type type)
  {
    _type = type;
  }

  public BioBrick(BioBrick b)
  {
        Logger.Log("BioBrick::BioBrick("+b+") starting", Logger.Level.WARN);
    switch(b._type)
    {
        case Type.PROMOTER:
                Logger.Log("BioBrick::BioBrick("+b+") PROMOTER brick type="+b._type, Logger.Level.WARN);
            new PromoterBrick((PromoterBrick)b);
            break;
        case Type.RBS:
                Logger.Log("BioBrick::BioBrick("+b+") RBS brick type="+b._type, Logger.Level.WARN);
            new RBSBrick((RBSBrick)b);
            break;
        case Type.GENE:
                Logger.Log("BioBrick::BioBrick("+b+") GENE brick type="+b._type, Logger.Level.WARN);
            new GeneBrick((GeneBrick)b);
            break;
        case Type.TERMINATOR:
                Logger.Log("BioBrick::BioBrick("+b+") TERMINATOR brick type="+b._type, Logger.Level.WARN);
            new TerminatorBrick((TerminatorBrick)b);
            break;
        case Type.UNKNOWN:
            Logger.Log("BioBrick::BioBrick("+b+") unmanaged brick type="+b._type, Logger.Level.WARN);
            break;
        default:
            Logger.Log("BioBrick::BioBrick("+b+") unidentified brick type="+b._type, Logger.Level.WARN);
            break;
    }
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
	
  public override string ToString ()
  {
	return string.Format ("[PromoterBrick: name: {0}, beta: {1}, formula: {2}]", _name, _beta, _formula);
  }
}

class RBSBrick : BioBrick
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

  public override string ToString ()
  {
	return string.Format ("[RBSBrick: name: {0}, RBSFactor: {1}]", _name, _RBSFactor);
  }
}

class GeneBrick : BioBrick
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
	
  public override string ToString ()
  {
	return string.Format ("[GeneBrick: name: {0}, proteinName: {1}]", _name, _proteinName);
  }
}

class TerminatorBrick : BioBrick
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
	
  public override string ToString ()
  {
	  return string.Format ("[TerminatorBrick: name: {0}, terminatorFactor: {1}]", _name, _terminatorFactor);
  }
}
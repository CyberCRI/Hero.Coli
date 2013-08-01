using System;
using System.Collections;
using System.Collections.Generic;


public abstract class BioBrick
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
	
  public PromoterBrick(float beta, string formula) : base(BioBrick.Type.PROMOTER)
  {
		_beta = beta;
		_formula = formula;
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

  public RBSBrick(float RBSFactor) : base(BioBrick.Type.RBS)
  {
		_RBSFactor = RBSFactor;
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

  public GeneBrick(string proteinName) : base(BioBrick.Type.GENE)
  {
		_proteinName = proteinName;
  }
	
  public override string ToString ()
  {
	return string.Format ("[GeneBrick: name: {0}, proteinName: {1}]", _name, _proteinName);
  }
}

class TerminatorBrick : BioBrick
{
  private float _terminatorFactor;

  public void setTerminatorFactor(float v) { _terminatorFactor = v; }
  public float getTerminatorFactor() { return _terminatorFactor; }

  public TerminatorBrick() : base(BioBrick.Type.TERMINATOR)
  {
  }

  public TerminatorBrick(float terminatorFactor) : base(BioBrick.Type.TERMINATOR)
  {
		_terminatorFactor = terminatorFactor;
  }
	
  public override string ToString ()
  {
	return string.Format ("[TerminatorBrick: name: {0}, terminatorFactor: {1}]", _name, _terminatorFactor);
  }
}
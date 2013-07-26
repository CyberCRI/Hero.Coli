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
  public string getForumla() { return _formula; }

  public PromoterBrick() : base(BioBrick.Type.PROMOTER)
  {
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
}

class GeneBrick : BioBrick
{
  private string _proteinName;
           
  public void setProteinName(string name) { _proteinName = name; }
  public string getProteinName() { return _proteinName; }

  public GeneBrick() : base(BioBrick.Type.GENE)
  {
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
}
using System;
using System.Collections.Generic;

public class Device
{
  private string                        _name;
  private List<ExpressionModule>       _modules;
  
  public string getName() { return _name; }
  public void setName(string v) { _name = v; }
  public List<ExpressionModule> getBioBricks() { return _modules; }

  public int getSize()
  {
    int sum = 0;

    foreach (ExpressionModule em in _modules)
      sum += em.getSize();
    return sum;
  }

  public Device(string name, List<ExpressionModule> modules)
  {
    _name = name;
    _modules = new List<ExpressionModule>();
    foreach (ExpressionModule em in modules)
      _modules.Add(em);
  }
}
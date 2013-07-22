using System;
using System.Collections.Generic;

class Device
{
  private string                        _name;
  private List<ExpressionModules>       _modules;
  
  public string getName() { return _name; }
  public void setName(string v) { _name = v; }
  public List<ExpressionModules> getBioBricks() { return _modules; }

  public int getSize()
  {
    int sum = 0;

    foreach (ExpressionModules em in _modules)
      sum += em.getSize();
    return sum;
  }

  public Device(string name, List<ExpressionModules> bricks)
  {
    _name = name;
    _modules = new List<ExpressionModules>();
    foreach (ExpressionModules em in _modules)
      _modules.Add(em);
  }
}
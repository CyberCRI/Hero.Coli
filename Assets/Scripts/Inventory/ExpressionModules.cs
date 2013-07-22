using System;
using System.Collections;
using System.Collections.Generic;

public class ExpressionModules
{
  private string                _name;
  private List<BioBrick>        _bioBricks;

  public string getName() { return _name; }
  public void setName(string v) { _name = v; }
  public List<BioBrick> getBioBricks() { return _bioBricks; }

  public int getSize()
  {
    int sum = 0;

    foreach (BioBrick b in _bioBricks)
      sum += b.getSize();
    return sum;
  }

  public ExpressionModules(string name, List<BioBrick> bricks)
  {
    _name = name;
    _bioBricks = new List<BioBrick>();
    foreach (BioBrick b in bricks)
      _bioBricks.Add(b);
  }
}
using System;
using System.Collections;
using System.Collections.Generic;

public class ExpressionModule
{
  private string                _name;
  private LinkedList<BioBrick>        _bioBricks;

  public string getName() { return _name; }
  public void setName(string v) { _name = v; }
  public LinkedList<BioBrick> getBioBricks() { return _bioBricks; }

  public int getSize()
  {
    int sum = 0;

    foreach (BioBrick b in _bioBricks)
      sum += b.getSize();
    return sum;
  }

  public ExpressionModule(string name, LinkedList<BioBrick> bricks)
  {
    _name = name;
    _bioBricks = new LinkedList<BioBrick>();
    foreach (BioBrick b in bricks)
      _bioBricks.AddLast(b);
  }
}
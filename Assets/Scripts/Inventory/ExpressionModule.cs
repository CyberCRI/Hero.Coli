using System;
using System.Collections;
using System.Collections.Generic;

public class ExpressionModule
{
  private string                _name;
  private LinkedList<BioBrick>  _bioBricks;

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

  public ExpressionModule(ExpressionModule m)
  {
    _name = m._name;
    _bioBricks = new LinkedList<BioBrick>();
    foreach (BioBrick b in m.getBioBricks())
    {
      //_bioBricks.AddLast(b);
            Logger.Log("ExpressionModule::ExpressionModule(m) on b="+b, Logger.Level.WARN);
      _bioBricks.AddLast(new BioBrick(b));
    }
  }

  public bool hasSameBricks(ExpressionModule module) {
    if(module._bioBricks.Count != _bioBricks.Count) {
      Logger.Log("Device::hasSameBricks("+module+") of "+this+" differ on count: "+module._bioBricks.Count+"≠"+_bioBricks.Count);
      return false;
    }

    IEnumerator<BioBrick> enumerator1 = module._bioBricks.GetEnumerator();
    IEnumerator<BioBrick> enumerator2 = _bioBricks.GetEnumerator();

    while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
      if(!enumerator1.Current.Equals(enumerator2.Current)) {
        Logger.Log("ExpressionModule::hasSameBricks("+module+") of "+this+" differ on "+enumerator1.Current+"≠"+enumerator2.Current);
        return false;
      }
    }
    Logger.Log("ExpressionModule::hasSameBricks("+module+") of "+this+" coincide");
    return true;
  }

  public override string ToString ()
  {
		string bricksString = "[BioBricks: "+Logger.ToString(_bioBricks)+"]";
		return string.Format ("[ExpressionModule: name: {0}, bricks: {1}]", _name, bricksString);
  }
}
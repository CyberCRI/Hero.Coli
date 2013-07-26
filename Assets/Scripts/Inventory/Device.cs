using System;
using System.Collections.Generic;

class Device
{
  private string                        _name;
  private LinkedList<ExpressionModule>       _modules;
  
  public string getName() { return _name; }
  public void setName(string v) { _name = v; }
  public LinkedList<ExpressionModule> getBioBricks() { return _modules; }

  public int getSize()
  {
    int sum = 0;

    foreach (ExpressionModule em in _modules)
      sum += em.getSize();
    return sum;
  }

  private Device()
  {}

  private Device(string name, LinkedList<ExpressionModule> bricks)
  {
    _name = name;
    _modules = new LinkedList<ExpressionModule>();
    foreach (ExpressionModule em in _modules)
      _modules.AddLast(new ExpressionModule(em));
  }

  private static bool checkPromoter(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.PROMOTER);
  }

  private static bool checkRBS(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.RBS);
  }

  private static bool checkGene(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.GENE);
  }

  private static bool checkTerminator(BioBrick b)
  {
    return (b.getType() == BioBrick.Type.TERMINATOR);
  }  

  private static bool checkRBSGene(LinkedList<BioBrick> list)
  {
    if (list == null)
      return false;
    if (list.Count == 0 || list.First.Value == null)
      return false;
    if (checkRBS(list.First.Value) == false)
      return false;
    list.RemoveFirst();
    if (list.Count == 0 || list.First.Value == null)
      return false;
    if (checkGene(list.First.Value) == false)
      return false;
    list.RemoveFirst();
    return true;
  }

  private static bool checkOperon(LinkedList<BioBrick> bricks)
  {
    bool b = false;

    while (checkRBSGene(bricks))
      b = true;
    return b;
  }

  private static bool checkModuleValidity(ExpressionModule module)
  {
    if (module == null)
      return false;

    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(module.getBioBricks());
    if (bricks == null)
      return false;
    if (bricks.Count == 0 || bricks.First.Value == null)
      return false;
    if (checkPromoter(bricks.First.Value) == false)
      return false;
    bricks.RemoveFirst();

    if (checkOperon(bricks) == false)
      return false;

    if (bricks.Count == 0 || bricks.First.Value != null || checkTerminator(bricks.First.Value) == false)
      return false;
    bricks.RemoveFirst();
    if (bricks.Count != 0)
      return false;
    return true;
  }

  private static bool checkDeviceValidity(LinkedList<ExpressionModule> device)
  {
    foreach (ExpressionModule em in device)
      if (checkModuleValidity(em) == false)
        return false;
    return true;
  }

  public static Device builDevice(string name, LinkedList<ExpressionModule> bricks)
  {
    if (bricks == null || checkDeviceValidity(bricks) == false)
      return null;
    Device device = new Device(name, bricks);
    return device;
  }
}
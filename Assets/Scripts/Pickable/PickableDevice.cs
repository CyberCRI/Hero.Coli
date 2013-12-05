using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableDevice : PickableItem {
  public DevicesDisplayer _devicesDisplayer;
  public AvailableBioBricksManager _availableBioBricksManager;
  protected new Device _dnaBit;

  void Awake()
  {
    Logger.Log("PickableDevice::Start", Logger.Level.TEMP);
    PromoterBrick prom = new PromoterBrick("PromY2", 75f, "[0.01,2]Y");
    RBSBrick rbs = new RBSBrick("RBS3", 3.0f);
    GeneBrick gene = new GeneBrick("X", "X");
    TerminatorBrick term = new TerminatorBrick("T1", 1.0f);
    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(new List<BioBrick>(){prom, rbs, gene, term});
    ExpressionModule module = new ExpressionModule("expr", bricks);
    _dnaBit = Device.buildDevice("DEV", new LinkedList<ExpressionModule>(new List<ExpressionModule>(){module}));
  }

  protected override void addTo()
  {
    Logger.Log("PickableDevice::addTo "+_dnaBit, Logger.Level.TEMP);
    foreach(BioBrick brick in _dnaBit.getExpressionModules().First.Value.getBioBricks())
    {
      Logger.Log("PickableDevice::addTo brick "+brick, Logger.Level.TEMP);
      _availableBioBricksManager.addAvailableBioBrick(brick, false);
      //AvailableBioBricksManager.get().addAvailableBioBrick((BioBrick) _dnaBit, false);
    }

    Logger.Log("PickableDevice::addTo device "+_dnaBit, Logger.Level.TEMP);
    _devicesDisplayer.addInventoriedDevice(_dnaBit);
    //DevicesDisplayer.get().addInventoriedDevice(_dnaBit);
  }
}

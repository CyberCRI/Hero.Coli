using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PickableDevice : PickableItem {
  protected new Device _dnaBit;

  void Awake()
  {
    _dnaBit = produceDevice();
  }

  protected abstract Device produceDevice();

  protected override void addTo()
  {
    Logger.Log("PickableDevice::addTo "+_dnaBit, Logger.Level.INFO);
    foreach(BioBrick brick in _dnaBit.getExpressionModules().First.Value.getBioBricks())
    {
      Logger.Log("PickableDevice::addTo brick "+brick, Logger.Level.INFO);
      AvailableBioBricksManager.get().addAvailableBioBrick(brick, false);
    }

    Logger.Log("PickableDevice::addTo device "+_dnaBit, Logger.Level.DEBUG);
    Inventory.get().askAddDevice(_dnaBit);
  }
}

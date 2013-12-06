using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PickableDevice : PickableItem {
  public Inventory _inventory;
  protected new Device _dnaBit;

  void Awake()
  {
    _dnaBit = produceDevice();
  }

  protected abstract Device produceDevice();

  protected override void addTo()
  {
    Logger.Log("PickableDevice::addTo "+_dnaBit, Logger.Level.DEBUG);
    foreach(BioBrick brick in _dnaBit.getExpressionModules().First.Value.getBioBricks())
    {
      Logger.Log("PickableDevice::addTo brick "+brick, Logger.Level.TRACE);
      AvailableBioBricksManager.get().addAvailableBioBrick(brick, false);
    }

    Logger.Log("PickableDevice::addTo device "+_dnaBit, Logger.Level.DEBUG);
    _inventory.askAddDevice(_dnaBit);
    //Inventory.get().askAddDevice(_dnaBit);
  }
}

using UnityEngine;
using System.Collections;

public class PickableBioBrick : PickableItem {
  public AvailableBioBricksManager _availableBioBricksManager;

  void Awake()
  {
    Logger.Log("PickableBioBrick::Start", Logger.Level.TEMP);
    _dnaBit = new TerminatorBrick("TEST", 12.12f);
  }

  protected override void addTo()
  {
    _availableBioBricksManager.addAvailableBioBrick((BioBrick) _dnaBit, false);
    //AvailableBioBricksManager.get().addAvailableBioBrick((BioBrick) _dnaBit, false);
  }
}

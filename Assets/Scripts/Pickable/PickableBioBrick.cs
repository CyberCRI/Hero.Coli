using UnityEngine;
using System.Collections;

public abstract class PickableBioBrick : PickableItem {
  public AvailableBioBricksManager _availableBioBricksManager;
  protected new BioBrick _dnaBit;

  protected abstract BioBrick produceBioBrick();

  void Awake()
  {
    Logger.Log("PickableBioBrick::Start", Logger.Level.TEMP);
    _dnaBit = produceBioBrick();//new TerminatorBrick("TEST", 12.12f);
  }

  protected override void addTo()
  {
    _availableBioBricksManager.addAvailableBioBrick(_dnaBit, false);
    //AvailableBioBricksManager.get().addAvailableBioBrick((BioBrick) _dnaBit, false);
  }
}

using UnityEngine;
using System.Collections;

public abstract class PickableBioBrick : PickableItem {
  public AvailableBioBricksManager _availableBioBricksManager;
  protected new BioBrick _dnaBit;

  protected abstract BioBrick produceBioBrick();

  void Awake()
  {
    _dnaBit = produceBioBrick();
  }

  protected override void addTo()
  {
    _availableBioBricksManager.addAvailableBioBrick(_dnaBit, false);
    //AvailableBioBricksManager.get().addAvailableBioBrick((BioBrick) _dnaBit, false);
  }
}

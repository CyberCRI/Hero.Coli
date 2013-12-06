using UnityEngine;
using System.Collections;

public abstract class PickableBioBrick : PickableItem {
  protected new BioBrick _dnaBit;

  protected abstract BioBrick produceBioBrick();

  void Awake()
  {
    _dnaBit = produceBioBrick();
  }

  protected override void addTo()
  {
    AvailableBioBricksManager.get().addAvailableBioBrick(_dnaBit, false);
  }
}

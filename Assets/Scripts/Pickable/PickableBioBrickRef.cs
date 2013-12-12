using UnityEngine;
using System.Collections;

public class PickableBioBrickRef : PickableBioBrick {
  public string bioBrickName;

  protected override BioBrick produceBioBrick()
  {
    return AvailableBioBricksManager.get().getBioBrickFromAll(bioBrickName);
  }
}




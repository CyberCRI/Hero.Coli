using UnityEngine;
using System.Collections;

public class PickableBioBrickRef : PickableBioBrick {
  public string bioBrickName;

  protected override DNABit produceDNABit()
  {
    return AvailableBioBricksManager.get().getBioBrickFromAll(bioBrickName);
  }
}




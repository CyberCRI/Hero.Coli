using UnityEngine;

public class PickableBioBrickRef : PickableBioBrick {
  [SerializeField]
  private string bioBrickName;
  [SerializeField]
  private int amount = 1;

  protected override DNABit produceDNABit()
  {
    BioBrick brick = AvailableBioBricksManager.get().getBioBrickFromAll(bioBrickName);
    return brick.copy(amount);
  }
}




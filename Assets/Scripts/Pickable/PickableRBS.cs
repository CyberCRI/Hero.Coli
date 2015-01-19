using UnityEngine;
using System.Collections;

public class PickableRBS : PickableBioBrick {
  public string bioBrickName;
  public float rbsFactor;

    protected override DNABit produceDNABit()
  {
    return new RBSBrick(bioBrickName, rbsFactor);
  }
}


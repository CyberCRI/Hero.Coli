using UnityEngine;
using System.Collections;

public class PickableTerminator : PickableBioBrick {
  public string bioBrickName;
  public float terminatorFactor;

    protected override DNABit produceDNABit()
  {
    return new TerminatorBrick(bioBrickName, terminatorFactor);
  }
}


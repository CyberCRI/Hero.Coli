using UnityEngine;

public class PickableTerminator : PickableBioBrick {
  [SerializeField]
  private string bioBrickName;
  [SerializeField]
  private float terminatorFactor;
  [SerializeField]
  private int size;

    protected override DNABit produceDNABit()
  {
    return new TerminatorBrick(bioBrickName, terminatorFactor, size);
  }
}


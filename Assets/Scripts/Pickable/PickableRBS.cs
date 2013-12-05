using UnityEngine;
using System.Collections;

public class PickableRBS : PickableBioBrick {
  public string bioBrickName;
  public float rbsFactor;

  protected override BioBrick produceBioBrick()
  {
    return new RBSBrick(bioBrickName, rbsFactor);
  }
}


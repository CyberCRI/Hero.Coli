using UnityEngine;
using System.Collections;

public class PickablePromoter : PickableBioBrick {
  public string bioBrickName;
  public float beta;
  public string formula;

    protected override DNABit produceDNABit()
  {
    return new PromoterBrick(bioBrickName, beta, formula);
  }
}

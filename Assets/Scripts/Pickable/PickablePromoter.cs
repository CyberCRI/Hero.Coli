using UnityEngine;
using System.Collections;

public class PickablePromoter : PickableBioBrick {
  public string bioBrickName;
  public float beta;
  public string formula;

  protected override BioBrick produceBioBrick()
  {
    return new PromoterBrick(bioBrickName, beta, formula);
  }
}

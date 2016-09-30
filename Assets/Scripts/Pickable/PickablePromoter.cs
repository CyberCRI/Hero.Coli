using UnityEngine;

public class PickablePromoter : PickableBioBrick
{
    [SerializeField]
    private string bioBrickName;
    [SerializeField]
    private float beta;
    [SerializeField]
    private string formula;
    [SerializeField]
    private int size;

    protected override DNABit produceDNABit()
    {
        return new PromoterBrick(bioBrickName, beta, formula, size);
    }
}

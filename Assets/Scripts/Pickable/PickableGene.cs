using UnityEngine;

public class PickableGene : PickableBioBrick
{
    [SerializeField]
    private string bioBrickName;
    [SerializeField]
    private string proteinName;
    [SerializeField]
    private int geneSize;

    protected override DNABit produceDNABit()
    {
        return new GeneBrick(bioBrickName, proteinName, geneSize);
    }
}

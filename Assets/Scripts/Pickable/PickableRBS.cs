using UnityEngine;

public class PickableRBS : PickableBioBrick
{
    [SerializeField]
    private string bioBrickName;
    [SerializeField]
    private float rbsFactor;
    [SerializeField]
    private int size;

    protected override DNABit produceDNABit()
    {
        return new RBSBrick(bioBrickName, rbsFactor, size);
    }
}


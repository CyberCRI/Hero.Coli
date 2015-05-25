using System;
using UnityEngine;

public class DNABit {
    public override string ToString ()
    {
        return "DNABit";
    }

    public virtual string getInternalName ()
    {
        return ToString();
    }
};


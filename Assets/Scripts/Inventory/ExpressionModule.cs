using System.Collections.Generic;
using UnityEngine;

public class ExpressionModule
{
    private string                _name;
    private LinkedList<BioBrick>  _bioBricks;
    private const string _invalidEMName = "invalidEMName";

    //TODO factorize code with Device's
    public string displayedName { get; set; }
    private string _internalName;
    public string getInternalName() {
        Debug.Log(this.GetType() + " getInternalName()");
        if(string.IsNullOrEmpty(_internalName))
        {
            _internalName = generateInternalName();
        }
        return _internalName;
    }

    public LinkedList<BioBrick> getBioBricks() { return _bioBricks; }

    public int getSize()
    {
        int sum = 0;

        foreach (BioBrick b in _bioBricks)
            sum += b.getSize();
        return sum;
    }

    //generates internal name from biobricks sequence
    private string generateInternalName()
    {
        Debug.Log(this.GetType() + " generateName(bricks)");
        return generateInternalName(_bioBricks);
    }

    private static string generateInternalName(LinkedList<BioBrick> bricks)
    {
        Debug.Log("ExpressionModule generateInternalName(bricks)");

        if(isBioBricksSequenceValid(bricks))
        {
            string name = "";
            string separator = ":";

            LinkedList<BioBrick> bb = new LinkedList<BioBrick>(bricks);
            while(1 != bb.Count)
            {
                name+=bb.First.Value.getName()+separator;
                bb.RemoveFirst();
            }
            name+=bb.First.Value.getName();
            return name;
        }
        else
        {
            Debug.LogWarning("Device generateInternalName(bricks) was provided incorrect BioBrick sequence");
            return _invalidEMName;
        }
    }

    //TODO generate name from BioBricks sequence
    public ExpressionModule(LinkedList<BioBrick> bricks)
    {
        Debug.Log(this.GetType() + " ExpressionModule(bricks)");
        if(isBioBricksSequenceValid(bricks))
        {
            new ExpressionModule("test", bricks);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " ExpressionModule(bricks) failed");
        }
    }

    public ExpressionModule(string name, LinkedList<BioBrick> bricks)
    {
        if(isBioBricksSequenceValid(bricks))
        {
            _name = name;
            _bioBricks = new LinkedList<BioBrick>();
            foreach (BioBrick b in bricks)
                _bioBricks.AddLast(b);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " ExpressionModule("+name+", bricks) failed");
        }
    }

    //copy constructor
    public ExpressionModule(ExpressionModule m)
    {
        Debug.Log(this.GetType() + " ExpressionModule("+m+")");
        _name = m._name;
        _bioBricks = new LinkedList<BioBrick>();
        foreach (BioBrick b in m.getBioBricks())
    {
      _bioBricks.AddLast(b.copy());
    }
  }

    public bool hasSameBricks(ExpressionModule module) {

        if(module._bioBricks.Count != _bioBricks.Count) {
            return false;
        }

        IEnumerator<BioBrick> enumerator1 = module._bioBricks.GetEnumerator();
        IEnumerator<BioBrick> enumerator2 = _bioBricks.GetEnumerator();

        while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
            if(!enumerator1.Current.Equals(enumerator2.Current)) {
                return false;
            }
        }
        return true;
    }

    private static string generateInternalNameFromBricks(LinkedList<BioBrick> bricks)
    {
        Debug.Log("ExpressionModule generateInternalNameFromBricks("+bricks+")");
        return "test";
    }

    private static bool checkPromoter(BioBrick b)
    {
        Debug.Log("ExpressionModule checkPromoter("+b+")");
        return (b.getType() == BioBrick.Type.PROMOTER);
    }
    
    private static bool checkRBS(BioBrick b)
    {
        Debug.Log("ExpressionModule checkRBS("+b+")");
        return (b.getType() == BioBrick.Type.RBS);
    }
    
    private static bool checkGene(BioBrick b)
    {
        Debug.Log("ExpressionModule checkGene("+b+")");
        return (b.getType() == BioBrick.Type.GENE);
    }
    
    private static bool checkTerminator(BioBrick b)
    {
        Debug.Log("ExpressionModule checkTerminator("+b+")");
        return (b.getType() == BioBrick.Type.TERMINATOR);
    }  
    
    private static bool checkRBSGene(LinkedList<BioBrick> bricks)
    {
        Debug.Log("ExpressionModule checkRBSGene("+bricks+")");
        if (bricks == null)
            return false;
        if (bricks.Count == 0 || bricks.First.Value == null)
            return false;
        if (checkRBS(bricks.First.Value) == false)
            return false;
        bricks.RemoveFirst();
        if (bricks.Count == 0 || bricks.First.Value == null)
            return false;
        if (checkGene(bricks.First.Value) == false)
            return false;
        bricks.RemoveFirst();
        return true;
    }
    
    private static bool checkOperon(LinkedList<BioBrick> bricks)
    {
        Debug.Log("ExpressionModule checkOperon("+bricks+")");
        bool b = false;
        
        while (checkRBSGene(bricks))
            b = true;
        return b;
    }
    
    //checks for basic BioBrick sequence (4 bricks)
    public static bool isBioBricksSequenceValid(LinkedList<BioBrick> bioBricksToCheck)
    {
        if (bioBricksToCheck == null) {
            Debug.Log("ExpressionModule isBioBricksSequenceValid failed (bioBricksToCheck == null)");
            return false;
        }
        LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(bioBricksToCheck);
        
        
        //PROMOTER
        if (bricks.Count == 0 || bricks.First.Value == null) {
            Debug.Log("ExpressionModule isBioBricksSequenceValid failed: no promoter");
            return false;
        }
        if (checkPromoter(bricks.First.Value) == false) {
            Debug.Log("ExpressionModule isBioBricksSequenceValid failed (checkPromoter(bricks.First.Value) == false)");
            return false;
        }
        bricks.RemoveFirst();
        
        //RBS & ORF
        if (bricks.Count == 0 || bricks.First.Value == null) {
            Debug.Log("ExpressionModule isBioBricksSequenceValid failed: no RBS/ORF/Terminator");
            return false;
        }
        if (checkOperon(bricks) == false) {
            Debug.Log("ExpressionModule isBioBricksSequenceValid failed (checkOperon(bricks) == false)");
            return false;
        }        
        bool b1 = (bricks.Count == 0);
        bool b2 = (b1 || (bricks.First.Value == null));
        bool b3 = (b2 || checkTerminator(bricks.First.Value) == false);
        if (b1 || b2 || b3) {
            if (b1) Debug.Log("ExpressionModule isBioBricksSequenceValid failed: no terminator");
            if (b2) Debug.Log("ExpressionModule isBioBricksSequenceValid failed: terminator is null");
            if (b3) Debug.Log("ExpressionModule isBioBricksSequenceValid failed: not a terminator");
            return false;
        }
        bricks.RemoveFirst();
        if (bricks.Count != 0) {
            Debug.LogWarning("Device isBioBricksSequenceValid failed: terminator wasn't last brick");
            return false;
        }
        return true;
    }
    
    public bool isValid()
    {
        Debug.Log(this.GetType() + " checkModuleValidity("+this.ToString()+")");
        return isBioBricksSequenceValid(this.getBioBricks());
    }

    public override string ToString ()
    {
        string bricksString = "[BioBricks: "+Logger.ToString(_bioBricks)+"]";
        return string.Format ("[ExpressionModule: name: {0}, bricks: {1}]", _name, bricksString);
    }
}
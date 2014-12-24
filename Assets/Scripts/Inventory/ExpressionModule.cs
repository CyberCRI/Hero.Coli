using System;
using System.Collections;
using System.Collections.Generic;

public class ExpressionModule
{
    private string                _name;
    private LinkedList<BioBrick>  _bioBricks;
    private static string _invalidEMName = "invalidEMName";

    //TODO factorize code with Device's
    public string displayedName { get; set; }
    private string _internalName;
    public string getInternalName() {
        Logger.Log("ExpressionModule::getInternalName()", Logger.Level.DEBUG);
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
        Logger.Log("ExpressionModule::generateName(bricks)", Logger.Level.DEBUG);
        return generateInternalName(_bioBricks);
    }

    private static string generateInternalName(LinkedList<BioBrick> bricks)
    {
        Logger.Log("ExpressionModule::generateInternalName(bricks)", Logger.Level.INFO);

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
            Logger.Log("ExpressionModule::generateInternalName(bricks) was provided incorrect BioBrick sequence", Logger.Level.WARN);
            return _invalidEMName;
        }
    }

    //TODO generate name from BioBricks sequence
    public ExpressionModule(LinkedList<BioBrick> bricks)
    {
        Logger.Log("ExpressionModule::ExpressionModule(bricks)", Logger.Level.INFO);
        if(isBioBricksSequenceValid(bricks))
        {
            new ExpressionModule("test", bricks);
        }
        else
        {
            Logger.Log("ExpressionModule::ExpressionModule(bricks) failed", Logger.Level.WARN);
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
            Logger.Log("ExpressionModule::ExpressionModule("+name+", bricks) failed", Logger.Level.WARN);
        }
    }

    //copy constructor
    public ExpressionModule(ExpressionModule m)
    {
        Logger.Log("ExpressionModule::ExpressionModule("+m+")", Logger.Level.DEBUG);
        _name = m._name;
        _bioBricks = new LinkedList<BioBrick>();
        foreach (BioBrick b in m.getBioBricks())
    {
      _bioBricks.AddLast(b.copy());
    }
  }

    public bool hasSameBricks(ExpressionModule module) {
        if(module._bioBricks.Count != _bioBricks.Count) {
            Logger.Log("ExpressionModule::hasSameBricks("+module+") of "+this+" differ on count: "+module._bioBricks.Count+"≠"+_bioBricks.Count);
            return false;
        }

        IEnumerator<BioBrick> enumerator1 = module._bioBricks.GetEnumerator();
        IEnumerator<BioBrick> enumerator2 = _bioBricks.GetEnumerator();

        while(enumerator1.MoveNext() && enumerator2.MoveNext()) {
            if(!enumerator1.Current.Equals(enumerator2.Current)) {
                Logger.Log("ExpressionModule::hasSameBricks("+module+") of "+this+" differ on "+enumerator1.Current+"≠"+enumerator2.Current);
                return false;
            }
        }
        Logger.Log("ExpressionModule::hasSameBricks("+module+") of "+this+" coincide");
        return true;
    }

    private static string generateInternalNameFromBricks(LinkedList<BioBrick> bricks)
    {
        Logger.Log("ExpressionModule::generateInternalNameFromBricks("+bricks+")", Logger.Level.INFO);
        return "test";
    }

    private static bool checkPromoter(BioBrick b)
    {
        Logger.Log("ExpressionModule::checkPromoter("+b+")", Logger.Level.DEBUG);
        return (b.getType() == BioBrick.Type.PROMOTER);
    }
    
    private static bool checkRBS(BioBrick b)
    {
        Logger.Log("ExpressionModule::checkRBS("+b+")", Logger.Level.DEBUG);
        return (b.getType() == BioBrick.Type.RBS);
    }
    
    private static bool checkGene(BioBrick b)
    {
        Logger.Log("ExpressionModule::checkGene("+b+")", Logger.Level.DEBUG);
        return (b.getType() == BioBrick.Type.GENE);
    }
    
    private static bool checkTerminator(BioBrick b)
    {
        Logger.Log("ExpressionModule::checkTerminator("+b+")", Logger.Level.DEBUG);
        return (b.getType() == BioBrick.Type.TERMINATOR);
    }  
    
    private static bool checkRBSGene(LinkedList<BioBrick> bricks)
    {
        Logger.Log("ExpressionModule::checkRBSGene("+bricks+")", Logger.Level.DEBUG);
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
        Logger.Log("ExpressionModule::checkOperon("+bricks+")", Logger.Level.DEBUG);
        bool b = false;
        
        while (checkRBSGene(bricks))
            b = true;
        return b;
    }
    
    //checks for basic BioBrick sequence (4 bricks)
    public static bool isBioBricksSequenceValid(LinkedList<BioBrick> bioBricksToCheck)
    {
        LinkedList<BioBrick> bricks = new LinkedList<BioBrick>(bioBricksToCheck);
        
        if (bricks == null) {
            Logger.Log("ExpressionModule::isBioBricksSequenceValid failed (bricks == null)", Logger.Level.DEBUG);
            return false;
        }
        if (bricks.Count == 0 || bricks.First.Value == null) {
            Logger.Log("ExpressionModule::isBioBricksSequenceValid failed (bricks.Count == 0 || bricks.First.Value == null)", Logger.Level.DEBUG);
            return false;
        }
        if (checkPromoter(bricks.First.Value) == false) {
            Logger.Log("ExpressionModule::isBioBricksSequenceValid failed (checkPromoter(bricks.First.Value) == false)", Logger.Level.DEBUG);
            return false;
        }
        bricks.RemoveFirst();
        
        if (checkOperon(bricks) == false) {
            Logger.Log("ExpressionModule::isBioBricksSequenceValid failed (checkOperon(bricks) == false)", Logger.Level.DEBUG);
            return false;
        }
        
        bool b1 = (bricks.Count == 0);
        bool b2 = (bricks.First.Value == null);
        bool b3 = (checkTerminator(bricks.First.Value) == false);
        if (b1 || b2 || b3) {
            if (b1) Logger.Log("ExpressionModule::isBioBricksSequenceValid failed: bricks.Count == 0", Logger.Level.DEBUG);
            if (b2) Logger.Log("ExpressionModule::isBioBricksSequenceValid failed: bricks.First.Value == null", Logger.Level.DEBUG);
            if (b3) Logger.Log("ExpressionModule::isBioBricksSequenceValid failed: !checkTerminator(bricks.First.Value", Logger.Level.DEBUG);
            return false;
        }
        bricks.RemoveFirst();
        if (bricks.Count != 0) {
            Logger.Log("ExpressionModule::isBioBricksSequenceValid failed (bricks.Count != 0)", Logger.Level.WARN);
            return false;
        }
        return true;
    }
    
    public bool isValid()
    {
        Logger.Log("ExpressionModule::checkModuleValidity("+this.ToString()+")", Logger.Level.TRACE);
        return isBioBricksSequenceValid(this.getBioBricks());
    }

    public override string ToString ()
    {
        string bricksString = "[BioBricks: "+Logger.ToString(_bioBricks)+"]";
        return string.Format ("[ExpressionModule: name: {0}, bricks: {1}]", _name, bricksString);
    }
}
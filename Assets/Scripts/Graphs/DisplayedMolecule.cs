using UnityEngine;
using System.Collections;

public class DisplayedMolecule
{
    private string _name;
    private string _val;
    private bool _updated;
    
    public string getName()
    {
        return _name;
    }
    
    public string getVal()
    {
        return _val;
    }
    
    public bool isUpdated()
    {
        return _updated;
    }
    
    public DisplayedMolecule(string name, string val)
    {
        _updated = true;
        _name = name;
        _val = val;
    }
    
    public DisplayedMolecule(string name, float val) : this(name, val.ToString())
    {
    }
    
    public void update(string val)
    {
        _val = val;
        _updated = true;
    }
    
    public void update(float val)
    {
        update(val.ToString());
    }
    
    public void reset()
    {
        _updated = false;
    }
}

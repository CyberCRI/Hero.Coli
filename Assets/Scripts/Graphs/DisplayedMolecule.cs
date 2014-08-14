using UnityEngine;
using System.Collections;

public class DisplayedMolecule
{
    private string _name;
    private string _val;
    private bool _updated;
    private DisplayType _displayType;

    public enum DisplayType {
      MOLECULELIST,
      DEVICEMOLECULELIST
    }
    
    public string getName()
    {
        return _name;
    }
    
    public string getVal()
    {
        return _val;
    }

    public DisplayType getDisplayType()
    {
        return _displayType;
    }
    
    public void setDisplayType(DisplayType displayType)
    {
        _displayType = displayType;
    }
    
    public bool isUpdated()
    {
        return _updated;
    }
    
    public DisplayedMolecule(string name, string val, DisplayType displayType = DisplayType.MOLECULELIST)
    {
        _updated = true;
        _name = name;
        _val = val;
        _displayType = displayType;
    }
    
    public DisplayedMolecule(string name, float val, DisplayType displayType = DisplayType.MOLECULELIST) : this(name, val.ToString(), displayType)
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

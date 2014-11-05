using UnityEngine;
using System.Collections;

public class DisplayedMolecule
{
    private string _codeName;
    private string _realName;
    private string _val;
    private bool _updated;
    private DisplayType _displayType;

    public enum DisplayType {
      MOLECULELIST,
      DEVICEMOLECULELIST
    }
    
    public string getCodeName()
    {
      return _codeName;
    }
    
    public string getRealName()
    {
      return _realName;
    }

    public void setRealName(string realName)
    {
      _realName = realName;
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
    
    public DisplayedMolecule(string codeName, string realName, string val, DisplayType displayType = DisplayType.MOLECULELIST)
    {
      _updated = true;
      _codeName = codeName;
      _realName = realName;
      _val = val;
      _displayType = displayType;
    }
    
    public DisplayedMolecule(string codeName, string realName, float val, DisplayType displayType = DisplayType.MOLECULELIST) : this(codeName, realName, val.ToString(), displayType)
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

    public void OnLanguageChanged()
    {
      _realName = GameplayNames.getMoleculeRealName(_codeName);
    }
}

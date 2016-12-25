using UnityEngine;

public class DisplayedMolecule
{
    private string _codeName;
    private string _realName;
    private string _val;
    private float _fval;
    private bool _updated;
    private GraphMoleculeList _gml;
    private DisplayType _displayType;

    public enum DisplayType
    {
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
        Debug.Log(this.GetType() + " setRealName(" + realName + ")");
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
        _gml.setDisplayTypeChanged(true);
    }

    public GraphMoleculeList getGraphMoleculeList()
    {
        return _gml;
    }

    public void setGraphMoleculeList(GraphMoleculeList gml)
    {
        _gml = gml;
    }

    public bool isUpdated()
    {
        return _updated;
    }

    public DisplayedMolecule(string codeName, string realName, string val, GraphMoleculeList gml, DisplayType displayType = DisplayType.MOLECULELIST)
    {
        Debug.Log(this.GetType() + " constructor called with codeName=" + codeName + " and realName=" + realName);
        _updated = true;
        _codeName = codeName;
        _realName = realName;
        _val = val;
        _gml = gml;
        _displayType = displayType;
    }

    public DisplayedMolecule(string codeName, string realName, float val, GraphMoleculeList gml, DisplayType displayType = DisplayType.MOLECULELIST) : this(codeName, realName, val.ToString(), gml, displayType)
    {
    }

    public void update(string val)
    {
        _val = val;
        _updated = true;
    }

    public void update(float val)
    {
        if (_fval != val)
        {
            _fval = val;
            update(val.ToString("F4"));
        }
        else
        {
            _updated = true;
        }
    }

    public void reset()
    {
        _updated = false;
    }

    public void onLanguageChanged()
    {
        Debug.Log(this.GetType() + " OnLanguageChanged");
        _realName = GameplayNames.getMoleculeRealName(_codeName);
        _gml.setLanguageChanged(true);
    }
}

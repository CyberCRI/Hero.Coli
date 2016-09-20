/*!
  \brief This class describes a species of a chemical reaction
 */
public abstract class ReactionSpecies
{
    protected string _name;                 //! The name of the chemical species
    protected float _v;                     //! The stoichiometric coefficient ν (Greek nu), or quantity factor

    public void setName(string name) { _name = Tools.epurStr(name); }
    public string getName() { return _name; }

    public float v
    {
        get
        {
            return _v;
        }
        set
        {
            _v = value;
        }
    }

    public override bool Equals(object obj)
    {
        ReactionSpecies species = obj as ReactionSpecies;
        return (species != null)
          && _name == species._name
          && _v == species._v;
    }

    public abstract override string ToString();
}
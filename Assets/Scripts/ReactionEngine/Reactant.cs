/*!
  \brief This class describes a reactant of a chemical reaction
 */
public class Reactant : ReactionSpecies
{    
    protected float _n;                     //! The order of the chemical species in the rate equation

    public Reactant() { }
    public Reactant(string name, float stoichiometricCoefficient, float order)
    {
        _name = name;
        _v = stoichiometricCoefficient;
        _n = order;
    }
    public Reactant(Reactant p)
    {
        _name = p._name;
        _v = p._v;
        _n = p._n;
    }

    public float n
    {
        get
        {
            return _n;
        }
        set
        {
            _n = value;
        }
    }

    public override bool Equals(object obj)
    {
        Reactant reactant = obj as Reactant;
        return (reactant != null)
          && _name == reactant._name
          && _v == reactant._v
          && _n == reactant._n;
    }

    public override string ToString()
    {
        return "Reactant[name:" + _name + ", v:" + _v + ", n:" + _n + "]";
    }
}

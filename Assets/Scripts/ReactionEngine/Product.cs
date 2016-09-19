/*!
  \brief This class describes a product of a chemical reaction
 */
public class Product : ReactionSpecies
{
    public Product() { }
    public Product(string name, float stoichiometricCoefficient)
    {
        _name = name;
        _v = stoichiometricCoefficient;
    }
    public Product(ReactionSpecies p)
    {
        _name = p.getName();
        _v = p.v;
    }

    public override string ToString()
    {
        return "Product[name:" + _name + ", v:" + _v + "]";
    }
}
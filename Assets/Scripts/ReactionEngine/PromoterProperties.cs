
/*!
  \brief This class represents a PromoterReaction and can be loaded by the simulator.
  
  \sa PromoterReaction
 */
public class PromoterProperties
{
  public string name;                           //!< The name of the reaction
  public float beta;                            //!< The maximal production rate of the promoter
  public float terminatorFactor;                //!< The Coefficient that represent the terminator
  public string formula;                        //!< The formula that drive the promoter behaviour
  public LinkedList<Product> products;          //!< The list of products
  public float energyCost;                      //!< The cost in energy
  
  
  /*!
 *  \brief     ToString method
 *  \details   ToString method, with all fields, including detailed internal products
 */
  public override string ToString() {
    return "PromoterProperties["+
      "name:"+name+
        ", beta:"+beta+
        ", terminatorFactor:"+terminatorFactor+
        ", formula:"+formula+
        ", products:"+Logger.ToString<Product>(products)+
        ", energyCost:"+energyCost+"]";
  }
}
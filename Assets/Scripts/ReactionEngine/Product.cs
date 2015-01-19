using UnityEngine;
using System;

/*!
  \brief This class describes the Product of a chemical reaction
 */
public class Product
{
  protected string      _name;                  //! The name of the molecule
  protected float       _quantityFactor;        //! The factor of production
  
  public Product() { }
  public Product(string name, float quantityFactor) {
    _name = name;
    _quantityFactor = quantityFactor;
  }
  public Product(Product p)
  {
    _name = p._name;
    _quantityFactor = p._quantityFactor;
  }
  
  public void setName(string name) { _name = Tools.epurStr(name); }
  public string getName() { return _name; }
  public void setQuantityFactor(float quantity) { _quantityFactor = quantity; }
  public float getQuantityFactor() { return _quantityFactor; }
  
  public override bool Equals (object obj)
  {
    Product product = obj as Product;
    return (product != null)
      && _name           == product._name
        && _quantityFactor == product._quantityFactor;
  }
  
  public override string ToString() {
    return "Product[name:"+_name+", quantityFactor:"+_quantityFactor+"]";
  }
}
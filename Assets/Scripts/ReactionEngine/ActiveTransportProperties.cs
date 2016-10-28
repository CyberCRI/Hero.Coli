
using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

/*!
 \brief This class is a kind of descriptor of ActiveTransportReaction
 
  \important
  A file should respect this convention:

        <activeTransports>
          <ATProp>
            <name>name</name>                  -> The name
            <Medium>1<Medium>                  -> In which medium the reaction is done.
            <MediumSrc>1</MediumSrc>           -> Medium where the reaction takes place
            <MediumDst>2</MediumDst>           -> Medium where the product will be released
            <EnergyCost>0.1</EnergyCost>       -> Energy that a reaction costs
            <substrate>S</substrate>           -> The Molecule that will be consumed in the SrcMedium
            <enzyme>E</enzyme>                 -> The Molecule which will play the role of the tunnel
            <Kcat>1</Kcat>                     -> Reaction constant of enzymatic reaction
            <effector>False</effector>         -> The Molecule that will inhibit the reaction or activate the reaction (False value is accepted, meaning that there is no effector)
            <alpha>1</alpha>                   -> Alpha parameter. Describes the competitivity of the effector (see execEnzymeReaction in EnzymeReaction)
            <beta>0</beta>                     -> Beta parameter. Describes the extend of inhibition or the extend of activation (see execEnzymeReaction in EnzymeReaction)
            <Km>1.3</Km>                       -> Km parameter. Affinity between substrate and enzyme
            <Ki>0.0000001</Ki>                 -> Ki parameter. Affinity between effector and enzyme
            <Products>                         -> List of the products
              <name>O</name>                   -> Name of the product molecule
            </Products>
          </ATProp>
        </activeTransports>
  \sa EnzymeReaction  
  
 \sa ActiveTransport
 \sa ActiveTransportReaction
 */
public class ActiveTransportProperties : LoadableFromXmlImpl
{
  public string name;
  public int mediumId;                  //!< The Medium where the reaction will be executed
  public int srcMediumId;               //!< The source medium for the transport
  public int dstMediumId;               //!< The destination medium for the transport
  public string substrate;              //!< The substrate of the reaction
  public string enzyme;                 //!< The enzyme of the reaction
  public float Kcat;                    //!< The affinity between the substrate and the enzyme coefficient
  public string  effector;              //!< The effector of the reaction
  public float alpha;                   //!< Alpha descriptor of the effector
  public float beta;                    //!< Beta descriptor of the effector
  public float Km;                      //!< Affinity coefficient between substrate and enzyme
  public float Ki;                      //!< Affinity coefficient between effector and enzyme
  public LinkedList<Product> products;  //!< The list of the products
  public float energyCost;              //!< Cost in energy for one reaction
  
  /*!
  \brief Load all the attributes of an ActiveTransportProperties.
  \param node The XmlNode corresponding to the <ATProp> </ATProp> node.
  */
  public override bool tryInstantiateFromXml(XmlNode node)
  {
    Logger.Log("ActiveTransportProperties.tryInstantiateFromXml("+node+"), this="+this+") will load", Logger.Level.INFO);
    
    foreach (XmlNode attr in node)
    {
      switch (attr.Name)
      {
      case "name":
        name = checkActiveTransportString(attr.InnerText);
        break;
      case "substrate":
        substrate = checkActiveTransportString(attr.InnerText);
        break;
      case "enzyme":
        enzyme = checkActiveTransportString(attr.InnerText);
        break;
      case "Kcat":
        Kcat = checkActiveTransportFloat(attr.InnerText);
        break;
      case "effector":
        effector = checkActiveTransportString(attr.InnerText);
        break;
      case "alpha":
        alpha = checkActiveTransportFloat(attr.InnerText);
        break;
      case "beta":
        beta = checkActiveTransportFloat(attr.InnerText);
        break;
      case "Km":
        Km = checkActiveTransportFloat(attr.InnerText);
        break;
      case "Ki":
        Ki = checkActiveTransportFloat(attr.InnerText);
        break;
      case "Medium":
        mediumId = checkActiveTransportInt(attr.InnerText);
        break;
      case "EnergyCost":
        energyCost = checkActiveTransportFloat(attr.InnerText);
        break;
      case "MediumSrc":
        srcMediumId = checkActiveTransportInt(attr.InnerText);
        break;
      case "MediumDst":
        dstMediumId = checkActiveTransportInt(attr.InnerText);
        break;
      case "Products":
        loadActiveTransportReactionProducts(attr);
        break;
      default:
        Debug.LogWarning(this.GetType() + " tryInstantiateFromXml unrecognized attribute '"+attr.Name+"'");
        break;
      }
    }
    
    if(
      string.IsNullOrEmpty(name)
      || string.IsNullOrEmpty(substrate)
      || string.IsNullOrEmpty(enzyme)
      || string.IsNullOrEmpty(effector)
      || (0 == mediumId)
      || (0 == srcMediumId)
      || (0 == dstMediumId)
      || (0 == products.Count)
      //TODO conditions on Kcat, alpha, beta, Km, Ki, energyCost
      )
    {
      Logger.Log("ActiveTransportProperties.tryInstantiateFromXml failed to load", Logger.Level.INFO);
      return false;
    }
    else
    {
      Logger.Log("ActiveTransportProperties.tryInstantiateFromXml(node) loaded this="+this, Logger.Level.INFO);
      return true;
    }
  }
  
  /*!
  \brief Check the validity of a string
  \param value the string to check
  \return Return the string if everything is ok or a empty string else
  */
  private string checkActiveTransportString(string value)
  {
    if (String.IsNullOrEmpty(value))
    {
      Debug.Log("Error: Empty field");
      return "";
    }
    return value;
  }
  
  /*!
  \brief Check the validity of a string to get parsed by float.Parse and return the float that corresponding
  \param value the string to check
  \return Return the corresponding float if everything is ok or 0 else
  */
  private float checkActiveTransportFloat(string value)
  {
    if (String.IsNullOrEmpty(value))
    {
      Debug.Log("Error: Empty field");
      return 0f;
    }
    return(float.Parse(value.Replace(",", ".")));
  }
  
  /*!
  \brief Check the validity of a string to get parsed by int.Parse and return the int that corresponding
  \param value the string to check
  \return Return the corresponding int if everything is ok or 0 else
  */
  private int checkActiveTransportInt(string value)
  {
    if (String.IsNullOrEmpty(value))
    {
      Debug.Log("Error: Empty field");
      return 0;
    }
    return (int.Parse(value));
  }
  
  /*!
  \brief This function load all the products which are in <Products> fields and store them in an ActiveTransportProperties.
  \param node The XmlNode corresponding to <Products> </Products>.
  \param AT The ActiveTransportProperties where to store the products
  \return Return true always (not really usefull)
  */
  private bool loadActiveTransportReactionProducts(XmlNode node)
  {
    products = new LinkedList<Product>();
    foreach (XmlNode attr in node)
    {
      if (attr.Name == "name")
      {
        if (String.IsNullOrEmpty(attr.InnerText))
          Debug.Log("Warning : Empty name field in ActiveTransport Reaction definition");
        Product prod = new Product();
        prod.setName(node.InnerText);
        products.AddLast(prod);
      }
    }
    if(0 == products.Count)
    {
      Debug.LogError(this.GetType() + " loadActiveTransportReactionProducts no product loaded");
      return false;
    }
    else
    {
      return true;
    }
  }
  
  public override string ToString ()
  {
    return string.Format (
      "[ActiveTransportProperties"
      +" name:"+name
      +", mediumId:"+mediumId
      +", srcMediumId:"+srcMediumId
      +", dstMediumId:"+dstMediumId
      +", substrate:"+substrate
      +", enzyme:"+enzyme
      +", Kcat:"+Kcat
      +", effector:"+effector
      +", alpha:"+alpha
      +", beta:"+beta
      +", Km:"+Km
      +", Ki:"+Ki
      +", products:"+Logger.ToString<Product>(products)
      +", energyCost:"+energyCost
      +"]");
  }
}

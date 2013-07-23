using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

/*!
  \brief This class load promoters reactions from xml files
  \details

  A Promoter reaction should respect this synthax:

  <promoter>
    <name>ptet</name>                           -> The name of the reaction
    <productionMax>100</productionMax>          -> The maximal production speed of the promoter
    <terminatorFactor>1</terminatorFactor>      -> between 0 and 1, represent the Terminator
    <formula>![0.8,3]tetR</formula>             -> The formula that manage the behaviour of the promoter (see Promoter class for more infos)
    <EnergyCost>0.1</EnergyCost>                -> The cost in energy
    <operon>
      <gene>
        <name>RFP</name>                        -> The molecule name of a product
        <RBSFactor>0.12</RBSFactor>             -> The RBS factor that represent the affinity between Ribosome and RBS
      </gene>
      <gene>
        <name>LacI</name>
        <RBSFactor>0.12</RBSFactor>
      </gene>
    </operon>
  </promoter>

  \sa Promoter
  \author Pierre COLLET
 */
public class PromoterLoader
{
  private PromoterParser _parser;               //!< The Formula Parser

  //! Default constructor
  public PromoterLoader()
  {
    _parser = new PromoterParser();
  }

  /*!
    \brief Load promoter name by checking the validity of the given string
    \param value The given name
    \param prom The Promoter reaction
    \return Return true if succed and false if value parameter is invalid.
   */
  private bool loadPromoterName(string value, Promoter prom)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty name field");
        return false;
      }
    prom.setName(value);
    return true;
  }

  /*!
    \brief Load promoter maximal production speed by checking the validity of the given string
    \param value The given maximal production
    \param prom The Promoter reaction
    \return Return true if succed and false if value parameter is invalid.
   */
  private bool loadPromoterProductionMax(string value, Promoter prom)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty productionMax field");
        return false;
      }
    prom.setBeta(float.Parse(value.Replace(",", ".")));
    return true;
  }

  /*!
    \brief Load promoter terminator factor by checking the validity of the given string
    \param value The given terminator factor
    \param prom The Promoter reaction
    \return Return true if succed and false if value parameter is invalid.
   */
  private bool loadPromoterTerminatorFactor(string value, Promoter prom)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty TerminatorFactor field");
        return false;
      }
    prom.setTerminatorFactor(float.Parse(value.Replace(",", ".")));
    return true;
  }

  /*!
    \brief Load promoter energy cost by checking the validity of the given string
    \param value The given energy cost
    \param prom The Promoter reaction
    \return Return true if succed and false if value parameter is invalid.
   */
  private bool loadEnergyCost(string value, Promoter prom)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty EnergyCost field. default value = 0");
        prom.setEnergyCost(0f);
      }
    else
      prom.setEnergyCost(float.Parse(value.Replace(",", ".")));
    return true;
  }

  /*!
    \brief Load promoter gene by checking the validity of the given strings
    \param prom The Promoter reaction
    \param name The name of the molecule that the gene will produce
    \param RBSf The Ribosome Binding Site factor string
    \return Return true if succed and false if value parameter is invalid.
   */
  private bool loadGene(Promoter prom, string name, string RBSf)
  {
    Product gene = new Product();

    if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(RBSf))
      {
        Debug.Log("Error: Empty Gene name field");
        return false;
      }
    gene.setName(name);
    gene.setQuantityFactor(float.Parse(RBSf.Replace(",", ".")));
    prom.addProduct(gene);
    return true;
  }

  /*!
    \brief Load promoter operon
    \param node the xml node
    \param prom The Promoter reaction
    \return Return true if succed and false if value parameter is invalid.
   */
  private bool loadPromoterOperon(XmlNode node, Promoter prom)
  {
    string name = null;
    string RBSf = null;
    bool n = false;
    bool rbsf = false;
    bool b = true;

    foreach (XmlNode gene in node)
      {
        n = false;
        rbsf = false;
        foreach(XmlNode attr in gene)
          {
            switch (attr.Name)
              {
              case "name":
                name = attr.InnerText;
                n = true;
                break;
              case "RBSFactor":
                RBSf = attr.InnerText;
                rbsf = true;
                break;
              }
          }
        if (n && rbsf)
          b = b && loadGene(prom, name, RBSf);
        if (!n)
          Debug.Log("Error : Missing Gene name in operon");
        if (!rbsf)
          Debug.Log("Error : Missing RBSfactor in operon");
      }
    return b;
  }

  /*!
    \brief Load promoter formula by checking the validity of the given string
    \param formula The given formula
    \param p The Promoter reaction
    \return Return true if succed and false if value parameter is invalid.
  */
  private bool loadPromoterFormula(string formula, Promoter p)
  {
    TreeNode<PromoterNodeData> tree = _parser.Parse(formula);
    
    if (tree == null)
      {
        Debug.Log("Syntax Error in promoter Formula");
        return false;
      }
    p.setFormula(tree);
    return true;
  }

  /*!
    \brief Load all promoter reaction in the given node
    \param node The xml node
    \param reactions The list of reactions where the new promoter reactions will be added
    \return Return true if succed and false if value parameter is invalid.
  */
  public bool loadPromoters(XmlNode node, LinkedList<IReaction> reactions)
  {
    XmlNodeList promotersList = node.SelectNodes("promoter");
    bool b = true;

    foreach (XmlNode promoter in promotersList)
      {
        Promoter p = new Promoter();
        foreach (XmlNode attr in promoter)
          {
            switch (attr.Name)
              {
              case "name":
                b = b && loadPromoterName(attr.InnerText, p);
                break;
              case "productionMax":
                b = b && loadPromoterProductionMax(attr.InnerText, p);
                break;
              case "terminatorFactor":
                b = b && loadPromoterTerminatorFactor(attr.InnerText, p);
                break;
              case "EnergyCost":
                b = b && loadEnergyCost(attr.InnerText, p);
                break;
              case "formula":
                b = b && loadPromoterFormula(attr.InnerText, p);
                break;
              case "operon":
                b = b && loadPromoterOperon(attr, p);
                break;
              }
          }
        reactions.AddLast(p);
      }
    return b;
  }
}
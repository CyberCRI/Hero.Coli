using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

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
    \param prom The PromoterReaction reaction
    \return Return true if succeeded and false if value parameter is invalid.
   */
  private bool loadPromoterName(string value, PromoterReaction prom)
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
    \param prom The PromoterReaction
    \return Return true if succeeded and false if value parameter is invalid.
   */
  private bool loadPromoterProductionMax(string value, PromoterReaction prom)
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
    \param prom The PromoterReaction
    \return Return true if succeeded and false if value parameter is invalid.
   */
  private bool loadPromoterTerminatorFactor(string value, PromoterReaction prom)
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
    \param prom The PromoterReaction
    \return Return true if succeeded and false if value parameter is invalid.
   */
  private bool loadEnergyCost(string value, PromoterReaction prom)
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
    \param prom The PromoterReaction
    \param name The name of the molecule that the gene will produce
    \param RBSf The Ribosome Binding Site factor string
    \return Return true if succeeded and false if value parameter is invalid.
   */
  private bool loadGene(PromoterReaction prom, string name, string RBSf)
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
    \param prom The PromoterReaction
    \return Return true if succeeded and false if value parameter is invalid.
   */
  private bool loadPromoterOperon(XmlNode node, PromoterReaction prom)
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
    \param p The PromoterReaction
    \return Return true if succeeded and false if value parameter is invalid.
  */
  private bool loadPromoterFormula(string formula, PromoterReaction p)
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
}
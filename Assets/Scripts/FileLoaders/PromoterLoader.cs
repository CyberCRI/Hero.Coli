using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

public class PromoterLoader
{
  private PromoterParser _parser;

  public PromoterLoader()
  {
    _parser = new PromoterParser();
  }

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
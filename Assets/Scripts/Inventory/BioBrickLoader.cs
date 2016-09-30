using System.Xml;
using System;
using System.Collections.Generic;
using UnityEngine;

//TODO refactor with FileLoader
public class BioBrickLoader
{

    string bioBrickName;
    string bioBrickSize;
    string bioBrickType;

    string beta;
    string formula;
    string rbsfactor;
    string protein;
    string terminatorfactor;

    int size;

    BioBrick bioBrick;

    private void reinitVars()
    {
        bioBrickName = null;
        bioBrickSize = null;
        bioBrickType = null;

        beta = null;
        formula = null;
        rbsfactor = null;
        protein = null;
        terminatorfactor = null;

        bioBrick = null;

        size = 0;
    }

    public BioBrickLoader()
    {
        // Debug.Log(this.GetType() + " BioBrickLoader()");
    }

    public BioBrick loadBioBrick(XmlNode bioBrickNode)
    {

        reinitVars();
        //common biobrick attributes
        try
        {
            bioBrickName = bioBrickNode.Attributes[BioBricksXMLTags.ID].Value;
            bioBrickSize = bioBrickNode.Attributes[BioBricksXMLTags.SIZE].Value;
            bioBrickType = bioBrickNode.Attributes[BioBricksXMLTags.TYPE].Value;
        }
        catch (NullReferenceException exc)
        {
            Debug.LogWarning(this.GetType() + " loadBioBricksFromFile bad xml, missing field\n" + exc);
            return null;
        }
        catch (Exception exc)
        {
            Debug.LogWarning(this.GetType() + " loadBioBricksFromFile failed, got exc=" + exc);
            return null;
        }
        // Debug.Log(this.GetType() + " loadBioBricksFromFile got id=" + bioBrickName
        //   + ", size=" + bioBrickSize
        //   + ", type=" + bioBrickType);

        if (checkString(bioBrickName) && checkString(bioBrickSize))
        {
            size = parseInt(bioBrickSize);
            switch (bioBrickType)
            {
                case BioBricksXMLTags.PROMOTER:
                    logCurrentBioBrick(BioBricksXMLTags.PROMOTER);
                    foreach (XmlNode attr in bioBrickNode)
                    {
                        switch (attr.Name)
                        {
                            case BioBricksXMLTags.BETA:
                                beta = attr.InnerText;
                                break;
                            case BioBricksXMLTags.FORMULA:
                                formula = attr.InnerText;
                                break;
                            case XMLTags.COMMENT:
                                break;
                            default:
                                logUnknownAttr(attr, BioBricksXMLTags.PROMOTER);
                                break;
                        }
                    }
                    if (checkString(beta) && checkString(formula))
                    {
                        bioBrick = new PromoterBrick(bioBrickName, parseFloat(beta), formula, size);
                    }
                    break;
                case BioBricksXMLTags.RBS:
                    logCurrentBioBrick(BioBricksXMLTags.RBS);
                    foreach (XmlNode attr in bioBrickNode)
                    {
                        switch (attr.Name)
                        {
                            case BioBricksXMLTags.RBSFACTOR:
                                rbsfactor = attr.InnerText;
                                break;
                            case XMLTags.COMMENT:
                                break;
                            default:
                                logUnknownAttr(attr, BioBricksXMLTags.RBS);
                                break;
                        }
                    }
                    if (checkString(rbsfactor))
                    {
                        bioBrick = new RBSBrick(bioBrickName, parseFloat(rbsfactor), size);
                    }
                    break;
                case BioBricksXMLTags.GENE:
                    logCurrentBioBrick(BioBricksXMLTags.GENE);
                    foreach (XmlNode attr in bioBrickNode)
                    {
                        switch (attr.Name)
                        {
                            case BioBricksXMLTags.PROTEIN:
                                protein = attr.InnerText;
                                break;
                            case XMLTags.COMMENT:
                                break;
                            default:
                                logUnknownAttr(attr, BioBricksXMLTags.GENE);
                                break;
                        }
                    }
                    if (checkString(protein))
                    {
                        bioBrick = new GeneBrick(bioBrickName, protein, size);
                    }
                    break;
                case BioBricksXMLTags.TERMINATOR:
                    logCurrentBioBrick(BioBricksXMLTags.TERMINATOR);
                    foreach (XmlNode attr in bioBrickNode)
                    {
                        switch (attr.Name)
                        {
                            case BioBricksXMLTags.TERMINATORFACTOR:
                                terminatorfactor = attr.InnerText;
                                break;
                            case XMLTags.COMMENT:
                                break;
                            default:
                                logUnknownAttr(attr, BioBricksXMLTags.TERMINATOR);
                                break;
                        }
                    }
                    if (checkString(terminatorfactor))
                    {
                        bioBrick = new TerminatorBrick(bioBrickName, parseFloat(terminatorfactor), size);
                    }
                    break;
                default:
                    Debug.LogWarning(this.GetType() + " loadBioBricksFromFile wrong type " + bioBrickType);
                    break;
            }
            return bioBrick;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " loadBioBricksFromFile Error : missing attribute id in BioBrick node");
            return null;
        }
    }

    public LinkedList<BioBrick> loadBioBricksFromFile(string filePath)
    {
        // Debug.Log(this.GetType() + " loadBioBricksFromFile(" + filePath + ")");

        LinkedList<BioBrick> resultBioBricks = new LinkedList<BioBrick>();

        XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

        XmlNodeList bioBrickList = xmlDoc.GetElementsByTagName(BioBricksXMLTags.BIOBRICK);

        foreach (XmlNode bioBrickNode in bioBrickList)
        {
            loadBioBrick(bioBrickNode);
            if (bioBrick != null)
            {
                resultBioBricks.AddLast(bioBrick);
            }
        }
        return resultBioBricks;
    }

    private static float parseFloat(string real)
    {
        return float.Parse(real.Replace(",", "."));
    }

    private static int parseInt(string integer)
    {
        return int.Parse(integer);
    }

    private static void logUnknownAttr(XmlNode attr, string type)
    {
        Debug.LogWarning("BioBrickLoader loadBioBricksFromFile unknown attr " + attr.Name + " for " + type + " node");
    }

    private static void logCurrentBioBrick(string type)
    {
        // Debug.Log("BioBrickLoader loadBioBricksFromFile type=" + type);
    }

    private bool checkString(string toCheck)
    {
        return !String.IsNullOrEmpty(toCheck);
    }

}

using UnityEngine;
using System;
using System.Xml;

public class LevelInfo : LoadableFromXmlImpl
{
    public string code {get; set;}
    public bool inventoryBioBricks {get; set;}
    public bool inventoryDevices {get; set;}
    public bool areAllBioBricksAvailable {get; set;}
    public bool areAllDevicesAvailable {get; set;}

    public override string getTag() { return LevelInfoXMLTags.INFO; }

    //! Create from an XML node a LevelInfo.
    //! \param node The XML node
    public override bool tryInstantiateFromXml(XmlNode node)
    {
        // Debug.Log(this.GetType() + " LevelInfo.tryInstantiateFromXml("+node+") will load");

        if(node.Name == getTag())
        {
            foreach (XmlNode attr in node)
            {
                switch (attr.Name)
                {
                    case LevelInfoXMLTags.CODE:
                        code = attr.InnerText;
                        break;
                    case LevelInfoXMLTags.INVENTORYBIOBRICKS:
                        inventoryBioBricks = Tools.safeGetBool(attr.InnerText);
                        break;
                    case LevelInfoXMLTags.INVENTORYDEVICES:
                        inventoryDevices = Tools.safeGetBool(attr.InnerText);
                        break;
                    case LevelInfoXMLTags.AREALLBIOBRICKSAVAILABLE:
                        areAllBioBricksAvailable = Tools.safeGetBool(attr.InnerText);
                        break;
                    case LevelInfoXMLTags.AREALLDEVICESAVAILABLE:
                        areAllDevicesAvailable = Tools.safeGetBool(attr.InnerText);
                        break;
                    default:
                        Debug.LogWarning(this.GetType() + " loadInfoFromFile unknown attr "+attr.Name+" for info node");
                        break;
                }
            }
        }
        else
        {
            Debug.LogWarning(this.GetType() + " tryInstantiateFromXml no appropriate tag: found '"+node.Name+"'≠ expected '"+getTag()+"'");
        }
        
        // Debug.Log(this.GetType() + " LevelInfo.tryInstantiateFromXml(node) loaded this="+this);
        return true;
    }
    
    public override string ToString ()
    {
        return string.Format ("[LevelInfo code: {0}, areAllBricksAvailable: {1}, areAllDevicesAvailable: {2}]", code, areAllBioBricksAvailable, areAllDevicesAvailable);
    }
}
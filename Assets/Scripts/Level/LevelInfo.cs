using UnityEngine;
using System;
using System.Xml;

public class LevelInfo : LoadableFromXmlImpl
{
    public string code {get; set;}
    public bool areAllBioBricksAvailable {get; set;}
    public bool areAllDevicesAvailable {get; set;}

    //! Create from an XML node a LevelInfo.
    //! \param node The XML node
    public override bool tryInstantiateFromXml(XmlNode node)
    {
        Logger.Log("LevelInfo.tryInstantiateFromXml("+node+") will load", Logger.Level.DEBUG);
        
        foreach (XmlNode attr in node)
        {
            switch (attr.Name)
            {
                case LevelInfoXMLTags.AREALLBIOBRICKSAVAILABLE:
                    areAllBioBricksAvailable = Tools.safeGetBool(attr.InnerText);
                    break;
                case LevelInfoXMLTags.AREALLDEVICESAVAILABLE:
                    areAllDevicesAvailable = Tools.safeGetBool(attr.InnerText);
                    break;
                default:
                    Logger.Log("LevelInfoLoader::loadInfoFromFile unknown attr "+attr.Name+" for info node", Logger.Level.WARN);
                    break;
            }
        }
        
        Logger.Log("LevelInfo.tryInstantiateFromXml(node) loaded this="+this, Logger.Level.DEBUG);
        return true;
    }
    
    public override string ToString ()
    {
        return string.Format ("[LevelInfo _code: {0}, _areAllBricksAvailable: {1}, _areAllDevicesAvailable: {2}]", code, areAllBioBricksAvailable, areAllDevicesAvailable);
    }
}
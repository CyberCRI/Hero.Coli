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
                case LevelInfoXMLTags.CODE:
                    Debug.LogError("CODE "+attr.InnerText+"spotted");
                    code = attr.InnerText;
                    break;
                case LevelInfoXMLTags.AREALLBIOBRICKSAVAILABLE:
                    Debug.LogError("AREALLBIOBRICKSAVAILABLE "+attr.InnerText+"spotted");
                    areAllBioBricksAvailable = Tools.safeGetBool(attr.InnerText);
                    break;
                case LevelInfoXMLTags.AREALLDEVICESAVAILABLE:
                    Debug.LogError("AREALLDEVICESAVAILABLE "+attr.InnerText+"spotted");
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
        return string.Format ("[LevelInfo code: {0}, areAllBricksAvailable: {1}, areAllDevicesAvailable: {2}]", code, areAllBioBricksAvailable, areAllDevicesAvailable);
    }
}
using UnityEngine;
using System;
using System.Xml;

public class LevelInfo : LoadableFromXmlImpl
{
    public string code {get; set;}
    public bool areAllBioBricksAvailable {get; set;}
    public bool areAllDevicesAvailable {get; set;}

    public override string getTag() { return LevelInfoXMLTags.INFO; }

    //! Create from an XML node a LevelInfo.
    //! \param node The XML node
    public override bool tryInstantiateFromXml(XmlNode node)
    {
        Logger.Log("LevelInfo.tryInstantiateFromXml("+node+") will load", Logger.Level.DEBUG);

        if(node.Name == getTag())
        {
            foreach (XmlNode attr in node)
            {
                switch (attr.Name)
                {
                    case LevelInfoXMLTags.CODE:
                        code = attr.InnerText;
                        break;
                    case LevelInfoXMLTags.AREALLBIOBRICKSAVAILABLE:
                        areAllBioBricksAvailable = Tools.safeGetBool(attr.InnerText);
                        break;
                    case LevelInfoXMLTags.AREALLDEVICESAVAILABLE:
                        areAllDevicesAvailable = Tools.safeGetBool(attr.InnerText);
                        break;
                    default:
                        Logger.Log("LevelInfo::loadInfoFromFile unknown attr "+attr.Name+" for info node", Logger.Level.WARN);
                        break;
                }
            }
        }
        else
        {
            Logger.Log("LevelInfo::tryInstantiateFromXml no appropriate tag: found '"+node.Name+"'≠ expected '"+getTag()+"'", Logger.Level.WARN);
        }
        
        Logger.Log("LevelInfo.tryInstantiateFromXml(node) loaded this="+this, Logger.Level.DEBUG);
        return true;
    }
    
    public override string ToString ()
    {
        return string.Format ("[LevelInfo code: {0}, areAllBricksAvailable: {1}, areAllDevicesAvailable: {2}]", code, areAllBioBricksAvailable, areAllDevicesAvailable);
    }
}
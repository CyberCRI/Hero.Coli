using System.Xml;
using System;
using System.Collections.Generic;
using UnityEngine;

//TODO refactor with FileLoader
public class TooltipLoader {

    public  const string _tooltipPrefix     = "TOOLTIP.";
    private const string _titleSuffix       = ".TITLE";
    public  const string _tooltipTypePrefix = "MAIN.TYPE.";
    private const string _subtitleSuffix    = ".SUBTITLE";
    private const string _customFieldSuffix = ".CUSTOMFIELD";
    private const string _customValueSuffix = ".CUSTOMVALUE";
    private const string _lengthSuffix      = ".LENGTH";
    private const string _referenceSuffix   = ".REFERENCE";
    private const string _energySuffix      = ".ENERGYCONSUMPTION";
    private const string _explanationSuffix = ".EXPLANATION";
    
    private string biobrickUpperString = TooltipManager.TooltipType.BIOBRICK.ToString().ToLowerInvariant();
    private string deviceUpperString = TooltipManager.TooltipType.DEVICE.ToString().ToLowerInvariant();
    
    private static string _biobrickKey = _tooltipPrefix+_tooltipTypePrefix+TooltipManager.TooltipType.BIOBRICK.ToString().ToUpperInvariant();
    public static string biobrickKey
    {
        get
        {
            return _biobrickKey;
        }
    }
    private static string _deviceKey = _tooltipPrefix+_tooltipTypePrefix+TooltipManager.TooltipType.DEVICE.ToString().ToUpperInvariant();
    public static string deviceKey
    {
        get
        {
            return _deviceKey;
        }
    }   

    private string _code;
    private string _title;
    private string _type;
    private TooltipManager.TooltipType _tooltipType;
    private string _subtitle;
    private string _illustration;
    private string _customField;
    private string _customValue;
    private string _length;
    private string _reference;
    private string _energyConsumption;
    private string _explanation;

    private TooltipInfo _info;

    private void reinitVars() {
        _code = null;
        _title = null;
        _type = null;
        _tooltipType = TooltipManager.TooltipType.BIOBRICK;
        _subtitle = null;
        _illustration = null;
        _customField = null;
        _customValue = null;
        _length = null;
        _reference = null;
        _energyConsumption = null;
        _explanation = null;
        _info = null;
    }

    public LinkedList<TooltipInfo> loadInfoFromFile(string filePath)
    {
        // Debug.Log(this.GetType() + " loadInfoFromFile("+filePath+")");

        LinkedList<TooltipInfo> resultInfo = new LinkedList<TooltipInfo>();

        XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

        XmlNodeList infoList = xmlDoc.GetElementsByTagName(TooltipXMLTags.TOOLTIP);

        foreach (XmlNode infoNode in infoList)
        {
            reinitVars();
            //common info attributes
            try {
                _code = infoNode.Attributes[TooltipXMLTags.CODE].Value;
            }
            catch (NullReferenceException exc) {
                Debug.LogWarning(this.GetType() + " loadInfoFromFile bad xml, missing field\n"+exc);
                continue;
            }
            catch (Exception exc) {
                Debug.LogWarning(this.GetType() + " loadInfoFromFile failed, got exc="+exc);
                continue;
            }

            if (!String.IsNullOrEmpty(_code))
            {
                foreach (XmlNode attr in infoNode)
                {
                    switch (attr.Name)
                    {
                        case TooltipXMLTags.TITLE:
                            _title = attr.InnerText;
                            break;
                        case TooltipXMLTags.TYPE:
                            _type = attr.InnerText;
                            break;
                        case TooltipXMLTags.SUBTITLE:
                            _subtitle = attr.InnerText;
                            break;
                        case TooltipXMLTags.ILLUSTRATION:
                            _illustration = attr.InnerText;
                            break;
                        case TooltipXMLTags.CUSTOMFIELD:
                            _customField = attr.InnerText;
                            break;
                        case TooltipXMLTags.CUSTOMVALUE:
                            _customValue = attr.InnerText;
                            break;
                        case TooltipXMLTags.LENGTH:
                            _length = attr.InnerText;
                            break;
                        case TooltipXMLTags.REFERENCE:
                            _reference = attr.InnerText;
                            break;
                        case TooltipXMLTags.ENERGYCONSUMPTION:
                            _energyConsumption = attr.InnerText;
                            break;
                        case TooltipXMLTags.EXPLANATION:
                            _explanation = attr.InnerText;
                            break;
                        default:
                            Debug.LogWarning(this.GetType() + " loadInfoFromFile unknown attr "+attr.Name+" for info node");
                            break;
                    }
                }
                if(!String.IsNullOrEmpty(_type))
                {
            
                    string root = _tooltipPrefix+_code.ToUpper().Replace(' ','_');
                    _title = string.IsNullOrEmpty(_title)?root+_titleSuffix:_title;
                    _subtitle = string.IsNullOrEmpty(_subtitle)?root+_subtitleSuffix:_subtitle;
                    _customField = string.IsNullOrEmpty(_customField)?getKeyIfExists(root+_customFieldSuffix):_customField;
                    _customValue = string.IsNullOrEmpty(_customValue)?getKeyIfExists(root+_customValueSuffix):_customValue;
                    _length = string.IsNullOrEmpty(_length)?root+_lengthSuffix:_length;
                    _reference = string.IsNullOrEmpty(_reference)?root+_referenceSuffix:_reference;
                    _energyConsumption = string.IsNullOrEmpty(_energyConsumption)?getKeyIfExists(root+_energySuffix):_energyConsumption;
                    _explanation = string.IsNullOrEmpty(_explanation)?root+_explanationSuffix:_explanation;
                    
                    string lower = _type.ToLowerInvariant();
                    if(lower == TooltipManager.TooltipType.DEVICE.ToString().ToLowerInvariant())
                    {
                        _tooltipType = TooltipManager.TooltipType.DEVICE;
                    }
                    else if(lower == TooltipManager.TooltipType.BIOBRICK.ToString().ToLowerInvariant())
                    {
                        _tooltipType = TooltipManager.TooltipType.BIOBRICK;
                    }
                    else
                    {
                        _tooltipType = TooltipManager.TooltipType.BIOBRICK;
                    }
                    
                    _info = new TooltipInfo(
                        _code,
                        _title,
                        _tooltipType,
                        _subtitle,
                        _illustration,
                        _customField,
                        _customValue,
                        _length,
                        _reference,
                        _energyConsumption,
                        _explanation
                    );
                }
                if(null != _info)
                {
                        resultInfo.AddLast(_info);
                }
            } else {
                Debug.LogWarning(this.GetType() + " loadInfoFromFile Error : missing attribute code in info node");
            }
        }
        return resultInfo;
    }
  
    private static string getKeyIfExists(string code) {
        string res = Localization.Localize(code) == code ? "" : code;
        return res;
    }

}

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
    public const string _customFieldSuffix = ".CUSTOMFIELD";
    private const string _customValueSuffix = ".CUSTOMVALUE";
    private const string _lengthSuffix      = ".LENGTH";
    private const string _referenceSuffix   = ".REFERENCE";
    private const string _energySuffix      = ".ENERGYCONSUMPTION";
    private const string _explanationSuffix = ".EXPLANATION";
    public const string _emptyField        = _tooltipPrefix + "EMPTYFIELD";
    
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
                        // fields that are treated another way - via translation files - are commented out
                        // see next 'if'

                        // case TooltipXMLTags.TITLE:
                        //     _title = attr.InnerText;
                        //     break;
                        case TooltipXMLTags.TYPE:
                            _type = attr.InnerText;
                            break;
                        // case TooltipXMLTags.SUBTITLE:
                        //     _subtitle = attr.InnerText;
                        //     break;
                        case TooltipXMLTags.ILLUSTRATION:
                            _illustration = attr.InnerText;
                            break;
                        // case TooltipXMLTags.CUSTOMFIELD:
                        //     _customField = attr.InnerText;
                        //     break;
                        // case TooltipXMLTags.CUSTOMVALUE:
                        //     _customValue = attr.InnerText;
                        //     break;
                        // case TooltipXMLTags.LENGTH:
                        //     _length = attr.InnerText;
                        //     break;
                        // case TooltipXMLTags.REFERENCE:
                        //     _reference = attr.InnerText;
                        //     break;
                        // case TooltipXMLTags.ENERGYCONSUMPTION:
                        //     _energyConsumption = attr.InnerText;
                        //     break;
                        // case TooltipXMLTags.EXPLANATION:
                        //     _explanation = attr.InnerText;
                        //     break;
                        default:
                            Debug.LogWarning(this.GetType() + " loadInfoFromFile unknown attr "+attr.Name+" for info node");
                            break;
                    }
                }
                if(!String.IsNullOrEmpty(_type))
                {
            
                    string root = getKeyRoot(_code);
                    _title = getField(root, _title, _titleSuffix);
                    _subtitle = getField(root, _subtitle, _subtitleSuffix);
                    _customField = getFieldIfExists(root, _customField, _customFieldSuffix);
                    _customValue = getFieldIfExists(root, _customValue, _customValueSuffix);
                    _length = getField(root, _length, _lengthSuffix);
                    _reference = getField(root, _reference, _referenceSuffix);
                    _energyConsumption = getFieldIfExists(root, _energyConsumption, _energySuffix);
                    _explanation = getField(root, _explanation, _explanationSuffix);
                    
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

    public static string getKeyRoot(string tooltipCode)
    {
        return _tooltipPrefix+tooltipCode.ToUpper().Replace(' ','_');
    }
    public static string getField(string root, string defaultValue, string suffix)
    {
        return string.IsNullOrEmpty(defaultValue)?root+suffix:defaultValue;
    }
    public static string getFieldIfExists(string root, string defaultValue, string suffix)
    {
        return string.IsNullOrEmpty(defaultValue)?getLocalizationKeyIfExists(root+suffix):defaultValue;
    }
    public static string getCustomField(string root)
    {
        return getLocalizationKeyIfExists(root+_customFieldSuffix);
    }
    public static string getCustomValue(string root)
    {
        return getLocalizationKeyIfExists(root+_customValueSuffix);
    }
    public static string getEnergyConsumption(string root)
    {
        return getLocalizationKeyIfExists(root+_energySuffix);
    }
    public static string getLength(string root)
    {
        return root + _lengthSuffix;
    }
    public static string getReference(string root)
    {
        return root + _energySuffix;
    }
    public static string getExplanation(string root)
    {
        return root + _explanationSuffix;
    }
  
    private static string getLocalizationKeyIfExists(string code) {
        string res = Localization.Localize(code) == code ? _emptyField : code;
        return res;
    }



}

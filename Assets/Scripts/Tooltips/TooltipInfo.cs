using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipInfo
{
  public string _code;
  public string _background;
  public string _title;
  public string _type;
  public TooltipManager.TooltipType _tooltipType;
  public string _subtitle;
  public string _illustration;
  public string _customField;
  public string _customValue;
  public string _length;
  public string _reference;
  public string _energyConsumption;
  public string _explanation;

  public TooltipInfo(
    string code,
    string title,
    string type,
    string subtitle,
    string illustration,
    string customField,
    string customValue,
    string length,
    string reference,
    string energyConsumption,
    string explanation
    )
  {
    _code              = code;
    _title             = title;
    _type              = type;
    _subtitle          = subtitle;
    _illustration      = illustration;
    _customField       = customField;
    _customValue       = customValue;
    _length            = length;
    _reference         = reference;
    _energyConsumption = energyConsumption;
    _explanation       = explanation;

    string lower = _type.ToLower();
    if(lower == "device")
    {
      _background = TooltipManager.getDeviceBackground();
      _tooltipType = TooltipManager.TooltipType.DEVICE;
    }
    else if(lower == "biobrick")
    {
      _background = TooltipManager.getBioBrickBackground();
      _tooltipType = TooltipManager.TooltipType.BIOBRICK;
    }
    else
    {
      _background = TooltipManager.getBioBrickBackground();
      _tooltipType = TooltipManager.TooltipType.BIOBRICK;
    }
  }

  public override string ToString ()
  {
    return string.Format ("[TooltipInfo " +
      "_code:"+_code+
      ", _background:"+_background+
      ", _title:"+_title+
      ", _type:"+_type+
      ", _subtitle:"+_subtitle+
      ", _illustration:"+_illustration+
      ", _customField:"+_customField+
      ", _customValue:"+_customValue+
      ", _length:"+_length+
      ", _reference:"+_reference+
      ", _energyConsumption:"+_energyConsumption+
      ", _explanation:"+_explanation+
      "]");
  }
}


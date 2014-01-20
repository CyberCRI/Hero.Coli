using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipInfo
{
  public string _code;
  public string _title;
  public string _type;
  public string _subtitle;
  public string _texture;
  public string _length;
  public string _reference;
  public string _explanation;

  public TooltipInfo(
    string code,
    string title,
    string type,
    string subtitle,
    string texture,
    string length,
    string reference,
    string explanation
    )
  {
    _code        = code;
    _title       = title;
    _type        = type;
    _subtitle    = subtitle;
    _texture     = texture;
    _length      = length;
    _reference   = reference;
    _explanation = explanation;
  }

  public override string ToString ()
  {
    return string.Format ("[TooltipInfo " +
      "_code:"+_code+
      ", _title:"+_title+
      ", _type:"+_type+
      ", _subtitle:"+_subtitle+
      ", _texture:"+_texture+
      ", _length:"+_length+
      ", _reference:"+_reference+
      ", _explanation:"+_explanation+
      "]");
  }
}

